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
using Connection.Utils;

namespace Connection.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly SettingsService _settingsService;
        private readonly Window _window;
        private GameSettings _originalSettings;
        private GameSettings _currentSettings;
        private Language _currentLanguage = Language.Korean;

        public SettingsViewModel(SettingsService settingsService, Window window)
        {
            _settingsService = settingsService;
            _window = window;

            // 명령어 초기화
            SaveCommand = new AsyncRelayCommand(SaveSettingsAsync);
            CancelCommand = new RelayCommand(CancelSettings);
            ResetToDefaultCommand = new AsyncRelayCommand(ResetToDefaultAsync);
            ClearCacheCommand = new RelayCommand(ClearCache);
            RedownloadResourcesCommand = new RelayCommand(RedownloadResources);

            // 초기화
            _ = InitializeAsync();
        }

        #region 다국어 지원 프로퍼티들

        public string SettingsWindowTitle => LocalizationHelper.GetLocalizedString("Settings", _currentLanguage);

        // 탭 헤더
        public string GraphicsTabText => LocalizationHelper.GetLocalizedString("Graphics", _currentLanguage);
        public string ControlsTabText => LocalizationHelper.GetLocalizedString("Controls", _currentLanguage);
        public string AudioTabText => LocalizationHelper.GetLocalizedString("Audio", _currentLanguage);
        public string LanguageTabText => LocalizationHelper.GetLocalizedString("Language", _currentLanguage);
        public string ResourcesTabText => LocalizationHelper.GetLocalizedString("Resources", _currentLanguage);
        public string MiscTabText => LocalizationHelper.GetLocalizedString("Misc", _currentLanguage);

        // 버튼 텍스트
        public string SaveButtonText => LocalizationHelper.GetLocalizedString("Save", _currentLanguage);
        public string CancelButtonText => LocalizationHelper.GetLocalizedString("Cancel", _currentLanguage);

        // 그래픽 설정 라벨
        public string DisplayModeText => LocalizationHelper.GetLocalizedString("DisplayMode", _currentLanguage);
        public string FrameRateText => LocalizationHelper.GetLocalizedString("FrameRate", _currentLanguage);
        public string GraphicsQualityText => LocalizationHelper.GetLocalizedString("GraphicsQuality", _currentLanguage);
        public string WindowSizeText => LocalizationHelper.GetLocalizedString("WindowSize", _currentLanguage);

        // 조작 설정 라벨
        public string KeySettingsText => LocalizationHelper.GetLocalizedString("KeySettings", _currentLanguage);
        public string ChangeText => LocalizationHelper.GetLocalizedString("Change", _currentLanguage);

        // 음향 설정 라벨
        public string MasterVolumeText => LocalizationHelper.GetLocalizedString("MasterVolume", _currentLanguage);
        public string MusicText => LocalizationHelper.GetLocalizedString("Music", _currentLanguage);
        public string VoiceText => LocalizationHelper.GetLocalizedString("Voice", _currentLanguage);
        public string EffectsText => LocalizationHelper.GetLocalizedString("Effects", _currentLanguage);
        public string MuteText => LocalizationHelper.GetLocalizedString("Mute", _currentLanguage);

        // 언어 설정 라벨
        public string GameLanguageText => LocalizationHelper.GetLocalizedString("GameLanguage", _currentLanguage);
        public string VoiceLanguageText => LocalizationHelper.GetLocalizedString("VoiceLanguage", _currentLanguage);

        // 리소스 설정 라벨
        public string ResourceManagementText => LocalizationHelper.GetLocalizedString("ResourceManagement", _currentLanguage);
        public string AutoDownloadText => LocalizationHelper.GetLocalizedString("AutoDownload", _currentLanguage);
        public string AutoDeleteText => LocalizationHelper.GetLocalizedString("AutoDeleteOldResources", _currentLanguage);
        public string DownloadQualityText => LocalizationHelper.GetLocalizedString("DownloadQuality", _currentLanguage);
        public string CacheSizeLimitText => LocalizationHelper.GetLocalizedString("CacheSizeLimit", _currentLanguage);
        public string ClearCacheText => LocalizationHelper.GetLocalizedString("ClearCache", _currentLanguage);
        public string RedownloadResourcesText => LocalizationHelper.GetLocalizedString("RedownloadResources", _currentLanguage);

        // 기타 설정 라벨
        public string GameInfoText => LocalizationHelper.GetLocalizedString("GameInfo", _currentLanguage);
        public string DeveloperText => LocalizationHelper.GetLocalizedString("Developer", _currentLanguage);
        public string EngineText => LocalizationHelper.GetLocalizedString("Engine", _currentLanguage);
        public string CopyrightInfoText => LocalizationHelper.GetLocalizedString("CopyrightInfo", _currentLanguage);
        public string ResetToDefaultText => LocalizationHelper.GetLocalizedString("ResetToDefault", _currentLanguage);

        #endregion

        #region Properties

        // 그래픽 설정
        public DisplayMode DisplayMode
        {
            get => _currentSettings?.Graphics.DisplayMode ?? DisplayMode.Windowed;
            set
            {
                if (_currentSettings != null)
                {
                    _currentSettings.Graphics.DisplayMode = value;
                    OnPropertyChanged();
                }
            }
        }

        public FrameRate FrameRate
        {
            get => _currentSettings?.Graphics.FrameRate ?? FrameRate.FPS60;
            set
            {
                if (_currentSettings != null)
                {
                    _currentSettings.Graphics.FrameRate = value;
                    OnPropertyChanged();
                }
            }
        }

        public GraphicsQuality GraphicsQuality
        {
            get => _currentSettings?.Graphics.Resolution ?? GraphicsQuality.FHD;
            set
            {
                if (_currentSettings != null)
                {
                    _currentSettings.Graphics.Resolution = value;
                    OnPropertyChanged();
                    // 해상도 변경 시 창 크기도 업데이트
                    OnPropertyChanged(nameof(WindowWidth));
                    OnPropertyChanged(nameof(WindowHeight));
                }
            }
        }

        public int WindowWidth
        {
            get
            {
                if (_currentSettings?.Graphics != null)
                {
                    var (width, height) = GetWindowSizeForResolution(_currentSettings.Graphics.Resolution);
                    return (int)width;
                }
                return 1920;
            }
            set
            {
                if (_currentSettings != null)
                {
                    _currentSettings.Graphics.WindowWidth = value;
                    OnPropertyChanged();
                }
            }
        }

        public int WindowHeight
        {
            get
            {
                if (_currentSettings?.Graphics != null)
                {
                    var (width, height) = GetWindowSizeForResolution(_currentSettings.Graphics.Resolution);
                    return (int)height;
                }
                return 1080;
            }
            set
            {
                if (_currentSettings != null)
                {
                    _currentSettings.Graphics.WindowHeight = value;
                    OnPropertyChanged();
                }
            }
        }

        // 음향 설정
        public float MasterVolume
        {
            get => _currentSettings?.Audio.MasterVolume ?? 1.0f;
            set
            {
                if (_currentSettings != null)
                {
                    _currentSettings.Audio.MasterVolume = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(MasterVolumePercent));
                }
            }
        }

        public float MusicVolume
        {
            get => _currentSettings?.Audio.MusicVolume ?? 0.8f;
            set
            {
                if (_currentSettings != null)
                {
                    _currentSettings.Audio.MusicVolume = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(MusicVolumePercent));
                }
            }
        }

        public float VoiceVolume
        {
            get => _currentSettings?.Audio.VoiceVolume ?? 0.9f;
            set
            {
                if (_currentSettings != null)
                {
                    _currentSettings.Audio.VoiceVolume = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(VoiceVolumePercent));
                }
            }
        }

        public float EffectVolume
        {
            get => _currentSettings?.Audio.EffectVolume ?? 0.7f;
            set
            {
                if (_currentSettings != null)
                {
                    _currentSettings.Audio.EffectVolume = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(EffectVolumePercent));
                }
            }
        }

        public bool IsMuted
        {
            get => _currentSettings?.Audio.IsMuted ?? false;
            set
            {
                if (_currentSettings != null)
                {
                    _currentSettings.Audio.IsMuted = value;
                    OnPropertyChanged();
                }
            }
        }

        // 볼륨 퍼센트 표시
        public string MasterVolumePercent => $"{(int)(MasterVolume * 100)}%";
        public string MusicVolumePercent => $"{(int)(MusicVolume * 100)}%";
        public string VoiceVolumePercent => $"{(int)(VoiceVolume * 100)}%";
        public string EffectVolumePercent => $"{(int)(EffectVolume * 100)}%";

        // 언어 설정
        public Language GameLanguage
        {
            get => _currentSettings?.Language.GameLanguage ?? Language.Korean;
            set
            {
                if (_currentSettings != null)
                {
                    _currentSettings.Language.GameLanguage = value;
                    _currentLanguage = value; // 현재 언어 업데이트
                    OnPropertyChanged();
                    UpdateUITexts(); // UI 텍스트 업데이트
                }
            }
        }
        /// <summary>
        /// 모든 UI 텍스트를 현재 언어로 업데이트
        /// </summary>
        private void UpdateUITexts()
        {
            // 탭 헤더 업데이트
            OnPropertyChanged(nameof(GraphicsTabText));
            OnPropertyChanged(nameof(ControlsTabText));
            OnPropertyChanged(nameof(AudioTabText));
            OnPropertyChanged(nameof(LanguageTabText));
            OnPropertyChanged(nameof(ResourcesTabText));
            OnPropertyChanged(nameof(MiscTabText));

            // 버튼 텍스트 업데이트
            OnPropertyChanged(nameof(SaveButtonText));
            OnPropertyChanged(nameof(CancelButtonText));

            // 라벨 텍스트 업데이트
            OnPropertyChanged(nameof(DisplayModeText));
            OnPropertyChanged(nameof(FrameRateText));
            OnPropertyChanged(nameof(GraphicsQualityText));
            OnPropertyChanged(nameof(WindowSizeText));
            OnPropertyChanged(nameof(KeySettingsText));
            OnPropertyChanged(nameof(ChangeText));
            OnPropertyChanged(nameof(MasterVolumeText));
            OnPropertyChanged(nameof(MusicText));
            OnPropertyChanged(nameof(VoiceText));
            OnPropertyChanged(nameof(EffectsText));
            OnPropertyChanged(nameof(MuteText));
            OnPropertyChanged(nameof(GameLanguageText));
            OnPropertyChanged(nameof(VoiceLanguageText));
            OnPropertyChanged(nameof(ResourceManagementText));
            OnPropertyChanged(nameof(AutoDownloadText));
            OnPropertyChanged(nameof(AutoDeleteText));
            OnPropertyChanged(nameof(DownloadQualityText));
            OnPropertyChanged(nameof(CacheSizeLimitText));
            OnPropertyChanged(nameof(ClearCacheText));
            OnPropertyChanged(nameof(RedownloadResourcesText));
            OnPropertyChanged(nameof(GameInfoText));
            OnPropertyChanged(nameof(CopyrightInfoText));
            OnPropertyChanged(nameof(ResetToDefaultText));
        }

        public Language VoiceLanguage
        {
            get => _currentSettings?.Language.VoiceLanguage ?? Language.Korean;
            set
            {
                if (_currentSettings != null)
                {
                    _currentSettings.Language.VoiceLanguage = value;
                    OnPropertyChanged();
                }
            }
        }

        // 리소스 설정
        public bool AutoDownload
        {
            get => _currentSettings?.Resources.AutoDownload ?? true;
            set
            {
                if (_currentSettings != null)
                {
                    _currentSettings.Resources.AutoDownload = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool AutoDelete
        {
            get => _currentSettings?.Resources.AutoDelete ?? false;
            set
            {
                if (_currentSettings != null)
                {
                    _currentSettings.Resources.AutoDelete = value;
                    OnPropertyChanged();
                }
            }
        }

        public ResourceQuality DownloadQuality
        {
            get => _currentSettings?.Resources.DownloadQuality ?? ResourceQuality.High;
            set
            {
                if (_currentSettings != null)
                {
                    _currentSettings.Resources.DownloadQuality = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CacheSizeGB
        {
            get => $"{(_currentSettings?.Resources.CacheSize ?? 1073741824) / 1073741824.0:F1}";
            set
            {
                if (_currentSettings != null && float.TryParse(value, out float gb))
                {
                    _currentSettings.Resources.CacheSize = (long)(gb * 1073741824);
                    OnPropertyChanged();
                }
            }
        }

        // 콤보박스 아이템 소스
        public IEnumerable<DisplayMode> DisplayModes => Enum.GetValues<DisplayMode>();
        public IEnumerable<FrameRate> FrameRates => Enum.GetValues<FrameRate>();
        public IEnumerable<GraphicsQuality> GraphicsQualities => Enum.GetValues<GraphicsQuality>();
        public IEnumerable<Language> Languages => Enum.GetValues<Language>();
        public IEnumerable<ResourceQuality> ResourceQualities => Enum.GetValues<ResourceQuality>();

        // 키 바인딩
        public ObservableCollection<KeyBindingViewModel> KeyBindings { get; } = new ObservableCollection<KeyBindingViewModel>();

        #endregion

        #region Commands

        public IAsyncRelayCommand SaveCommand { get; }
        public IRelayCommand CancelCommand { get; }
        public IAsyncRelayCommand ResetToDefaultCommand { get; }
        public IRelayCommand ClearCacheCommand { get; }
        public IRelayCommand RedownloadResourcesCommand { get; }

        #endregion

        #region Methods

        /// <summary>
        /// 해상도 설정에 따른 창 크기를 반환합니다
        /// </summary>
        private (double width, double height) GetWindowSizeForResolution(GraphicsQuality quality)
        {
            switch (quality)
            {
                case GraphicsQuality.FHD:
                    return (1920, 1080);
                case GraphicsQuality.QHD:
                    return (2560, 1440);
                case GraphicsQuality.UHD:
                    return (3840, 2160);
                default:
                    return (1920, 1080);
            }
        }

        private async Task InitializeAsync()
        {
            try
            {
                _originalSettings = await _settingsService.LoadSettingsAsync();
                _currentSettings = CloneSettings(_originalSettings);
                _currentLanguage = _currentSettings.Language.GameLanguage;

                // 키 바인딩 초기화
                InitializeKeyBindings();

                // 모든 프로퍼티 알림
                OnPropertyChanged(string.Empty);
                UpdateUITexts();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"설정 로드 중 오류가 발생했습니다: {ex.Message}",
                              "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InitializeKeyBindings()
        {
            KeyBindings.Clear();

            if (_currentSettings?.Controls?.KeyBindings != null)
            {
                foreach (var binding in _currentSettings.Controls.KeyBindings)
                {
                    KeyBindings.Add(new KeyBindingViewModel
                    {
                        Action = binding.Key,
                        ActionName = GetActionDisplayName(binding.Key),
                        KeyName = binding.Value,
                        ChangeKeyCommand = new RelayCommand<GameAction>(ChangeKey)
                    });
                }
            }
        }

        private string GetActionDisplayName(GameAction action)
        {
            return action switch
            {
                GameAction.NextScript => "다음 스크립트",
                GameAction.FastForward => "빨리감기",
                GameAction.ShowMenu => "메뉴 표시",
                GameAction.ShowLog => "로그 표시",
                GameAction.ShowSettings => "설정 표시",
                GameAction.Screenshot => "스크린샷",
                GameAction.Skip => "스킵",
                GameAction.Auto => "자동재생",
                _ => action.ToString()
            };
        }

        private void ChangeKey(GameAction action)
        {
            // 키 변경 다이얼로그 구현 (추후)
            MessageBox.Show($"{GetActionDisplayName(action)} 키 변경 기능은 곧 구현 예정입니다.",
                          "알림", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task SaveSettingsAsync()
        {
            try
            {
                await _settingsService.SaveSettingsAsync(_currentSettings);
                _originalSettings = CloneSettings(_currentSettings);

                // 설정 저장 후 모든 창에 즉시 적용
                foreach (Window window in Application.Current.Windows)
                {
                    if (window != _window) // 설정 창 제외
                    {
                        _settingsService.ApplySettingsToNewWindow(window);

                        // 메인 윈도우의 언어 업데이트
                        if (window.DataContext is MainViewModel mainViewModel)
                        {
                            mainViewModel.OnLanguageChanged(_currentSettings.Language.GameLanguage);
                        }
                    }
                }

                _window.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"설정 저장 중 오류가 발생했습니다: {ex.Message}",
                              "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelSettings()
        {
            _window.Close();
        }

        private async Task ResetToDefaultAsync()
        {
            var result = MessageBox.Show("모든 설정을 기본값으로 초기화하시겠습니까?",
                                       "설정 초기화", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    await _settingsService.ResetToDefaultAsync();
                    await InitializeAsync();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"설정 초기화 중 오류가 발생했습니다: {ex.Message}",
                                  "오류", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ClearCache()
        {
            var result = MessageBox.Show("캐시를 삭제하시겠습니까?",
                                       "캐시 삭제", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // 추후 캐시 삭제 로직 구현
                MessageBox.Show("캐시가 삭제되었습니다.", "완료", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void RedownloadResources()
        {
            var result = MessageBox.Show("모든 리소스를 재다운로드하시겠습니까?",
                                       "리소스 재다운로드", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // 추후 리소스 재다운로드 로직 구현
                MessageBox.Show("리소스 재다운로드가 시작되었습니다.", "시작", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private GameSettings CloneSettings(GameSettings settings)
        {
            // 간단한 클론 (JSON 직렬화 사용)
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(settings);
            return Newtonsoft.Json.JsonConvert.DeserializeObject<GameSettings>(json);
        }

        #endregion
    }

    public class KeyBindingViewModel
    {
        public GameAction Action { get; set; }
        public string ActionName { get; set; }
        public string KeyName { get; set; }
        public RelayCommand<GameAction> ChangeKeyCommand { get; set; }
    }
}