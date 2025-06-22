using System.Windows;
using System.Windows.Input;
using Connection.Models;
using Connection.Services;
using Connection.ViewModels;

namespace Connection.Views
{
    public partial class GameWindow : Window
    {
        private GameViewModel _viewModel;

        public GameWindow(UserData userData, StoryService storyService,
                         SettingsService settingsService, DataService dataService)
        {
            InitializeComponent();

            _viewModel = new GameViewModel(userData, storyService, settingsService, dataService, this);
            DataContext = _viewModel;

            // 설정 적용
            settingsService.ApplySettingsToNewWindow(this);

            // 포커스 설정
            Loaded += (s, e) => Focus();
        }

        private async void Window_KeyDown(object sender, KeyEventArgs e)
        {
            await _viewModel.HandleKeyInputAsync(e.Key);
        }

        private async void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 포커스 설정
            Focus();
        }

        private async void ClickArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // 대화 진행
            await _viewModel.AdvanceTextCommand.ExecuteAsync(null);
        }
    }
}