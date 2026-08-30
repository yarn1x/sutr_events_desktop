using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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

namespace college_events_desktop.View.Controls.Containers
{
    public partial class double_stack : UserControl
    {
        private ObservableCollection<UIElement> _children;

        public IList<UIElement> Children
        {
            get => _children;
            set
            {
                // Отписываемся от старой коллекции, если она была
                if (_children != null)
                    _children.CollectionChanged -= OnChildrenChanged;

                // Создаем новую коллекцию на основе переданных данных
                _children = value != null
                    ? new ObservableCollection<UIElement>(value)
                    : new ObservableCollection<UIElement>();

                // Подписываемся на изменения и принудительно перестраиваем UI
                _children.CollectionChanged += OnChildrenChanged;
                RebuildVisualTree();
            }
        }

        public double_stack()
        {
            InitializeComponent();

            // Инициализируем пустую коллекцию по умолчанию
            Children = new ObservableCollection<UIElement>();
        }

        // Срабатывает при добавлении, удалении или очистке элементов пользователем
        private void OnChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RebuildVisualTree();
        }

        // Метод распределения элементов по двум StackPanel
        private void RebuildVisualTree()
        {
            // Очищаем оба контейнера перед перераспределением
            column1.Children.Clear();
            column2.Children.Clear();

            if (_children == null) return;

            for (int i = 0; i < _children.Count; i++)
            {
                var element = _children[i];

                RemoveVisualParent(element);

                if ((i + 1) % 2 != 0)
                {
                    column1.Children.Add(element);
                }
                else
                {
                    column2.Children.Add(element);
                }
            }
        }

        // Вспомогательный метод для защиты от ошибки "Element already has a logical parent"
        private void RemoveVisualParent(UIElement element)
        {
            if (element is FrameworkElement fe && fe.Parent is Panel parent)
            {
                parent.Children.Remove(element);
            }
        }
    }
}
