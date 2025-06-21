using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Connection.Models;
using Connection.Services;
using Connection.Views;

namespace Connection.ViewModels
{
    public class GameViewModel : ObservableObject
    {
        private readonly UserData _userData;
        private readonly StoryService _storyService;
        private readonly SettingsService _settingsService;
        private readonly DataService _dataService;
        private readonly Window _window;

        private ScriptData _currentScript;
        private int _currentScriptIndex;
        private bool _isMenuVisible;
        private bool _isAutoPlay;
        private System.Windows.Threading.DispatcherTimer _autoPlayTimer;

        public GameViewModel(UserData userData, StoryService storyService,
                           SettingsService settingsService, DataService dataService, Window window)
        {
            _currentChoices = new ObservableCollection<ChoiceViewModel>();

            _userData = userData;
            _storyService = storyService;
            _settingsService = settingsService;
            _dataService = dataService;
            _window = window;

            // 명령어 초기화
            ResumeGameCommand = new RelayCommand(ResumeGame);
            SaveGameCommand = new AsyncRelayCommand(SaveGameAsync);
            LoadGameCommand = new AsyncRelayCommand(LoadGameAsync);
            ShowSettingsCommand = new RelayCommand(ShowSettings);
            ReturnToTitleCommand = new RelayCommand(ReturnToTitle);

            // 자동재생 타이머 초기화
            _autoPlayTimer = new System.Windows.Threading.DispatcherTimer();
            _autoPlayTimer.Interval = TimeSpan.FromSeconds(2);
            _autoPlayTimer.Tick += AutoPlayTimer_Tick;

            // 게임 시작
            _ = InitializeGameAsync();
        }

        #region Properties

        private string _currentSpeaker;
        public string CurrentSpeaker
        {
            get => _currentSpeaker;
            set => SetProperty(ref _currentSpeaker, value);
        }

        private string _currentDialogue;
        public string CurrentDialogue
        {
            get => _currentDialogue;
            set => SetProperty(ref _currentDialogue, value);
        }

        private string _currentBackgroundImage;
        public string CurrentBackgroundImage
        {
            get => _currentBackgroundImage;
            set => SetProperty(ref _currentBackgroundImage, value);
        }

        private string _currentCharacterImage;
        public string CurrentCharacterImage
        {
            get => _currentCharacterImage;
            set => SetProperty(ref _currentCharacterImage, value);
        }

        public Visibility DialogueBoxVisibility =>
            _currentScript?.Scripts != null && _currentScriptIndex < _currentScript.Scripts.Count &&
            (_currentScript.Scripts[_currentScriptIndex]?.Type == ScriptType.Dialogue ||
            _currentScript.Scripts[_currentScriptIndex]?.Type == ScriptType.Narration)
            ? Visibility.Visible : Visibility.Collapsed;

        public Visibility SpeakerNameVisibility =>
            !string.IsNullOrEmpty(CurrentSpeaker) ? Visibility.Visible : Visibility.Collapsed;

        public Visibility ChoicesVisibility =>
            CurrentChoices?.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

        public Visibility GameMenuVisibility =>
            _isMenuVisible ? Visibility.Visible : Visibility.Collapsed;

        public Visibility AutoPlayVisibility =>
            _isAutoPlay ? Visibility.Visible : Visibility.Collapsed;

        public string CurrentStoryPosition =>
            $"{_userData.CurrentStory.Chapter}장 {_userData.CurrentStory.Episode}편";

        public string AutoPlayStatus => "자동재생 중...";

        private ObservableCollection<ChoiceViewModel> _currentChoices;
        public ObservableCollection<ChoiceViewModel> CurrentChoices => _currentChoices;

        #endregion

        #region Commands

        public IRelayCommand ResumeGameCommand { get; }
        public IAsyncRelayCommand SaveGameCommand { get; }
        public IAsyncRelayCommand LoadGameCommand { get; }
        public IRelayCommand ShowSettingsCommand { get; }
        public IRelayCommand ReturnToTitleCommand { get; }

        #endregion

