using college_events_desktop.DataModels;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace college_events_desktop.View.Layers.Tables
{
    public partial class table_tuple_EventGroup_edit : UserControl
    {
        #region Поля класса
        page_EventList_edit _page;
        public EventGroups _group {  get; private set; }
        page_table_EventGroup_edit _table;
        #endregion


        #region Конструктор
        public table_tuple_EventGroup_edit(page_EventList_edit page, EventGroups group, object table)
        {
            InitializeComponent();
            _page = page;
            _group = group;
            _table = table as page_table_EventGroup_edit;
            Loaded += Table_tuple_EventGroup_edit_Loaded;
        }

        #endregion


        #region Обработчики событий

        private void combobox_group_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Если группы ещё не было выбрано на этом элементе, создаём новый элемент с пустой группой
            if (text_supervisor_name.Text == "-")
            {
                edit_expectedListenersCount.Text = "0";
                edit_expectedParticipantsCount.Text = "0";
                edit_expectedSuperParticipantsCount.Text = "0";

                _table.stack_table_rows.Children.Add(new table_tuple_EventGroup_edit(_page, null, _table));
            }

            if (combobox_group.SelectedItem == null) return;
            
            //определяем выбранную группу и находим под эту группу куратора
            string selectedGroup = combobox_group.SelectedItem.ToString();
            text_supervisor_name.Text = GetSupervisor(selectedGroup);
            
            //изменяем тэг, внутри которого находится Id группы (это нужно для отправки запроса на сохранение)
            //тэг этого элемента используется для чтения в обработчике события клика кнопки "Сохранить"
            _group = new EventGroups()
            {
                groupId = _table._groups.Where(n => n.groupName == selectedGroup).FirstOrDefault().groupId,
            };
            Tag = _group.groupId;
            text_score.Text = Tag.ToString();
        }


        private void Table_tuple_EventGroup_edit_Loaded(object sender, RoutedEventArgs e)
        {
            if (_group != null)
            {
                Tag = _group.groupId;
                combobox_group.Text = _group.name;
                text_supervisor_name.Text = $"{_group.supervisorSurname} {_group.supervisorName} {_group.supervisorLastname}";
                edit_expectedListenersCount.Text = _group.expectedListenersCount.ToString();
                edit_expectedParticipantsCount.Text = _group.expectedParticipantsCount.ToString();
                edit_expectedSuperParticipantsCount.Text = _group.expectedSuperParticipantsCount.ToString();
                text_score.Text = Tag.ToString();
            }
            LoadGroups();
        }


        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation()
            {
                From = btn_delete_group.Width,
                To = 20,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseOut }
            };
            btn_delete_group.BeginAnimation(WidthProperty, anim);
        }



        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            DoubleAnimation anim = new DoubleAnimation()
            {
                From = btn_delete_group.Width,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase { EasingMode = EasingMode.EaseIn }
            };
            btn_delete_group.BeginAnimation(WidthProperty, anim);
        }



        /// <summary>
        /// метод удаляет строку из таблицы
        /// </summary>
        private void btn_delete_group_Click(object sender, RoutedEventArgs e)
        {
            if (text_supervisor_name.Text == "-")
            {
                MessageBox.Show("Это строка для добавления новой группы. Её нельзя удалить.");
                return;
            }
            var callback = MessageBox.Show(
                $"Хотите удалить группу {combobox_group.Text} из списка?",
                "Удаление",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question,
                MessageBoxResult.No);

            if (callback == MessageBoxResult.Yes)
            {
                _table.stack_table_rows.Children.Remove(this);
            }
        }



        /// <summary>
        /// метод определяет валидны ли данные в ячейке
        /// </summary>
        private void edit_expectedCount_Error(object sender, ValidationErrorEventArgs e)
        {
            //если ошибка валидации данных (допустим отрицательное число или символы недопустимые), то добавляем к счётчику ошибок +1
            if (e.Action == ValidationErrorEventAction.Added)
            {
                _page.validation_errors_count++;
            }
            //если ошибку исправили, убираем со счётчика -1
            else if (e.Action == ValidationErrorEventAction.Removed)
            {
                _page.validation_errors_count--;
            }
            //проверка валидности происходит в классе EventGroups (там все условия валидности)
        }
        
        #endregion


        #region Методы класса

        /// <summary>
        /// метод служит для того, что бы в новый комбобокс с добавить группы, но только те, которые не были добавлены ранее в другие комбобоксы
        /// </summary>
        private void LoadGroups()
        {
            //.OfType<...>() — выбирает из списка элементов только нужные строки таблицы
            //.Any(...) — проверяет, занято ли уже имя группы в созданных комбобоксах
            //.Where(...) — фильтрует и оставляет только свободные группы
            //.ForEach(...) — добавляет каждую оставшуюся группу в ваш новый комбобокс.
            _table._groups
                .Where(g => 
                    !_table.stack_table_rows.Children.OfType<table_tuple_EventGroup_edit>()
                    .Any(t => t.combobox_group.Text == g.groupName)
                )
                .ToList()
                .ForEach(g => combobox_group.Items.Add(g.groupName));
        }

        /// <summary>
        /// поиск куратора по названию группы
        /// </summary>
        /// <param name="groupName">название группы</param>
        /// <returns>ФИО куратора</returns>
        public string GetSupervisor(string groupName)
        {
            if (string.IsNullOrEmpty(groupName)) return "Куратор не указан";

            return _table.GroupSupervisor.TryGetValue(groupName, out string supervisor)
                ? supervisor
                : "Куратор не назначен";
        }
        #endregion
    }
}
