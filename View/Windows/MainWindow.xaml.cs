using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Controls;
using college_events_desktop.View.Layers.Events;
using college_events_desktop.View.Layers.Groups;
using college_events_desktop.View.Layers.Organizers;
using college_events_desktop.View.Layers.Settings;
using college_events_desktop.View.Layers.Users;
using college_events_desktop.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace college_events_desktop.View.Windows
{
	public partial class MainWindow : Window
	{
        #region Поля класса
        //UI элементы
        page_EventList eventList;
        page_OrganizerList organizerList;
        page_GroupList groupList;
        page_UserList userList;
        page_NewAccount newAccount;
        events_nav_buttons events_Nav_Buttons;
        users_nav_buttons users_Nav_Buttons;

        //сервисы
        internal DataService _dataService { get; private set; }
        internal readonly IOverlayService _overlayService;

        //остальные переменные
        #endregion


        #region Конструктор
        public MainWindow(DataService dataService)
		{
			InitializeComponent();
            LoadingService.Register(this); //регистрация сервиса интерфейса загрузки

            //присвоение переменным значений
            _dataService = dataService;

            eventList = new page_EventList(this, _dataService);
            organizerList = new page_OrganizerList(this, _dataService);
            groupList = new page_GroupList(this, _dataService);
            userList = new page_UserList(this, _dataService);
            newAccount = new page_NewAccount();
			events_Nav_Buttons = new events_nav_buttons(this, eventList);
            users_Nav_Buttons = new users_nav_buttons(this, userList, newAccount);
            _overlayService = new OverlayService(this);

            //назначение событий
            Loaded += MainWindow_Loaded;
            EventNotification.StatusChanged += EventNotification_StatusChanged;
		}

        #endregion


        #region Обработчики событий

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
		{
			mainframe.Navigate(eventList);
        }


        private bool isNavButtonTextVisible = true;
        private void btn_nav_menu_drop_Click(object sender, RoutedEventArgs e)
        {
            //меняем состояние флага
            isNavButtonTextVisible = !isNavButtonTextVisible;

            //целевая ширина для Border
            double targetWidth = isNavButtonTextVisible ? 240 : 90;
            
            DoubleAnimation widthAnimation = new DoubleAnimation
            {
                From = border_basement_menu.ActualWidth,
                To = targetWidth,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseInOut }
            };

            //управление моментом отображения текста на кнопках навигации
            if (isNavButtonTextVisible)
            {
                //если меню открывается, включаем текст сразу, чтобы он плавно выезжал
                ShowNavButtonsText(true);
            }
            else
            {
                //если меню закрывается, ждем окончания анимации, чтобы текст не пропал резко в процессе
                widthAnimation.Completed += (s, args) =>
                {
                    if (!isNavButtonTextVisible)
                    {
                        ShowNavButtonsText(false);
                    }
                };
            }
            border_basement_menu.BeginAnimation(Border.WidthProperty, widthAnimation);
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



        private page_Settings page_settings = new page_Settings();
        private void btn_settings_Click(object sender, RoutedEventArgs e)
        {
            _overlayService.Open(page_settings);
        }



        private void btn_close_overlay_Click(object sender, RoutedEventArgs e)
        {
            _overlayService.Close();
        }


        private void btn_organizers_Click(object sender, RoutedEventArgs e)
        {
            mainframe.Navigate(organizerList);
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

        private void ShowNavButtonsText(bool state)
        {
            Visibility visibility = state ? Visibility.Visible : Visibility.Collapsed;
            text_menu_events.Visibility = visibility;
            text_menu_organizers.Visibility = visibility;
            text_menu_groups.Visibility = visibility;
            text_menu_supervisors.Visibility = visibility;
            text_menu_users.Visibility = visibility;
        }
        #endregion

        private void btn_groups_Click(object sender, RoutedEventArgs e)
        {
            mainframe.Navigate(groupList);
        }
    }
}