        #region Methods

        private async Task InitializeGameAsync()
        {
            try
            {
                // 현재 위치의 스크립트 로드
                await LoadCurrentScriptAsync();

                // 첫 번째 스크립트 표시
                await DisplayCurrentScriptAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"게임 초기화 중 오류가 발생했습니다: {ex.Message}",
                              "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                _window.Close();
            }
        }

        private async Task LoadCurrentScriptAsync()
        {
            _currentScript = await _storyService.LoadScriptAsync(
                _userData.CurrentStory.Chapter,
                _userData.CurrentStory.Episode);

            _currentScriptIndex = _userData.CurrentStory.ScriptIndex;
        }

        private async Task DisplayCurrentScriptAsync()
        {
            if (_currentScript == null || _currentScriptIndex >= _currentScript.Scripts.Count)
            {
                // 현재 에피소드 완료
                await CompleteCurrentEpisodeAsync();
                return;
            }

            var currentLine = _currentScript.Scripts[_currentScriptIndex];
            var gameLanguage = _settingsService.GetCurrentSettings().Language.GameLanguage;

            // 스크립트 타입에 따른 처리
            switch (currentLine.Type)
            {
                case ScriptType.Dialogue:
                case ScriptType.Narration:
                    CurrentSpeaker = currentLine.Type == ScriptType.Dialogue ? currentLine.Speaker : "";
                    CurrentDialogue = currentLine.Text.GetValueOrDefault(gameLanguage,
                        currentLine.Text.Values.FirstOrDefault() ?? "");
                    break;

                case ScriptType.Background:
                    CurrentBackgroundImage = currentLine.BackgroundImage;
                    await NextScriptAsync(); // 자동으로 다음으로
                    return;

                case ScriptType.Character:
                    CurrentCharacterImage = currentLine.CharacterImage;
                    await NextScriptAsync(); // 자동으로 다음으로
                    return;

                case ScriptType.Choice:
                    await DisplayChoicesAsync(currentLine);
                    return;
            }

            // 배경/캐릭터 이미지 업데이트
            if (!string.IsNullOrEmpty(currentLine.BackgroundImage))
                CurrentBackgroundImage = currentLine.BackgroundImage;

            if (!string.IsNullOrEmpty(currentLine.CharacterImage))
                CurrentCharacterImage = currentLine.CharacterImage;

            // UI 업데이트 알림
            OnPropertyChanged(nameof(DialogueBoxVisibility));
            OnPropertyChanged(nameof(SpeakerNameVisibility));
            OnPropertyChanged(nameof(ChoicesVisibility));
        }

        private async Task DisplayChoicesAsync(ScriptLine choiceLine)
        {
            _currentChoices.Clear();
            var gameLanguage = _settingsService.GetCurrentSettings().Language.GameLanguage;

            foreach (var choice in choiceLine.Choices)
            {
                var choiceText = choice.Text.GetValueOrDefault(gameLanguage,
                    choice.Text.Values.FirstOrDefault() ?? "");

                _currentChoices.Add(new ChoiceViewModel
                {
                    Text = choiceText,
                    SelectCommand = new AsyncRelayCommand(() => SelectChoiceAsync(choice.NextScriptIndex))
                });
            }

            OnPropertyChanged(nameof(ChoicesVisibility));
        }

        private async Task SelectChoiceAsync(int nextScriptIndex)
        {
            _currentChoices.Clear();
            OnPropertyChanged(nameof(ChoicesVisibility));

            _currentScriptIndex = nextScriptIndex;
            _userData.CurrentStory.ScriptIndex = _currentScriptIndex;

            await DisplayCurrentScriptAsync();
        }

        private async Task NextScriptAsync()
        {
            _currentScriptIndex++;
            _userData.CurrentStory.ScriptIndex = _currentScriptIndex;

            // 자동 저장
            await _dataService.SaveUserDataAsync(_userData);

            await DisplayCurrentScriptAsync();
        }

