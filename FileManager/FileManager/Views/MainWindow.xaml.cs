using System.Windows;
using System.Windows.Controls;
using FileManager.Models;
using FileManager.ViewModels;

namespace FileManager.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is MainViewModel viewModel && e.NewValue is FileSystemObjectModel selectedItem)
            {
                viewModel.SelectedObject = selectedItem;
                viewModel.SelectItemCommand.Execute(null);
            }
        }
        private void TreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            var treeViewItem = e.OriginalSource as TreeViewItem;
            if (treeViewItem?.DataContext is FileSystemObjectModel item)
            {
                var viewModel = DataContext as MainViewModel;
                if (item.Children.Count == 0)
                {
                    viewModel?.LoadChildrenCommand.Execute(item);
                }
            }
        }

        private void TreeViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if(DataContext is MainViewModel viewModel)
            {
                viewModel.MouseDoubleClickCommand.Execute(null);
            }
        }
    }
}
