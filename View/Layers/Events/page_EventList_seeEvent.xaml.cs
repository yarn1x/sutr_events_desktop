using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.Services;
using college_events_desktop.View.Controls;
using college_events_desktop.View.Helpers;
using college_events_desktop.View.Windows;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace college_events_desktop.View.Layers.Events
{
    public partial class page_EventList_seeEvent : Page
    {

        private MainWindow mainWindow;
        private DataService _dataService;
        private Event _Event;
        private List<EventGroupsActualAttendances> groupsList;


        public page_EventList_seeEvent(Window win, DataService dataService, Event _event)
        {
            InitializeComponent();

            mainWindow = win as MainWindow;
            _dataService = dataService;
            _Event = _event;
            DataContext = _Event;

            Loaded += Page_EventList_seeEvent_Loaded;
        }

        private async void Page_EventList_seeEvent_Loaded(object sender, RoutedEventArgs e)
        {
            mainWindow.Title = StringConstants.mainWindow_Title_EventSeeStatistic;

            try
            {
                //добавление локаций
                _Event.locations.ForEach(location => stack_places.Children.Add
                (
                    LocationButtonFactory.CreateLocationButton(stack_places, location, (Style)TryFindResource("ButtonStyle_X"))
                ));

                //запрос на получение групп из ActualAttendances
                groupsList = await _dataService.apiClient.GetListOfEventGroupsStatistics(_Event.eventId);
                //вывод в DataGrid
                datagrid_groups.ItemsSource = groupsList;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void goback_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.mainframe.GoBack();
        }


        private void btn_export_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var fileName = FileNameSecurity.Build(
                    ".xlsx",
                    maxLength: 200,
                    _Event.title,
                    _Event.startDateTime.ToString("dd-MM-yyyy"),
                    _Event.eventId.ToString()
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
                        "Название мероприятия",
                        edit_event_name.Text
                    ),
                    new KeyValuePair<string, string>(
                        "Организатор",
                        edit_organizer_name.Text
                    ),
                    new KeyValuePair<string, string>(
                        "Направление",
                        edit_event_direction.Text
                    ),
                    new KeyValuePair<string, string>(
                        "Места",
                        string.Join(", ", stack_places.Children.OfType<Button>().Select(s => s.Content))
                    ),
                    new KeyValuePair<string, string>(
                        "Дата проведения",
                        edit_date.Text
                    ),
                    new KeyValuePair<string, string>(
                        "Время начала",
                        edit_startTime.Text
                    ),
                    new KeyValuePair<string, string>(
                        "Время окончания",
                        edit_endTime.Text
                    ),
                    new KeyValuePair<string, string>(
                        "Полное описание",
                        edit_fullDescription.Text
                    ),
                    new KeyValuePair<string, string>(
                        "Краткое описание",
                        edit_shortDescription.Text
                    ),
                    new KeyValuePair<string, string>(
                        "Дополнительно",
                        edit_additionalInfo.Text
                    ),
                    new KeyValuePair<string, string>(
                        "Максимальное число слушателей",
                        edit_maxListenersCount.Text
                    ),
                    new KeyValuePair<string, string>(
                        "Максимальное число участников",
                        edit_maxParticipantsCount.Text
                    ),
                    new KeyValuePair<string, string>(
                        "Внешняя организация",
                        edit_organizerOrganization.Text
                    ),
                    new KeyValuePair<string, string>(
                        "Должность организатора",
                        edit_organizerPosition.Text
                    ),
                };

                var table = new ExportTable
                {
                    Columns = new List<ExportColumn>
                    {
                        new ExportColumn
                        {
                            Header = "Группа",
                            ValueSelector = obj => ((EventGroupsActualAttendances)obj).groupName
                        },
                        new ExportColumn
                        {
                            Header = "Руководитель",
                            ValueSelector = obj => ((EventGroupsActualAttendances)obj).supervisorName
                        },
                        new ExportColumn
                        {
                            Header = "Слушатели",
                            ValueSelector = obj => ((EventGroupsActualAttendances)obj).actualListenersCount.ToString()
                        },
                        new ExportColumn
                        {
                            Header = "Участники",
                            ValueSelector = obj => ((EventGroupsActualAttendances)obj).actualParticipantsCount.ToString()
                        },
                        new ExportColumn
                        {
                            Header = "Супер-участники",
                            ValueSelector = obj => ((EventGroupsActualAttendances)obj).actualSuperParticipantsCount.ToString()
                        },
                        new ExportColumn
                        {
                            Header = "Общий балл",
                            ValueSelector = obj => ((EventGroupsActualAttendances)obj).totalScore.ToString()
                        }
                    },
                    Rows = groupsList.Cast<object>().ToList()
                };

                ExportDataService service = new ExportDataService();
                service.Export(sfd.FileName, header, table);
            }
            catch (Exception ex)
            {
                UserNotificationService.ShowError("Ошибка экспорта данных", ex, "page_EventList_seeEventAA001");
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
