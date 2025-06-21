using System.Windows;
using Connection.Services;
using Connection.ViewModels;

namespace Connection.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow(SettingsService settingsService)
        {
            InitializeComponent();
            DataContext = new SettingsViewModel(settingsService, this);
        }
    }
}