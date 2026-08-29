using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Layers.Events;
using college_events_desktop.View.Windows;
using college_events_desktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace college_events_desktop.View.Controls
{
	public partial class event_element : UserControl
	{
		#region Поля класса
		private readonly MainWindow mainWindow;
		private page_EventList _page;
		private DataService _dataService;
		private Event _Event;

		//Сопоставление id статуса с его цветом
		private Dictionary<string, Color> markerColors = new Dictionary<string, Color>()
		{
            {"-1", Color.FromArgb(0xFF, 0xFF, 0x49, 0x49)}, //красный
            {"1", Color.FromArgb(0xFF, 0xFF, 0xDD, 0x3C)}, //жёлтый
            {"2", Color.FromArgb(0xFF, 0x66, 0xFF, 0x3D)}, //зелёный
            {"3", Color.FromArgb(0xFF, 0x48, 0xAF, 0xFF)}, //голубой
            {"4", Color.FromArgb(0xFF, 0xA7, 0xA7, 0xA7)}, //серый
            {"5", Color.FromArgb(0xFF, 0xA6, 0xFF, 0x7D)}, //перенесено мероприятие (цвет чуть светлее зелёного)
            {"6", Color.FromArgb(0xFF, 0x8C, 0x4F, 0x1B)}, //тёмно-оранжевый
        };

		private readonly ILoadingService _loading;
        #endregion



        #region Конструктор
        public event_element(MainWindow win, page_EventList page, DataService dataService, Event _event)
		{
			InitializeComponent();
            
			mainWindow = win;
			_page = page;
			_dataService = dataService;
			_Event = _event;
			DataContext = _Event;
			Loaded += Event_element_Loaded;
			_loading = new LoadingService(win);

			SetElementTag();
		}
        #endregion



		#region Обработчики событий

        private void Event_element_Loaded(object sender, RoutedEventArgs e)
		{
			//меняем цвет точки слева сверху на плашке мероприятия в зависимости от статуса (статус хранится в тэге элемента. Почему именно в тэге? Потому что я сделал фильтр по статусу именно по тэгу и переделывать мне впадлу. Тебе же лучше. Потренируешься чистить чужой код)
			event_status.Background = new SolidColorBrush(markerColors[Tag.ToString()]);

			//добавляем инфу, которую просто так через привязку не добавить
			try
			{
				debug_eventId.Text = _Event.eventId.ToString();
				text_place.Text = _Event.locations[0].place.ToString();
			} catch { }

			//добавление числа к локации (+Х локаций проведения мероприятия)
			if (_Event.locations.Count > 1)
			{
				bg_text_additional_places_count.Visibility = Visibility.Visible;
				text_additional_places_count.Text = $"+{_Event.locations.Count - 1}";
            }

			//изменение видимости кнопок действий над мероприятиями в зависимости от статуса
			//действия ограничиваются чисто этим условием.
			//АККУРАТНО с добавлением кнопки просмотра отчёта. Если отчёт не будет составлен, ему неоткуда будет брать инфу
			switch (Tag.ToString())
			{
				case "1"://статус жёлтый
					btn_edit.Visibility = Visibility.Visible;
					stack_electoral_buttons.Visibility = Visibility.Visible;
					return;

				case "2"://статус зелёный
					btn_edit.Visibility = Visibility.Visible;
					return;

				case "3"://статус синий
					btn_create_event_report.Visibility = Visibility.Visible;
					return;

				case "4"://статус серый
					btn_see_event_report.Visibility = Visibility.Visible;
					return;

				case "5"://статус зелёный (перенесённое мероприятие)
					btn_edit.Visibility = Visibility.Visible;
					return;

				case "-1"://статус красный
                    btn_edit.Visibility = Visibility.Visible;
                    stack_electoral_buttons.Visibility = Visibility.Visible;
					return;
			}
		}


		private void btn_create_event_report_Click(object sender, RoutedEventArgs e)
		{
			mainWindow.mainframe.Navigate(new page_EventList_save(mainWindow, _dataService, _Event));
		}


		private void btn_see_event_report_Click(object sender, RoutedEventArgs e)
		{
			mainWindow.mainframe.Navigate(new page_EventList_seeEvent(mainWindow, _dataService, _Event));
		}


		private void btn_edit_Click(object sender, RoutedEventArgs e)
		{
			var page = new page_EventList_edit(mainWindow, _dataService, _Event)
			{
				Title = _Event.title + ". Редактирование мероприятия"
			};
			mainWindow.mainframe.Navigate(page);
		}


        private async void btn_claim_event_Click(object sender, RoutedEventArgs e)
        {
			if ((int)Tag == -1) 
			{ 
				MessageBoxResult user_answer = MessageBox.Show($"У этого мероприятия есть конфликт времени с другим мероприятием. Принять проведение?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (user_answer == MessageBoxResult.Yes)
                {
					await this.UpdateEventStatus(2, "принять");
                }
				return;
            }
			await this.UpdateEventStatus(2, "принять");
        }


        private async void btn_reject_event_Click(object sender, RoutedEventArgs e)
        {
			MessageBoxResult user_answer = MessageBox.Show($"Удалить проведение {_Event.title}?", "Вопрос", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (user_answer == MessageBoxResult.Yes)
			{
				await this.UpdateEventStatus(6, "удалить");
            }
        }


        private async void event_text_copy_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				//копирование информации в буфер

				var elem = sender as Button;
				//когда пользователь нажимает на кнопку копирования информации,
				//внутри кнопки находится иконка копирования (border) и текстблок.
				//КНОПКА имеет строго определённый в разметке тэг
				//тэг имеет НАЗВАНИЕ то же, что и НАИМЕНОВАНИЕ текстблока внутри кнопки
				//что бы найти текст для копирования, программа ищет элемент с именем как в тэге кнопки
				var text = elem.FindName(elem.Tag.ToString()) as TextBlock; //тупее ничего придумать нельзя было. За то все кнопки обращаются к одному методу
				Clipboard.SetText(text.Text);


				//отображение плашки "Скопировано!"

				Point position = Mouse.GetPosition(grid_main); //вычисление позиции курсора на экране
				position.Y = position.Y - 40;
				TextPlaceholder textBlock = new TextPlaceholder(grid_main, position, "Скопировано!");
                await textBlock.ShowAsync();
                await Task.Delay(1300);
                await textBlock.HideAsync();

            }
			catch (Exception ex)
			{
				MessageBox.Show($"Ошибка копирования информации в буфер\n\nCode=event_elementAA001\nMessage={ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
        #endregion



        #region Методы класса

        private void SetElementTag()
        {
            //проверка на конфликт времени
            var sameDayEvents = _dataService.events
                .Where(d => d.startDateTime.Date == _Event.startDateTime.Date && d != _Event)
                .ToList();
            bool hasConflict = sameDayEvents.Any(otherEvent =>
            {
                DateTime currentStart = _Event.startDateTime;
                DateTime? currentEnd = _Event.endDateTime;
                DateTime otherStart = otherEvent.startDateTime;
                DateTime? otherEnd = otherEvent.endDateTime;

                return (currentStart < otherEnd && currentEnd > otherStart);
            });
			
			//присвоение тэга элементу
            Tag = _Event.statusId;
            //если есть конфликт времени с другим мероприятием, тэг = -1
            if (hasConflict && (int)Tag == 1) Tag = -1;
            //если мероприятие перенесено (statusId=5), то всё равно в тэг запишем 2, потому что так оно попадёт в выборку фильтра по статусу
            else if ((int)Tag == 5) Tag = 2; 
        }


		/// <summary>
		/// Метод обновления статуса мероприятия после принятия или отклонения мероприятия на проведение
		/// </summary>
		/// <param name="statusId">айдишник статуса</param>
		/// <param name="action">"удалить" или "принять" вписывается в MessageBox</param>
		/// <returns></returns>
		private async Task UpdateEventStatus(int statusId, string action)
		{
            try
            {
                using (_loading.StartLoading()) //отображение loading_overlay
                {
                    //запрос на обновление статуса мероприятия
                    var response = await _dataService.apiClient.UpdateEventStatus(_Event.eventId, statusId);
                    //если успешно, обновляем страницу мероприятий
                    if (response)
                    {
                        await _page.Page_EventList_Loaded();
                    }
                    else
                    {
                        MessageBox.Show($"Не получилось {action} проведение мероприятия.\nПопробуйте позже.", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                } //после завершения инструкций внутри using, loading_overlay скроется
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не получилось {action} проведение мероприятия.\nПопробуйте позже.\nCode=event_elementAA002\nMessage={ex.Message}", "Ошибка!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
		        
		#endregion
	}
}
