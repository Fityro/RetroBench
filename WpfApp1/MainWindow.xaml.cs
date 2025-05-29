using Aida64Clone.ViewModels;
using System.Windows;

namespace Aida64Clone.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.SelectedNode = e.NewValue as TreeViewItemViewModel;
            }
        }

        protected override void OnClosed(System.EventArgs e)
        {
            // Останавливаем мониторинг при закрытии окна
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.StopMonitoring();
            }
            base.OnClosed(e);
        }
    }
}