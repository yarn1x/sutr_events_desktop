using college_events_desktop.DataModels;
using System.Windows.Controls;

namespace college_events_desktop.View.Layers.Groups
{
    public partial class control_group_card : UserControl
    {
        public Group _Group => DataContext as Group;

        /// <summary>
        /// Конструктор по умолчанию. Необходим для автоматической сборки через DataTemplate.
        /// </summary>
        public control_group_card()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Старый конструктор (для обратной совместимости, если где-то остался ручной вызов)
        /// </summary>
        public control_group_card(Group group) : this()
        {
            this.DataContext = group;
        }
    }
}