        private async Task CompleteCurrentEpisodeAsync()
        {
            // 현재 에피소드 완료 처리
            _storyService.UpdateProgress(_userData,
                _userData.CurrentStory.Chapter,
                _userData.CurrentStory.Episode,
                true);

            // 다음 에피소드로 진행 가능한지 확인
            if (_storyService.CanProgressToNext(_userData.CurrentStory))
            {
                var result = MessageBox.Show(
                    $"{_userData.CurrentStory}가 완료되었습니다. 다음 에피소드로 진행하시겠습니까?",
                    "에피소드 완료",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Information);

                if (result == MessageBoxResult.Yes)
                {
                    // 다음 에피소드로 진행
                    _userData.CurrentStory.Episode++;
                    _userData.CurrentStory.ScriptIndex = 0;
                    await LoadCurrentScriptAsync();
                    await DisplayCurrentScriptAsync();
                    return;
                }
            }

            // 타이틀로 돌아가기
            ReturnToTitle();
        }

        public async Task HandleKeyInputAsync(Key key)
        {
            var settings = _settingsService.GetCurrentSettings();
            var keyBindings = settings.Controls.KeyBindings;

            var keyString = key.ToString();

            if (keyBindings.ContainsValue(keyString))
            {
                var action = keyBindings.FirstOrDefault(x => x.Value == keyString).Key;

                switch (action)
                {
                    case GameAction.NextScript:
                        if (!_isMenuVisible && _currentChoices.Count == 0)
                            await NextScriptAsync();
                        break;

                    case GameAction.ShowMenu:
                        ToggleMenu();
                        break;

                    case GameAction.Auto:
                        ToggleAutoPlay();
                        break;

                    case GameAction.FastForward:
                        // 빨리감기 구현
                        break;
                }
            }
        }

        private void ToggleMenu()
        {
            _isMenuVisible = !_isMenuVisible;
            OnPropertyChanged(nameof(GameMenuVisibility));
        }

        private void ToggleAutoPlay()
        {
            _isAutoPlay = !_isAutoPlay;

            if (_isAutoPlay)
                _autoPlayTimer.Start();
            else
                _autoPlayTimer.Stop();

            OnPropertyChanged(nameof(AutoPlayVisibility));
        }

        private async void AutoPlayTimer_Tick(object sender, EventArgs e)
        {
            if (!_isMenuVisible && _currentChoices.Count == 0)
            {
                await NextScriptAsync();
            }
        }

        private void ResumeGame()
        {
            _isMenuVisible = false;
            OnPropertyChanged(nameof(GameMenuVisibility));
        }

        private async Task SaveGameAsync()
        {
            try
            {
                await _dataService.SaveUserDataAsync(_userData);
                MessageBox.Show("게임이 저장되었습니다.", "저장 완료",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"저장 중 오류가 발생했습니다: {ex.Message}",
                              "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadGameAsync()
        {
            try
            {
                var newUserData = await _dataService.LoadUserDataAsync();

                // 유저 데이터 업데이트
                _userData.CurrentStory = newUserData.CurrentStory;
                _userData.CompletedStories = newUserData.CompletedStories;

                await LoadCurrentScriptAsync();
                await DisplayCurrentScriptAsync();

                MessageBox.Show("게임을 불러왔습니다.", "불러오기 완료",
                              MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"불러오기 중 오류가 발생했습니다: {ex.Message}",
                              "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ShowSettings()
        {
            var settingsWindow = new SettingsWindow(_settingsService);
            settingsWindow.Owner = _window;
            settingsWindow.ShowDialog();

            // 설정 변경 후 현재 게임 창에 설정 적용
            _settingsService.ApplySettingsToNewWindow(_window);
        }

        private void ReturnToTitle()
        {
            var result = MessageBox.Show("타이틀로 돌아가시겠습니까? 저장되지 않은 진행상황은 사라집니다.",
                                       "타이틀로 돌아가기",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                _window.Close();
            }
        }

        #endregion
    }

    public class ChoiceViewModel
    {
        public string Text { get; set; }
        public IAsyncRelayCommand SelectCommand { get; set; }
    }
}