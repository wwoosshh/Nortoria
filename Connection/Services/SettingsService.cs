using System;
using System.Threading.Tasks;
using System.Windows;
using Connection.Models;
using Connection.Views;

namespace Connection.Services
{
    public class SettingsService
    {
        private readonly DataService _dataService;
        private GameSettings _currentSettings;

        public SettingsService(DataService dataService)
        {
            _dataService = dataService;
        }

        /// <summary>
        /// 설정을 로드합니다
        /// </summary>
        public async Task<GameSettings> LoadSettingsAsync()
        {
            try
            {
                var userData = await _dataService.LoadUserDataAsync();
                _currentSettings = userData.GameSettings ?? new GameSettings();

                // 설정을 즉시 적용
                ApplySettings(_currentSettings);

                return _currentSettings;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"설정 로드 실패: {ex.Message}");
                _currentSettings = new GameSettings();
                return _currentSettings;
            }
        }

        /// <summary>
        /// 설정을 저장합니다
        /// </summary>
        public async Task SaveSettingsAsync(GameSettings settings)
        {
            if (settings == null) return;

            try
            {
                var userData = await _dataService.LoadUserDataAsync();
                userData.GameSettings = settings;
                await _dataService.SaveUserDataAsync(userData);

                _currentSettings = settings;

                // 설정을 즉시 적용
                ApplySettings(settings);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"설정 저장 실패: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 그래픽 설정을 적용합니다
        /// </summary>
        private void ApplyGraphicsSettings(GraphicsSettings graphics)
        {
            if (graphics == null) return;

            Application.Current.Dispatcher.Invoke(() =>
            {
                // 모든 창에 설정 적용
                foreach (Window window in Application.Current.Windows)
                {
                    ApplyGraphicsToWindow(window, graphics);
                }
            });
        }

        /// <summary>
        /// 특정 창에 그래픽 설정을 적용합니다
        /// </summary>
        private void ApplyGraphicsToWindow(Window window, GraphicsSettings graphics)
        {
            if (window == null) return;

            // 설정 창은 항상 일반 창 모드로 유지
            if (window is SettingsWindow)
            {
                window.WindowState = WindowState.Normal;
                window.WindowStyle = WindowStyle.SingleBorderWindow;
                window.ResizeMode = ResizeMode.NoResize;
                return;
            }

            try
            {
                switch (graphics.DisplayMode)
                {
                    case DisplayMode.Fullscreen:
                        // 풀스크린: 타이틀바 있는 전체화면
                        window.WindowStyle = WindowStyle.SingleBorderWindow;
                        window.ResizeMode = ResizeMode.CanResize;
                        window.WindowState = WindowState.Maximized;
                        break;

                    case DisplayMode.Windowed:
                        // 창모드: 설정된 크기로 창 표시
                        window.WindowStyle = WindowStyle.SingleBorderWindow;
                        window.ResizeMode = ResizeMode.CanResize;
                        window.WindowState = WindowState.Normal;

                        // 해상도 설정에 따른 창 크기
                        var (width, height) = GetWindowSize(graphics.Resolution);
                        window.Width = width;
                        window.Height = height;

                        // 화면 중앙에 위치
                        window.Left = (SystemParameters.PrimaryScreenWidth - width) / 2;
                        window.Top = (SystemParameters.PrimaryScreenHeight - height) / 2;
                        break;

                    case DisplayMode.BorderlessWindowed:
                        // 테두리없는 전체화면: 타이틀바 없이 화면 꽉 채움
                        window.WindowStyle = WindowStyle.None;
                        window.ResizeMode = ResizeMode.NoResize;
                        window.WindowState = WindowState.Maximized;
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"윈도우 스타일 적용 실패: {ex.Message}");
                // 실패 시 기본 창모드로 복구
                try
                {
                    window.WindowStyle = WindowStyle.SingleBorderWindow;
                    window.WindowState = WindowState.Normal;
                    window.ResizeMode = ResizeMode.CanResize;
                }
                catch { }
            }
        }
        /// <summary>
        /// 해상도 설정에 따른 창 크기를 반환합니다
        /// </summary>
        private (double width, double height) GetWindowSize(GraphicsQuality quality)
        {
            var screenWidth = SystemParameters.PrimaryScreenWidth;
            var screenHeight = SystemParameters.PrimaryScreenHeight;

            switch (quality)
            {
                case GraphicsQuality.FHD:
                    // FHD: 1920x1080 또는 화면의 80% 중 작은 값
                    return (
                        Math.Min(1920, screenWidth * 0.8),
                        Math.Min(1080, screenHeight * 0.8)
                    );

                case GraphicsQuality.QHD:
                    // QHD: 2560x1440 또는 화면의 90% 중 작은 값
                    return (
                        Math.Min(2560, screenWidth * 0.9),
                        Math.Min(1440, screenHeight * 0.9)
                    );

                case GraphicsQuality.UHD:
                    // UHD: 3840x2160 또는 화면의 95% 중 작은 값 (4K 모니터 대응)
                    return (
                        Math.Min(3840, screenWidth * 0.95),
                        Math.Min(2160, screenHeight * 0.95)
                    );

                default:
                    return (screenWidth * 0.8, screenHeight * 0.8);
            }
        }

        /// <summary>
        /// 새로 생성되는 창에 현재 설정을 적용합니다
        /// </summary>
        public void ApplySettingsToNewWindow(Window window)
        {
            if (window == null) return;

            if (_currentSettings?.Graphics != null)
            {
                ApplyGraphicsToWindow(window, _currentSettings.Graphics);
            }
        }

        /// <summary>
        /// 모든 설정을 적용합니다
        /// </summary>
        private void ApplySettings(GameSettings settings)
        {
            if (settings?.Graphics != null)
            {
                ApplyGraphicsSettings(settings.Graphics);
            }

            // 추후 오디오, 언어 등 다른 설정들도 여기서 적용
            // ApplyAudioSettings(settings.Audio);
            // ApplyLanguageSettings(settings.Language);
        }

        /// <summary>
        /// 기본 설정으로 초기화합니다
        /// </summary>
        public async Task ResetToDefaultAsync()
        {
            var defaultSettings = new GameSettings();
            await SaveSettingsAsync(defaultSettings);
        }

        /// <summary>
        /// 현재 설정을 반환합니다
        /// </summary>
        public GameSettings GetCurrentSettings()
        {
            return _currentSettings ?? new GameSettings();
        }
    }
}