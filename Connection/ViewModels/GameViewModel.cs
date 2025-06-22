using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Connection.Models;
using Connection.Services;
using Connection.Views;
using Connection.Utils;

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
        private DispatcherTimer _autoPlayTimer;
        private DispatcherTimer _typingTimer;

        private string _fullDialogueText = "";
        private StringBuilder _typingTextBuilder = new StringBuilder();
        private int _typingIndex = 0;
        private bool _isTyping = false;
        private bool _canAdvance = true;

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
            AdvanceTextCommand = new AsyncRelayCommand(AdvanceTextAsync);

            // 타이머 초기화
            _autoPlayTimer = new DispatcherTimer();
            _autoPlayTimer.Interval = TimeSpan.FromSeconds(3);
            _autoPlayTimer.Tick += AutoPlayTimer_Tick;

            _typingTimer = new DispatcherTimer(DispatcherPriority.Render);
            _typingTimer.Interval = TimeSpan.FromMilliseconds(60); // 약 60FPS로 부드러운 타이핑
            _typingTimer.Tick += TypingTimer_Tick;

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
            $"{_userData.CurrentStory.Chapter}장 {_userData.CurrentStory.Episode}막";

        public string AutoPlayStatus =>
            LocalizationHelper.GetLocalizedString("AutoPlayMode", _settingsService.GetCurrentSettings().Language.GameLanguage);

        public string ContinuePrompt =>
            LocalizationHelper.GetLocalizedString("ClickToContinue", _settingsService.GetCurrentSettings().Language.GameLanguage);

        public Visibility ContinuePromptVisibility =>
            !_isTyping && DialogueBoxVisibility == Visibility.Visible && !_isAutoPlay ? Visibility.Visible : Visibility.Collapsed;

        private ObservableCollection<ChoiceViewModel> _currentChoices;
        public ObservableCollection<ChoiceViewModel> CurrentChoices => _currentChoices;

        #endregion

        /// <summary>
        /// 스크립트 효과를 적용합니다
        /// </summary>
        private void ApplyScriptEffects(List<ScriptEffect> effects)
        {
            if (effects == null || effects.Count == 0) return;

            foreach (var effect in effects)
            {
                ApplySingleEffect(effect);
            }
        }

        /// <summary>
        /// 단일 효과를 적용합니다
        /// </summary>
        private void ApplySingleEffect(ScriptEffect effect)
        {
            var gameLanguage = _settingsService.GetCurrentSettings().Language.GameLanguage;
            string message = "";

            switch (effect.Type.ToLower())
            {
                case "item":
                    if (effect.Action.ToLower() == "give")
                    {
                        _userData.Inventory.AddItem(effect.Target, effect.Amount);
                        if (!effect.Silent)
                        {
                            message = effect.Message.GetValueOrDefault(gameLanguage,
                                $"{effect.Target} x{effect.Amount}을(를) 획득했습니다!");
                        }
                    }
                    else if (effect.Action.ToLower() == "take")
                    {
                        if (_userData.Inventory.HasItem(effect.Target, effect.Amount))
                        {
                            // 아이템 제거 로직 추가 필요
                            RemoveItem(effect.Target, effect.Amount);
                            if (!effect.Silent)
                            {
                                message = effect.Message.GetValueOrDefault(gameLanguage,
                                    $"{effect.Target} x{effect.Amount}을(를) 잃었습니다.");
                            }
                        }
                        else
                        {
                            message = "필요한 아이템이 부족합니다.";
                        }
                    }
                    break;

                case "currency":
                    if (effect.Action.ToLower() == "give")
                    {
                        _userData.Inventory.Currency += effect.Amount;
                        if (!effect.Silent)
                        {
                            message = effect.Message.GetValueOrDefault(gameLanguage,
                                $"{effect.Amount} 골드를 획득했습니다!");
                        }
                    }
                    else if (effect.Action.ToLower() == "take")
                    {
                        if (_userData.Inventory.Currency >= effect.Amount)
                        {
                            _userData.Inventory.Currency -= effect.Amount;
                            if (!effect.Silent)
                            {
                                message = effect.Message.GetValueOrDefault(gameLanguage,
                                    $"{effect.Amount} 골드를 잃었습니다.");
                            }
                        }
                        else
                        {
                            message = "골드가 부족합니다.";
                        }
                    }
                    break;

                case "flag":
                    // 스토리 플래그 설정 (추후 구현)
                    if (!effect.Silent)
                    {
                        message = effect.Message.GetValueOrDefault(gameLanguage, "");
                    }
                    break;
            }

            // 메시지 표시
            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show(message, "알림", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        /// <summary>
        /// 아이템을 제거합니다
        /// </summary>
        private void RemoveItem(string itemId, int amount)
        {
            if (_userData.Inventory.Items.ContainsKey(itemId))
            {
                _userData.Inventory.Items[itemId] -= amount;
                if (_userData.Inventory.Items[itemId] <= 0)
                {
                    _userData.Inventory.Items.Remove(itemId);
                }
            }
        }

        /// <summary>
        /// 아이템을 획득합니다
        /// </summary>
        public void GiveItem(string itemId, int quantity = 1)
        {
            _userData.Inventory.AddItem(itemId, quantity);

            // UI에 알림 표시 (간단하게)
            MessageBox.Show($"아이템을 획득했습니다: {itemId} x{quantity}",
                            "아이템 획득", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        /// <summary>
        /// 인벤토리를 확인합니다  
        /// </summary>
        public void ShowInventory()
        {
            var inventoryWindow = new InventoryWindow(_userData);
            inventoryWindow.Owner = _window;
            inventoryWindow.ShowDialog();
        }

        #region Commands

        public IRelayCommand ResumeGameCommand { get; }
        public IAsyncRelayCommand SaveGameCommand { get; }
        public IAsyncRelayCommand LoadGameCommand { get; }
        public IRelayCommand ShowSettingsCommand { get; }
        public IRelayCommand ReturnToTitleCommand { get; }
        public IAsyncRelayCommand AdvanceTextCommand { get; }

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
            // 조건을 만족하는 다음 스크립트 찾기
            while (_currentScriptIndex < _currentScript.Scripts.Count)
            {
                var currentLine = _currentScript.Scripts[_currentScriptIndex];

                // 조건 체크: 조건이 없거나 모든 조건을 만족하면 실행
                if (CheckScriptConditions(currentLine.Conditions))
                {
                    await ExecuteCurrentScriptAsync(currentLine);
                    return;
                }
                else
                {
                    // 조건을 만족하지 않으면 다음 스크립트로 스킵
                    _currentScriptIndex++;
                    _userData.CurrentStory.ScriptIndex = _currentScriptIndex;
                }
            }

            // 모든 스크립트를 확인했지만 조건을 만족하는 것이 없으면 에피소드 완료
            await CompleteCurrentEpisodeAsync();
        }
        /// <summary>
        /// 현재 스크립트를 실행합니다
        /// </summary>
        private async Task ExecuteCurrentScriptAsync(ScriptLine currentLine)
        {
            var gameLanguage = _settingsService.GetCurrentSettings().Language.GameLanguage;

            // 스크립트 타입에 따른 처리
            switch (currentLine.Type)
            {
                case ScriptType.Dialogue:
                case ScriptType.Narration:
                    if (currentLine.Type == ScriptType.Dialogue && !string.IsNullOrEmpty(currentLine.Speaker))
                    {
                        CurrentSpeaker = LocalizationHelper.GetCharacterName(currentLine.Speaker, gameLanguage);
                    }
                    else
                    {
                        CurrentSpeaker = "";
                    }

                    _fullDialogueText = currentLine.Text.GetValueOrDefault(gameLanguage,
                        currentLine.Text.Values.FirstOrDefault() ?? "");

                    StartTypingAnimation();

                    // 스크립트 효과 적용
                    ApplyScriptEffects(currentLine.Effects);
                    break;

                case ScriptType.Background:
                    if (!string.IsNullOrEmpty(currentLine.BackgroundImage))
                    {
                        CurrentBackgroundImage = GetImagePath(currentLine.BackgroundImage);
                    }
                    ApplyScriptEffects(currentLine.Effects);
                    await NextScriptAsync();
                    return;

                case ScriptType.Character:
                    if (!string.IsNullOrEmpty(currentLine.CharacterImage))
                    {
                        CurrentCharacterImage = GetImagePath(currentLine.CharacterImage);
                    }
                    ApplyScriptEffects(currentLine.Effects);
                    await NextScriptAsync();
                    return;

                case ScriptType.Choice:
                    await DisplayChoicesAsync(currentLine);
                    return;
            }

            // 배경/캐릭터 이미지 업데이트
            if (!string.IsNullOrEmpty(currentLine.BackgroundImage))
                CurrentBackgroundImage = GetImagePath(currentLine.BackgroundImage);

            if (!string.IsNullOrEmpty(currentLine.CharacterImage))
                CurrentCharacterImage = GetImagePath(currentLine.CharacterImage);

            // UI 업데이트 알림
            OnPropertyChanged(nameof(DialogueBoxVisibility));
            OnPropertyChanged(nameof(SpeakerNameVisibility));
            OnPropertyChanged(nameof(ChoicesVisibility));
            OnPropertyChanged(nameof(ContinuePromptVisibility));
        }

        /// <summary>
        /// 스크립트 조건을 확인합니다
        /// </summary>
        private bool CheckScriptConditions(List<ScriptCondition> conditions)
        {
            if (conditions == null || conditions.Count == 0)
                return true; // 조건이 없으면 항상 실행

            // 모든 조건을 만족해야 함
            foreach (var condition in conditions)
            {
                if (!CheckSingleCondition(condition))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 단일 조건을 확인합니다
        /// </summary>
        private bool CheckSingleCondition(ScriptCondition condition)
        {
            switch (condition.Type.ToLower())
            {
                case "flag":
                    int flagValue = _userData.GetFlag(condition.Target);
                    if (condition.Operator.ToLower() == "equals")
                    {
                        return flagValue == int.Parse(condition.Value);
                    }
                    else if (condition.Operator.ToLower() == "true")
                    {
                        return flagValue > 0;
                    }
                    else if (condition.Operator.ToLower() == "false")
                    {
                        return flagValue == 0;
                    }
                    break;

                case "item":
                    int itemCount = _userData.Inventory.GetItemCount(condition.Target);
                    int requiredCount = int.Parse(condition.Value);
                    return itemCount >= requiredCount;

                case "relationship":
                    int relationshipValue = _userData.GetRelationship(condition.Target);
                    int requiredRelationship = int.Parse(condition.Value);

                    return condition.Operator.ToLower() switch
                    {
                        "greater" => relationshipValue > requiredRelationship,
                        "less" => relationshipValue < requiredRelationship,
                        "equals" => relationshipValue == requiredRelationship,
                        _ => true
                    };

                default:
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 타이핑 애니메이션을 시작합니다
        /// </summary>
        private void StartTypingAnimation()
        {
            _isTyping = true;
            _typingIndex = 0;
            _typingTextBuilder.Clear();
            CurrentDialogue = "";
            _canAdvance = false;

            OnPropertyChanged(nameof(ContinuePromptVisibility));
            _typingTimer.Start();
        }

        /// <summary>
        /// 타이핑 애니메이션을 완료합니다
        /// </summary>
        private void CompleteTypingAnimation()
        {
            _typingTimer.Stop();
            _isTyping = false;
            _canAdvance = true;
            _typingTextBuilder.Clear();
            _typingTextBuilder.Append(_fullDialogueText);
            CurrentDialogue = _fullDialogueText;
            OnPropertyChanged(nameof(ContinuePromptVisibility));

            // 자동재생 모드인 경우 타이머 시작
            if (_isAutoPlay)
            {
                _autoPlayTimer.Start();
            }
        }

        /// <summary>
        /// 타이핑 타이머 이벤트
        /// </summary>
        private void TypingTimer_Tick(object sender, EventArgs e)
        {
            if (_typingIndex < _fullDialogueText.Length)
            {
                // 한 번에 1-2글자씩 처리하여 더 부드럽게
                int charactersToAdd = Math.Min(2, _fullDialogueText.Length - _typingIndex);

                for (int i = 0; i < charactersToAdd; i++)
                {
                    if (_typingIndex < _fullDialogueText.Length)
                    {
                        _typingTextBuilder.Append(_fullDialogueText[_typingIndex]);
                        _typingIndex++;
                    }
                }

                CurrentDialogue = _typingTextBuilder.ToString();
            }
            else
            {
                CompleteTypingAnimation();
            }
        }

        /// <summary>
        /// 텍스트 진행 처리
        /// </summary>
        private async Task AdvanceTextAsync()
        {
            if (!_canAdvance && !_isTyping) return;

            // 타이핑 중이면 즉시 완료
            if (_isTyping)
            {
                CompleteTypingAnimation();
                return;
            }

            // 메뉴가 열려있거나 선택지가 있으면 진행하지 않음
            if (_isMenuVisible || _currentChoices.Count > 0) return;

            await NextScriptAsync();
        }

        /// <summary>
        /// 이미지 경로를 전체 경로로 변환합니다
        /// </summary>
        private string GetImagePath(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return null;

            // 이미 절대 경로인 경우
            if (Path.IsPathRooted(relativePath))
                return relativePath;

            // 상대 경로를 절대 경로로 변환
            var fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", relativePath);

            // 파일이 존재하는지 확인
            if (File.Exists(fullPath))
                return fullPath;

            // 파일이 없는 경우 기본 이미지 또는 null 반환
            Console.WriteLine($"이미지 파일을 찾을 수 없습니다: {fullPath}");
            return null;
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
            // 선택한 choice 찾기
            var currentLine = _currentScript.Scripts[_currentScriptIndex];
            var selectedChoice = currentLine.Choices.FirstOrDefault(c => c.NextScriptIndex == nextScriptIndex);

            // 선택지 효과 적용
            if (selectedChoice?.Effects != null)
            {
                ApplyScriptEffects(selectedChoice.Effects);
            }

            _currentChoices.Clear();
            OnPropertyChanged(nameof(ChoicesVisibility));

            _currentScriptIndex = nextScriptIndex;
            _userData.CurrentStory.ScriptIndex = _currentScriptIndex;

            await _dataService.SaveUserDataAsync(_userData);
            await DisplayCurrentScriptAsync();
        }

        private async Task NextScriptAsync()
        {
            // 자동재생 타이머 정지
            _autoPlayTimer.Stop();

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

            var currentChapter = _userData.CurrentStory.Chapter;
            var currentEpisode = _userData.CurrentStory.Episode;

            MessageBox.Show(
                $"{currentChapter}장 {currentEpisode}막이 완료되었습니다!",
                "에피소드 완료",
                MessageBoxButton.OK,
                MessageBoxImage.Information);

            // 다음 에피소드 확인
            var chapters = await _storyService.LoadChaptersAsync();
            var chapter = chapters.FirstOrDefault(c => c.ChapterNumber == currentChapter);

            if (chapter != null)
            {
                // 같은 장의 다음 막이 있는지 확인
                var nextEpisode = chapter.Episodes.FirstOrDefault(e => e.EpisodeNumber == currentEpisode + 1);

                if (nextEpisode != null)
                {
                    var result = MessageBox.Show(
                        $"다음 막으로 진행하시겠습니까?\n({currentChapter}장 {currentEpisode + 1}막: {nextEpisode.Title})",
                        "다음 막으로",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        // 다음 막으로 진행
                        _userData.CurrentStory.Episode++;
                        _userData.CurrentStory.ScriptIndex = 0;

                        // 다음 막 잠금 해제
                        nextEpisode.IsUnlocked = true;

                        await _dataService.SaveUserDataAsync(_userData);
                        await LoadCurrentScriptAsync();
                        await DisplayCurrentScriptAsync();
                        return;
                    }
                }
                else
                {
                    // 다음 장이 있는지 확인
                    var nextChapter = chapters.FirstOrDefault(c => c.ChapterNumber == currentChapter + 1);

                    if (nextChapter != null)
                    {
                        var result = MessageBox.Show(
                            $"{currentChapter}장이 완료되었습니다!\n다음 장으로 진행하시겠습니까?\n({nextChapter.ChapterNumber}장: {nextChapter.Title})",
                            "장 완료",
                            MessageBoxButton.YesNo,
                            MessageBoxImage.Information);

                        if (result == MessageBoxResult.Yes)
                        {
                            // 다음 장으로 진행
                            _userData.CurrentStory.Chapter++;
                            _userData.CurrentStory.Episode = 1;
                            _userData.CurrentStory.ScriptIndex = 0;

                            // 다음 장 잠금 해제
                            nextChapter.IsUnlocked = true;
                            if (nextChapter.Episodes.Count > 0)
                            {
                                nextChapter.Episodes[0].IsUnlocked = true;
                            }

                            await _dataService.SaveUserDataAsync(_userData);
                            await LoadCurrentScriptAsync();
                            await DisplayCurrentScriptAsync();
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show(
                            "축하합니다! 모든 스토리를 완료했습니다!",
                            "게임 완료",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                    }
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
                        await AdvanceTextAsync();
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
                    case GameAction.ShowLog:
                        ShowInventory();
                        break;
                }
            }
        }

        private void ToggleMenu()
        {
            _isMenuVisible = !_isMenuVisible;

            // 메뉴가 열릴 때 자동재생 일시정지
            if (_isMenuVisible && _isAutoPlay)
            {
                _autoPlayTimer.Stop();
            }
            else if (!_isMenuVisible && _isAutoPlay && !_isTyping)
            {
                _autoPlayTimer.Start();
            }

            OnPropertyChanged(nameof(GameMenuVisibility));
        }

        private void ToggleAutoPlay()
        {
            _isAutoPlay = !_isAutoPlay;

            if (_isAutoPlay && !_isTyping && !_isMenuVisible)
                _autoPlayTimer.Start();
            else
                _autoPlayTimer.Stop();

            OnPropertyChanged(nameof(AutoPlayVisibility));
        }

        private async void AutoPlayTimer_Tick(object sender, EventArgs e)
        {
            if (!_isMenuVisible && _currentChoices.Count == 0 && !_isTyping)
            {
                await NextScriptAsync();
            }
        }

        private void ResumeGame()
        {
            _isMenuVisible = false;

            // 자동재생 모드였다면 다시 시작
            if (_isAutoPlay && !_isTyping)
            {
                _autoPlayTimer.Start();
            }

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
            var result = MessageBox.Show("타이틀로 돌아가시겠습니까? 현재 진행상황은 자동으로 저장됩니다.",
                                       "타이틀로 돌아가기",
                                       MessageBoxButton.YesNo,
                                       MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // 현재 진행상황 저장
                _ = _dataService.SaveUserDataAsync(_userData);
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