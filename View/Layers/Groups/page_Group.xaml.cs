using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Controls;
using college_events_desktop.View.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace college_events_desktop.View.Layers.Groups
{
    public partial class page_Group : Page
    {
        #region Поля класса


        MainWindow _mainWindow;
        DataService _dataService;
        Group _group;
        
        
        #endregion



        #region Конструктор

        public page_Group(MainWindow mainWindow, DataService dataService, Group group)
        {
            InitializeComponent();
            _mainWindow = mainWindow;
            _dataService = dataService;
            _group = group;
            DataContext = _group;

            Loaded += OnLoaded;
        }


        #endregion



        #region Обработчики событий

        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            _mainWindow.Title = StringConstants.mainWindow_Title_GroupSeeStatistic;
            using (LoadingService.StartLoading())
            {
                try
                {
                    await _dataService.LoadGroupStatisticAsync(_group.groupId);
                    datagrid_events.ItemsSource = _dataService.groupStatistic.events;

                    await _dataService.LoadCategoriesAsync();
                    var categories = new List<string> { "Не выбрано" };
                    categories.AddRange(_dataService.categories.Select(c => c.name));

                    combobox_categories.ItemsSource = categories;
                    combobox_categories.SelectedIndex = 0;
                }
                catch (Exception ex)
                {
                    UserNotificationService.ShowError("Ошибка получения сведений о группе.", ex, "page_GroupAA001");
                }
            }
        }

        private void search_textChanged(object sender, TextChangedEventArgs e)
        {
            if (!(sender is SearchBar textBox)) return;
            string searchText = textBox.SearchText.ToLower().Trim();
            try
            {
                datagrid_events.ItemsSource = _dataService.groupStatistic.events.Where(ev => ev.title.ToLower().Trim().Contains(searchText));
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Ошибка применения поиска по тексту.", ex, "page_GroupAA002");
            }
        }

        private void combobox_categories_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(sender is ComboBox comboBox)) return;
            if (comboBox.SelectedIndex == 0)
            {
                datagrid_events.ItemsSource = _dataService.groupStatistic.events;
                return;
            }

            string searchText = comboBox.SelectedItem.ToString() ?? string.Empty;
            string lowerSearchText = searchText.ToLower().Trim();

            try
            {
                datagrid_events.ItemsSource = _dataService.groupStatistic.events.Where(ev => ev.categoryName.ToLower().Trim().Contains(lowerSearchText));
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Ошибка применения фильтра по направлению (категории).", ex, "page_GroupAA003");
            }
        }

        private void goback_Click(object sender, RoutedEventArgs e)
        {
            _mainWindow.mainframe.GoBack();
        }

        private void btn_export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileName = FileNameSecurity.Build(
                    ".xlsx",
                    maxLength: 200,
                    "Отчёт", _group.groupName, DateTime.Now.ToString("dd-MM-yyyy")
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
                        "Куратор",
                        edit_supervisor.Text
                    )
                };

                var table = new ExportTable
                {
                    Columns = new List<ExportColumn>
                    {
                        new ExportColumn
                        {
                            Header = "Мероприятие",
                            ValueSelector = obj => ((GroupEvents)obj).title
                        },
                        new ExportColumn
                        {
                            Header = "Слушатели",
                            ValueSelector = obj => ((GroupEvents)obj).actualListenersCount.ToString()
                        },
                        new ExportColumn
                        {
                            Header = "Участники",
                            ValueSelector = obj => ((GroupEvents)obj).actualParticipantsCount.ToString()
                        },
                        new ExportColumn
                        {
                            Header = "Супер-участники",
                            ValueSelector = obj => ((GroupEvents)obj).actualSuperParticipantsCount.ToString()
                        },
                        new ExportColumn
                        {
                            Header = "Общий балл",
                            ValueSelector = obj => ((GroupEvents)obj).totalScore.ToString()
                        }
                    },
                    Rows = _dataService.groupStatistic.events.Cast<object>().ToList()
                };

                ExportDataService service = new ExportDataService();
                service.Export(sfd.FileName, header, table);
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Ошибка экспорта данных", ex, "page_GroupAA004");
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


        #endregion
    }
}
