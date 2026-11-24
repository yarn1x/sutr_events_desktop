using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Controls;
using college_events_desktop.View.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace college_events_desktop.View.Layers.Organizers
{
    public partial class page_Organizer : Page
    {
        #region Поля класса
        MainWindow _mainWindow;
        DataService _dataService;
        AuthorizedUser _authorizedUser;
        #endregion


        #region Конструктор
        public page_Organizer(MainWindow mainWindow, DataService dataService, AuthorizedUser authorizedUser)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _dataService = dataService;
            _authorizedUser = authorizedUser;
            DataContext = _authorizedUser;

            Loaded += Page_Organizer_Loaded;
        }
        #endregion


        #region Обработчики событий
        private async void Page_Organizer_Loaded(object sender, RoutedEventArgs e)
        {
            _mainWindow.Title = StringConstants.mainWindow_Title_OrganizerSeeStatistic;

            using (LoadingService.StartLoading())
            {
                try
                {
                    await _dataService.LoadOrganizerStatisticAsync(_authorizedUser.AuthorizedUserId);
                    DataContext = _dataService.organizerStatistic;
                    datagrid_events.ItemsSource = _dataService.organizerStatistic.events;

                    await _dataService.LoadCategoriesAsync();
                    var categories = new List<string> { "Не выбрано" };
                    categories.AddRange(_dataService.categories.Select(c => c.name));

                    combobox_categories.ItemsSource = categories;
                    combobox_categories.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    UserNotificationService.ShowError("Ошибка получения данных.", ex, "page_OrganizerAA001");
                }
            }
        }

        private void search_textChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is SearchBar textBox)) return;
            string searchText = textBox.SearchText.ToLower().Trim();
            try
            {
                datagrid_events.ItemsSource = _dataService.organizerStatistic.events.Where(ev => ev.title.ToLower().Trim().Contains(searchText));
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Ошибка применения поиска по тексту.", ex, "page_OrganizerAA002");
            }
        }

        private void combobox_categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ComboBox comboBox)) return;
            if (comboBox.SelectedIndex == 0)
            {
                datagrid_events.ItemsSource = _dataService.organizerStatistic.events;
                return;
            }

            string searchText = comboBox.SelectedItem.ToString() ?? string.Empty;
            string lowerSearchText = searchText.ToLower().Trim();

            try
            {
                datagrid_events.ItemsSource = _dataService.organizerStatistic.events.Where(ev => ev.CategoryName.ToLower().Trim().Contains(lowerSearchText));
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Ошибка применения фильтра по направлению (категории).", ex, "page_OrganizerAA003");
            }
        }

        private void datepicker_date_changed(object sender, SelectionChangedEventArgs e)
        {
            ApplyDateFilter();
        }

        private void btn_export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileName = FileNameSecurity.Build(
                    ".xlsx",
                    maxLength: 200,
                    "Отчёт", _authorizedUser.user.surName, _authorizedUser.user.firstName, _authorizedUser.user.lastName, DateTime.Now.ToString("dd-MM-yyyy")
                );
                var sfd = new SaveFileDialog
                {
                    Filter = "Excel файл (*.xlsx)|*.xlsx",
                    FileName = fileName
                };
                if (sfd.ShowDialog() != true) return;

                var header = new List<KeyValuePair<string, string>>()
                {
                    new KeyValuePair<string, string>(
                        "Количество мероприятий",
                        edit_events_count.Text
                    ),
                    new KeyValuePair<string, string>(
                        "Количество студентов",
                        edit_students_count.Text
                    ),
                    new KeyValuePair<string, string>(
                        "Количество кураторов",
                        edit_supervisors_count.Text
                    )
                };

                var table = new ExportTable
                {
                    Columns = new List<ExportColumn>
                    {
                        new ExportColumn
                        {
                            Header = "Мероприятие",
                            ValueSelector = obj => ((OrganizerEvents)obj).title
                        },
                        new ExportColumn
                        {
                            Header = "Дата и время",
                            ValueSelector = obj => ((OrganizerEvents)obj).startDatetime.ToString("G")
                        },
                        new ExportColumn
                        {
                            Header = "Описание",
                            ValueSelector = obj => ((OrganizerEvents)obj).fullDescription
                        },
                        new ExportColumn
                        {
                            Header = "Слушатели",
                            ValueSelector = obj => ((OrganizerEvents)obj).actualListenersCount.ToString()
                        },
                        new ExportColumn
                        {
                            Header = "Участники",
                            ValueSelector = obj => ((OrganizerEvents)obj).actualParticipantsCount.ToString()
                        },
                        new ExportColumn
                        {
                            Header = "Супер-участники",
                            ValueSelector = obj => ((OrganizerEvents)obj).actualSuperParticipantsCount.ToString()
                        },
                        new ExportColumn
                        {
                            Header = "Общий балл",
                            ValueSelector = obj => ((OrganizerEvents)obj).totalScore.ToString()
                        }
                    },
                    Rows = _dataService.organizerStatistic.events.Cast<object>().ToList()
                };

                ExportDataService service = new ExportDataService();
                service.Export(sfd.FileName, header, table);
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Ошибка экспорта данных", ex, "page_OrganizerAA004");
            }

        }

        private async void btn_open_diagram_Click(object sender, RoutedEventArgs e)
        {
            Point point = new Point()
            {
                X = 100,
                Y = 0
            };
            TextPlaceholder text = new TextPlaceholder(stack_nav, point, "Пока в разработке.")
            {
                VerticalAlignment = VerticalAlignment.Center,
            };
            await text.ShowAsync();
            await Task.Delay(1000);
            await text.HideAsync();
        }

        #endregion


        #region Методы класса
        private void ApplyDateFilter()
        {
            DateTime? from = datepicker_from.SelectedDate;
            DateTime? to = datepicker_to.SelectedDate;
            if (from != null && to == null)
            {
                datagrid_events.ItemsSource = _dataService.organizerStatistic.events.Where(evnt => evnt.startDatetime.Date == from);
            }
            if (from != null && to != null)
            {
                datagrid_events.ItemsSource = _dataService.organizerStatistic.events.Where(evnt => evnt.startDatetime >= from && evnt.startDatetime <= to);
            }
        }

        #endregion

    }
}
