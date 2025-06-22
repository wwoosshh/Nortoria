using System.Windows;
using System.Windows.Input;
using Connection.Models;
using Connection.ViewModels;

namespace Connection.Views
{
    public partial class InventoryWindow : Window
    {
        private InventoryViewModel _viewModel;

        public InventoryWindow(UserData userData)
        {
            InitializeComponent();
            _viewModel = new InventoryViewModel(userData);
            DataContext = _viewModel;
        }

        private void ItemSlot_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.Tag is InventoryItemViewModel item)
            {
                _viewModel.SelectedItem = item;
            }
        }

        private void UseItem_Click(object sender, RoutedEventArgs e)
        {
            if (_viewModel.SelectedItem != null)
            {
                _viewModel.UseItem(_viewModel.SelectedItem.Id);
            }
        }

        private void SortItems_Click(object sender, RoutedEventArgs e)
        {
            _viewModel.SortItems();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}