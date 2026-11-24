using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Controls;
using college_events_desktop.View.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace college_events_desktop.View.Layers.Supervisors
{
    /// <summary>
    /// Логика взаимодействия для page_Supervisor.xaml
    /// </summary>
    public partial class page_Supervisor : Page
    {
        #region Поля класса
        MainWindow _mainWindow;
        DataService _dataService;
        AuthorizedUser _authorizedUser;
        #endregion


        public page_Supervisor(MainWindow mainWindow, DataService dataService, AuthorizedUser authorizedUser)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _dataService = dataService;
            _authorizedUser = authorizedUser;
            DataContext = _authorizedUser;

            Loaded += Page_Supervisor_Loaded;
        }

        private async void Page_Supervisor_Loaded(object sender, RoutedEventArgs e)
        {
            _mainWindow.Title = StringConstants.mainWindow_Title_SupervisorSeeStatistic;
            using (LoadingService.StartLoading())
            {
                try
                {
                    await _dataService.LoadSupervisorStatisticAsync(_authorizedUser.AuthorizedUserId);
                    DataContext = _dataService.supervisorStatistic;

                    await _dataService.LoadCategoriesAsync();
                    var categories = new List<string> { "Не выбрано" };
                    categories.AddRange(_dataService.categories.Select(c => c.name));

                    combobox_categories.ItemsSource = categories;
                    combobox_categories.SelectedIndex = 0;

                    stack_groups.Children.Clear();

                    foreach (var group in _dataService.supervisorStatistic.supervisorGroups)
                    {
                        SimpleTextPlaceholder text = new SimpleTextPlaceholder(group.groupName, Colors.White, Color.FromArgb(255, 102, 102, 102), Color.FromRgb(102, 102, 102))
                        {
                            Margin = new Thickness(0, 0, 10, 0)
                        };
                        stack_groups.Children.Add(text);
                    }

                    datagrid_events.ItemsSource = _dataService.supervisorStatistic.events;
                }
                catch (Exception ex)
                {
                    UserNotificationService.ShowError("Ошибка получения информации куратора.", ex, "page_SupervisorAA001");
                }
            }
        }

        private void search_textChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is SearchBar textBox)) return;
            string searchText = textBox.SearchText.ToLower().Trim();
            try
            {
                datagrid_events.ItemsSource = _dataService.supervisorStatistic.events.Where(ev => 
                    ev.title.ToLower().Trim().Contains(searchText) 
                    || ev.groupName.ToLower().Trim().Contains(searchText) 
                );
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Ошибка применения поиска по тексту.", ex, "page_SupervisorAA002");
            }
        }

        private void combobox_categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ComboBox comboBox)) return;
            if (comboBox.SelectedIndex == 0)
            {
                datagrid_events.ItemsSource = _dataService.supervisorStatistic.events;
                return;
            }

            string searchText = comboBox.SelectedItem.ToString() ?? string.Empty;
            string lowerSearchText = searchText.ToLower().Trim();

            try
            {
                datagrid_events.ItemsSource = _dataService.supervisorStatistic.events.Where(ev => ev.categoryName.ToLower().Trim().Contains(lowerSearchText));
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Ошибка применения фильтра по направлению (категории).", ex, "page_SupervisorAA003");
            }
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
                        "Количество направлений мероприятия",
                        edit_categories_count.Text
                    ),
                    new KeyValuePair<string, string>(
                        "Баллы",
                        edit_totalScore.Text
                    ),
                    new KeyValuePair<string, string>(
                        "Группы",
                        string.Join(", ", stack_groups.Children.OfType<SimpleTextPlaceholder>().Select(t => t.Text))
                    )
                };

                var table = new ExportTable
                {
                    Columns = new List<ExportColumn>
                    {

                        new ExportColumn
                        {
                            Header = "Группа",
                            ValueSelector = obj => ((SupervisorEvent)obj).groupName
                        },
                        new ExportColumn
                        {
                            Header = "Мероприятие",
                            ValueSelector = obj => ((SupervisorEvent)obj).title
                        },
                        new ExportColumn
                        {
                            Header = "Слушатели",
                            ValueSelector = obj => ((SupervisorEvent)obj).actualListenersCount.ToString()
                        },
                        new ExportColumn
                        {
                            Header = "Участники",
                            ValueSelector = obj => ((SupervisorEvent)obj).actualParticipantsCount.ToString()
                        },
                        new ExportColumn
                        {
                            Header = "Супер-участники",
                            ValueSelector = obj => ((SupervisorEvent)obj).actualSuperParticipantsCount.ToString()
                        },
                        new ExportColumn
                        {
                            Header = "Общий балл",
                            ValueSelector = obj => ((SupervisorEvent)obj).totalScore.ToString()
                        }
                    },
                    Rows = _dataService.supervisorStatistic.events.Cast<object>().ToList()
                };

                ExportDataService service = new ExportDataService();
                service.Export(sfd.FileName, header, table);
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Ошибка экспорта данных", ex, "page_SupervisorAA004");
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
    }
}
