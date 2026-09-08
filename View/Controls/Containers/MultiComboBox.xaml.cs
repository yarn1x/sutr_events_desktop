using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace college_events_desktop.View.Controls.Containers
{
    /// <summary>
    /// Пользовательский элемент управления WPF для выбора нескольких элементов из списка.
    /// </summary>
    /// <remarks>
    /// <see cref="Items"/> - возвращает или задаёт коллекцию для общего списка в <see cref="ComboBox"/>
    /// <see cref="SelectedItems"/> - возвращает или задаёт коллекцию выбранных элементов из списка <see cref="ComboBox"/>
    /// <see cref="DisplayMemberPath"/> - параметр, указывающий из какого поля будут браться данные для отображения в списке.
    /// </remarks>
    public partial class MultiComboBox : UserControl
    {
        private ObservableCollection<SelectionItem> _selectedWrappers = new ObservableCollection<SelectionItem>();

        public MultiComboBox()
        {
            InitializeComponent();
            SelectedItemsControl.ItemsSource = _selectedWrappers;
        }

        #region Dependency Properties

        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register(nameof(Items), typeof(IEnumerable), typeof(MultiComboBox),
                new PropertyMetadata(null, OnItemsChanged));

        /// <summary>
        /// Возвращает или задаёт коллекцию для общего списка в <see cref="ComboBox"/>
        /// </summary>
        public IEnumerable Items
        {
            get => (IEnumerable)GetValue(ItemsProperty);
            set => SetValue(ItemsProperty, value);
        }





        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register(nameof(SelectedItems), typeof(IEnumerable), typeof(MultiComboBox),
                new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectedItemsChanged));

        /// <summary>
        /// Возвращает или задаёт коллекцию выбранных элементов из списка <see cref="ComboBox"/>
        /// </summary>
        public IEnumerable SelectedItems
        {
            get => (IEnumerable)GetValue(SelectedItemsProperty);
            set => SetValue(SelectedItemsProperty, value);
        }



        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(nameof(DisplayMemberPath), typeof(string), typeof(MultiComboBox),
                new PropertyMetadata(string.Empty, OnDisplayMemberPathChanged));
        
        /// <summary>
        /// Параметр, указывающий из какого поля будут браться данные для отображения в списке.
        /// </summary>
        /// Можно задать как в code-behind как поле класса, но так же, проще через параметр в вёрстке xaml
        public string DisplayMemberPath
        {
            get => (string)GetValue(DisplayMemberPathProperty);
            set => SetValue(DisplayMemberPathProperty, value);
        }
        private static void OnDisplayMemberPathChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MultiComboBox control)
            {
                control.UpdateSelectedWidgets();
            }
        }

        #endregion

        #region Property Changed Callbacks

        private static void OnItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MultiComboBox control)
            {
                control.UpdateComboBoxItems();
            }
        }

        private static void OnSelectedItemsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is MultiComboBox control)
            {
                control.UpdateSelectedWidgets();
            }
        }

        #endregion

        //обновление списка в самом ComboBox, исключая уже выбранные элементы
        private void UpdateComboBoxItems()
        {
            if (Items == null)
            {
                MainComboBox.ItemsSource = null;
                return;
            }

            var selectedList = SelectedItems?.Cast<object>().ToList() ?? new List<object>();

            //фильтруем Items. Убираем те, что уже выбраны
            var availableItems = Items.Cast<object>()
                                      .Where(item => !selectedList.Contains(item))
                                      .ToList();

            MainComboBox.ItemsSource = availableItems;
        }

        //синхронизация визуальных кнопок с коллекцией SelectedItems
        private void UpdateSelectedWidgets()
        {
            _selectedWrappers.Clear();
            if (SelectedItems != null)
            {
                foreach (var item in SelectedItems)
                {
                    _selectedWrappers.Add(new SelectionItem
                    {
                        Value = item,
                        DisplayText = GetDisplayText(item)
                    });
                }
            }
            UpdateComboBoxItems();
        }

        //вспомогательный метод для получения текста через DisplayMemberPath
        private string GetDisplayText(object item)
        {
            if (item == null) return string.Empty;

            //если в метод случайно прилетела сама обертка SelectionItem, 
            //берем её внутреннее значение Value
            if (item is SelectionItem wrapper)
            {
                item = wrapper.Value;
                if (item == null) return string.Empty;
            }

            //если путь к свойству не задан (DisplayMemberPath=""), используем ToString()
            if (string.IsNullOrEmpty(DisplayMemberPath))
                return item.ToString();

            var itemType = item.GetType();

            //находим свойство с игнорированием регистра букв
            var property = itemType.GetProperty(DisplayMemberPath,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.IgnoreCase);

            if (property != null)
            {
                var val = property.GetValue(item, null);
                return val?.ToString() ?? string.Empty;
            }

            //на всякий случай проверяем и обычные поля (fields)
            var field = itemType.GetField(DisplayMemberPath,
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.IgnoreCase);

            if (field != null)
            {
                return field.GetValue(item)?.ToString() ?? string.Empty;
            }

            //если рефлексия не нашла такое свойство у объекта, возвращаем ToString()
            return item.ToString();
        }


        //логика выбора элемента в ComboBox
        private void MainComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainComboBox.SelectedItem == null) return;

            var selectedItem = MainComboBox.SelectedItem;

            //инициализируем коллекцию, если она null
            if (SelectedItems == null)
            {
                //пытаемся создать типизированный List<T> на основе типа элементов в Items
                Type itemType = Items?.Cast<object>().FirstOrDefault()?.GetType() ?? typeof(object);
                Type listType = typeof(List<>).MakeGenericType(itemType);
                SelectedItems = (IEnumerable)Activator.CreateInstance(listType);
            }

            //добавляем элемент в существующую коллекцию SelectedItems
            var currentSelected = SelectedItems.Cast<object>().ToList();
            if (!currentSelected.Contains(selectedItem))
            {
                currentSelected.Add(selectedItem);

                //пересоздаем коллекцию нужного типа для отправки обратно через DP
                Type itemType = selectedItem.GetType();
                var newList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));
                foreach (var obj in currentSelected) newList.Add(obj);

                SelectedItems = newList;
            }

            MainComboBox.SelectedIndex = -1;
        }

        //клик по кнопке (удаление элемента из выбранных)
        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag != null)
            {

                var itemToRemove = button.Tag;
                var currentSelected = SelectedItems?.Cast<object>().ToList() ?? new List<object>();

                if (currentSelected.Contains(itemToRemove))
                {
                    currentSelected.Remove(itemToRemove);

                    Type itemType = itemToRemove.GetType();
                    var newList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(itemType));
                    foreach (var obj in currentSelected)
                    {
                        newList.Add(obj);
                    }

                    //обновление визуальной части
                    SelectedItems = newList; 
                }
            }
        }

        /// <summary>
        /// Вспомогательный класс-обертка для кнопок.
        /// </summary>
        /// <remarks>
        /// <see cref="Value"/> - объект хранящий всю метадату.
        /// <see cref="DisplayText"/> - текстовое представление объекта.
        /// </remarks>
        private class SelectionItem
        {
            public object Value { get; set; }
            public string DisplayText { get; set; }
        }
    }
}
