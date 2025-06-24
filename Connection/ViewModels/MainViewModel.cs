using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Connection.Models;
using Connection.Services;
using Connection.Views;
using Connection.Utils;

namespace Connection.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly DataService _dataService;
        private readonly SettingsService _settingsService;
        private readonly StoryService _storyService;

        private UserData _userData;
        private bool _canContinueGame;
        private string _lastPlayTimeText;
        private Language _currentLanguage = Language.Korean;

        public MainViewModel()
        {
            _dataService = new DataService();
            _settingsService = new SettingsService(_dataService);
            _storyService = new StoryService();

            // 명령어 초기화
            NewGameCommand = new AsyncRelayCommand(StartNewGameAsync);
            ContinueGameCommand = new AsyncRelayCommand(ContinueGameAsync);
            ShowSettingsCommand = new RelayCommand(ShowSettings);
            ExitGameCommand = new RelayCommand(ExitGame);

            // 초기화
            _ = InitializeAsync();
        }

        #region Properties

        public bool CanContinueGame
        {
            get => _canContinueGame;
            set => SetProperty(ref _canContinueGame, value);
        }

        public string LastPlayTimeText
        {
            get => _lastPlayTimeText;
            set => SetProperty(ref _lastPlayTimeText, value);
        }

        // 다국어 지원 버튼 텍스트들 - 설정에 따라 동적으로 변경
        public string NewGameButtonText => LocalizationHelper.GetLocalizedString("NewGame", _currentLanguage);
        public string ContinueButtonText => LocalizationHelper.GetLocalizedString("Continue", _currentLanguage);
        public string SettingsButtonText => LocalizationHelper.GetLocalizedString("Settings", _currentLanguage);
        public string ExitButtonText => LocalizationHelper.GetLocalizedString("Exit", _currentLanguage);
        public string VersionText => "Connect Beta v1.0.2";

        #endregion

        #region Commands

        public IAsyncRelayCommand NewGameCommand { get; }
        public IAsyncRelayCommand ContinueGameCommand { get; }
        public IRelayCommand ShowSettingsCommand { get; }
        public IRelayCommand ExitGameCommand { get; }

        #endregion

        #region Methods

        private async Task InitializeAsync()
        {
            try
            {
                // 유저 데이터 로드
                _userData = await _dataService.LoadUserDataAsync();

                // 현재 언어 설정 로드
                _currentLanguage = _userData.GameSettings.Language.GameLanguage;

                // 이어하기 버튼 활성화 여부 설정
                CanContinueGame = _dataService.HasUserData() && !_userData.IsFirstTime;

                // 마지막 플레이 시간 설정
                UpdateLastPlayTimeText();

                // 설정 로드 및 적용
                await _settingsService.LoadSettingsAsync();

                // 현재 창에 설정 적용
                _settingsService.ApplySettingsToNewWindow(Application.Current.MainWindow);

                // 스토리 챕터 로드
                await _storyService.LoadChaptersAsync();

                // UI 텍스트 업데이트
                UpdateUITexts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"게임 초기화 중 오류가 발생했습니다: {ex.Message}",
                              "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 모든 UI 텍스트를 현재 언어로 업데이트
        /// </summary>
        private void UpdateUITexts()
        {
            OnPropertyChanged(nameof(NewGameButtonText));
            OnPropertyChanged(nameof(ContinueButtonText));
            OnPropertyChanged(nameof(SettingsButtonText));
            OnPropertyChanged(nameof(ExitButtonText));
        }

        /// <summary>
        /// 언어 설정이 변경되었을 때 호출
        /// </summary>
        public void OnLanguageChanged(Language newLanguage)
        {
            _currentLanguage = newLanguage;
            UpdateUITexts();
        }

        private async Task StartNewGameAsync()
        {
            try
            {
                // 기존 유저 데이터가 있는 경우 확인
                if (_dataService.HasUserData() && !_userData.IsFirstTime)
                {
                    var confirmMessage = LocalizationHelper.GetLocalizedString("ConfirmNewGame", _currentLanguage,
                        "기존 게임 데이터가 존재합니다. 정말로 새 게임을 시작하시겠습니까?\n기존 진행 상황이 모두 삭제됩니다.");
                    var confirmTitle = LocalizationHelper.GetLocalizedString("NewGame", _currentLanguage);

                    var result = MessageBox.Show(confirmMessage, confirmTitle, MessageBoxButton.YesNo, MessageBoxImage.Warning);

                    if (result != MessageBoxResult.Yes)
                        return;
                }

                // 새 게임 데이터 생성
                _userData = new UserData();
                _userData.IsFirstTime = false;
                _userData.CurrentStory = new StoryPosition { Chapter = 1, Episode = 1, ScriptIndex = 0 };

                // 현재 언어 설정 유지
                _userData.GameSettings.Language.GameLanguage = _currentLanguage;

                await _dataService.SaveUserDataAsync(_userData);

                // 게임 화면으로 전환
                await StartGameAsync();
            }
            catch (Exception ex)
            {
                var errorMessage = LocalizationHelper.GetLocalizedString("Error", _currentLanguage) + ": " + ex.Message;
                MessageBox.Show(errorMessage, LocalizationHelper.GetLocalizedString("Error", _currentLanguage),
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task ContinueGameAsync()
        {
            try
            {
                if (!CanContinueGame)
                {
                    var message = LocalizationHelper.GetLocalizedString("NoSaveData", _currentLanguage, "계속할 게임 데이터가 없습니다.");
                    MessageBox.Show(message, LocalizationHelper.GetLocalizedString("Information", _currentLanguage),
                                  MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // 게임 화면으로 전환
                await StartGameAsync();
            }
            catch (Exception ex)
            {
                var errorMessage = LocalizationHelper.GetLocalizedString("Error", _currentLanguage) + ": " + ex.Message;
                MessageBox.Show(errorMessage, LocalizationHelper.GetLocalizedString("Error", _currentLanguage),
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task StartGameAsync()
        {
            // 게임 윈도우 생성 및 표시
            var gameWindow = new GameWindow(_userData, _storyService, _settingsService, _dataService);
            gameWindow.Show();

            // 메인 윈도우 숨기기
            Application.Current.MainWindow.Hide();

            // 게임 윈도우가 닫힐 때 메인 윈도우 다시 표시
            gameWindow.Closed += async (s, e) =>
            {
                // 설정 다시 로드하여 일관성 유지
                await RefreshSettings();
                Application.Current.MainWindow.Show();
            };
        }

        /// <summary>
        /// 설정을 다시 로드하고 적용
        /// </summary>
        private async Task RefreshSettings()
        {
            try
            {
                // 유저 데이터 다시 로드
                _userData = await _dataService.LoadUserDataAsync();
                _currentLanguage = _userData.GameSettings.Language.GameLanguage;

                // 설정 다시 로드 및 적용
                await _settingsService.LoadSettingsAsync();
                _settingsService.ApplySettingsToNewWindow(Application.Current.MainWindow);

                // UI 업데이트
                CanContinueGame = _dataService.HasUserData() && !_userData.IsFirstTime;
                UpdateLastPlayTimeText();
                UpdateUITexts();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"설정 새로고침 실패: {ex.Message}");
            }
        }

        private void ShowSettings()
        {
            try
            {
                var settingsWindow = new SettingsWindow(_settingsService);
                settingsWindow.Owner = Application.Current.MainWindow;

                // 설정창이 닫힐 때 언어 변경 확인
                settingsWindow.Closed += async (s, e) =>
                {
                    await RefreshSettings();
                };

                settingsWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                var errorMessage = LocalizationHelper.GetLocalizedString("Error", _currentLanguage) + ": " + ex.Message;
                MessageBox.Show(errorMessage, LocalizationHelper.GetLocalizedString("Error", _currentLanguage),
                              MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExitGame()
        {
            var confirmMessage = LocalizationHelper.GetLocalizedString("ConfirmExit", _currentLanguage, "정말로 게임을 종료하시겠습니까?");
            var confirmTitle = LocalizationHelper.GetLocalizedString("Exit", _currentLanguage);

            var result = MessageBox.Show(confirmMessage, confirmTitle, MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
        }

        private void UpdateLastPlayTimeText()
        {
            if (_userData?.LastPlayTime != null && !_userData.IsFirstTime)
            {
                var lastPlayLabel = LocalizationHelper.GetLocalizedString("LastPlay", _currentLanguage, "마지막 플레이");
                LastPlayTimeText = $"{lastPlayLabel}: {_userData.LastPlayTime:yyyy-MM-dd HH:mm}";
            }
            else
            {
                LastPlayTimeText = "";
            }
        }

        private string GetLocalizedText(string key, string defaultText)
        {
            return LocalizationHelper.GetLocalizedString(key, _currentLanguage, defaultText);
        }

        #endregion
    }
}