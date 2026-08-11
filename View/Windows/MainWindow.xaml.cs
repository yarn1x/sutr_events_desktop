using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Controls;
using college_events_desktop.View.Layers;
using college_events_desktop.ViewModels;
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace college_events_desktop.View.Windows
{
	public partial class MainWindow : Window
	{
        #region Поля класса
        //различные UI элементы
        page_EventList eventList;
        events_nav_buttons events_Nav_Buttons;
        users_nav_buttons users_Nav_Buttons;

        //сервисы
        internal DataService _dataService { get; private set; }
        internal readonly IOverlayService _overlayService;

        //остальные переменные
        private bool isNavButtonTextVisible = true;
        #endregion


        #region Конструктор
        public MainWindow(DataService dataService)
		{
			InitializeComponent();

            //присвоение переменным значений
            _dataService = dataService;
            eventList = new page_EventList(this, _dataService);
			events_Nav_Buttons = new events_nav_buttons(this, eventList);
            users_Nav_Buttons = new users_nav_buttons(this);
            _overlayService = new OverlayService(this);

            //назначение событий
            Loaded += MainWindow_Loaded;
            EventNotification.StatusChanged += EventNotification_StatusChanged;
		}

        #endregion


        #region Обработчики событий

        private void btn_nav_menu_drop_Click(object sender, RoutedEventArgs e)
        {
            if (isNavButtonTextVisible)
            {
                grid_main.ColumnDefinitions[0].Width = new GridLength(90);
                isNavButtonTextVisible = false;
            }
            else
            {
                grid_main.ColumnDefinitions[0].Width = new GridLength(240);
                isNavButtonTextVisible = true;
            }
        }


        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			mainframe.Navigate(eventList);
			await eventList.Page_EventList_Loaded();
        }


        private void EventNotification_StatusChanged(object sender, EventStatusChangedEventArgs e)
        {
            //показываем, если есть элементы с tag 1 или -1
            ellipse_new_event_status.Visibility = e.HasElements
                ? Visibility.Visible
                : Visibility.Collapsed;   
        }


        private bool isEventsNavMenuOpened = false;
        
        private void open_events_nav_menu(object sender, RoutedEventArgs e)
		{
            if (!isEventsNavMenuOpened)
			{
				border_events_caret.OpacityMask = change_caret(isEventsNavMenuOpened);
                submenu_events.Content = events_Nav_Buttons;
				isEventsNavMenuOpened = true;
			}
			else
			{   
				border_events_caret.OpacityMask = change_caret(isEventsNavMenuOpened);
				submenu_events.Content = null;
				isEventsNavMenuOpened = false;
			}
		}


        private bool isSupervisorsNavMenuOpened = false;
        private void open_users_nav_menu_Click(object sender, RoutedEventArgs e)
        {
            if (!isSupervisorsNavMenuOpened)
            {
                border_supervisors_caret.OpacityMask = change_caret(isSupervisorsNavMenuOpened);
                submenu_users.Content = users_Nav_Buttons;
                isSupervisorsNavMenuOpened = true;
            }
            else
            {
                border_supervisors_caret.OpacityMask = change_caret(isSupervisorsNavMenuOpened);
                submenu_users.Content = null;
                isSupervisorsNavMenuOpened = false;
            }
        }

        private void btn_close_overlay_Click(object sender, RoutedEventArgs e)
        {
            _overlayService.Close();
        }

        /// <summary>
        /// По завершению работы окна
        /// </summary>
        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            //дизлайк, отписка от событий
            Loaded -= MainWindow_Loaded;
            EventNotification.StatusChanged -= EventNotification_StatusChanged;
        }
        #endregion


        #region Методы класса

        /// <summary>
        /// Метод, изменяющий направление индикатора развернутости меню.
        /// </summary>
        /// <param name="isNavMenuOpened">Развёрнуто ли меню</param>
        /// <returns>Кисть с изображением, взятое из App.xaml</returns>
        private ImageBrush change_caret(bool isNavMenuOpened)
		{
			BitmapImage bitmap;
			ImageBrush brush;
			if (!isNavMenuOpened)
			{
				bitmap = (BitmapImage)Application.Current.FindResource("caret_up");
				brush = new ImageBrush(bitmap);
			}
			else
			{
				bitmap = (BitmapImage)Application.Current.FindResource("caret_down");
				brush = new ImageBrush(bitmap);
			}
			brush.RelativeTransform = new ScaleTransform(1.5, 1.5, 0.5, 0.5);
			return brush;
		}

        public void ShowLoading(bool show)
        {
            loading_overlay.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
            grid_main.IsEnabled = !show;
        }

        #endregion

    }
}
