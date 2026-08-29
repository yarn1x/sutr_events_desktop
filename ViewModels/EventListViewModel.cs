using college_events_desktop.DataModels;
using college_events_desktop.Model;
using college_events_desktop.View.Controls;
using college_events_desktop.View.Layers.Events;
using college_events_desktop.View.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace college_events_desktop.ViewModels
{
    public class EventListViewModel
    {
        DataService _dataService;
        page_EventList _page;
        MainWindow mainWindow;
        public EventListViewModel(MainWindow win, page_EventList page, DataService dataService)
        {
            mainWindow = win;
            _page = page;
            _dataService = dataService;
        }

        public void LoadCategoryCombobox(List<Category> categories)
        {
            var combobox_categories = _page.combobox_categories;
            //Очищаем combobox от старых направлений и заполняем их новыми
            combobox_categories.Items.Clear();
            combobox_categories.Items.Add("Не выбрано");
            foreach (var category in categories)
            {
                combobox_categories.Items.Add(category.name);
            }
            combobox_categories.SelectedValue = "Не выбрано";
        }

        public void LoadEventsInStack(List<Event> events)
        {
            var stack_events = _page.stack_events;
            //очищаем StackPanel от дочерних элементов и выводим список мероприятий в него же
            stack_events.Children.Clear();
            foreach (Event i in events)
            {
                var element = new event_element(mainWindow, _page, _dataService, i)
                {
                    Margin = new Thickness(5, 7, 5, 0),
                    HorizontalAlignment = HorizontalAlignment.Stretch
                };
                stack_events.Children.Add(element);
                
                NotifyStatusChanged();
            }
        }

        private void NotifyStatusChanged()
        {
            var hasTag1 = _page.stack_events.Children
                .OfType<event_element>()
                .Any(e => e.Tag is int tag && tag == 1);

            var hasTagMinus1 = _page.stack_events.Children
                .OfType<event_element>()
                .Any(e => e.Tag is int tag && tag == -1);

            EventNotification.NotifyStatusChanged(1, hasTag1);
            EventNotification.NotifyStatusChanged(-1, hasTagMinus1);
        }
    }


    public static class EventNotification
    {
        //событие для уведомления об изменении статуса элементов
        public static event EventHandler<EventStatusChangedEventArgs> StatusChanged;

        public static void NotifyStatusChanged(int tag, bool hasElements)
        {
            StatusChanged?.Invoke(null, new EventStatusChangedEventArgs(tag, hasElements));
        }
    }

    public class EventStatusChangedEventArgs : EventArgs
    {
        public int Tag { get; }
        public bool HasElements { get; }

        public EventStatusChangedEventArgs(int tag, bool hasElements)
        {
            Tag = tag;
            HasElements = hasElements;
        }
    }
}
