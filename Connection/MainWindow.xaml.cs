using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Connection.ViewModels;
using Connection.Services;
using Connection.Models;
using System.Windows.Media.Imaging;

namespace Connection
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer _particleTimer;
        private Random _random = new Random();

        public MainWindow()
        {
            InitializeComponent();

            // 설정 서비스 초기화 및 화면 크기 적용
            InitializeWindowSettings();

            DataContext = new MainViewModel();

            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        /// <summary>
        /// 윈도우 설정 초기화 (화면 크기, 모드 등)
        /// </summary>
        private async void InitializeWindowSettings()
        {
            try
            {
                var dataService = new DataService();
                var settingsService = new SettingsService(dataService);

                // 설정 로드
                var settings = await settingsService.LoadSettingsAsync();

                // 화면 크기 적용
                ApplyDisplaySettings(settings.Graphics);

                // 설정 서비스를 윈도우에 적용
                settingsService.ApplySettingsToNewWindow(this);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"윈도우 설정 초기화 실패: {ex.Message}");
                // 기본 설정 적용
                ApplyDefaultDisplaySettings();
            }
        }

        /// <summary>
        /// 디스플레이 설정 적용
        /// </summary>
        private void ApplyDisplaySettings(GraphicsSettings graphics)
        {
            try
            {
                switch (graphics.DisplayMode)
                {
                    case DisplayMode.Fullscreen:
                        WindowStyle = WindowStyle.SingleBorderWindow;
                        WindowState = WindowState.Maximized;
                        ResizeMode = ResizeMode.NoResize;
                        break;

                    case DisplayMode.BorderlessWindowed:
                        WindowStyle = WindowStyle.None;
                        WindowState = WindowState.Maximized;
                        ResizeMode = ResizeMode.NoResize;
                        break;

                    case DisplayMode.Windowed:
                    default:
                        WindowStyle = WindowStyle.SingleBorderWindow;
                        WindowState = WindowState.Normal;
                        ResizeMode = ResizeMode.NoResize;

                        // 해상도에 따른 창 크기 설정
                        var (width, height) = GetWindowSizeForResolution(graphics.Resolution);
                        Width = width;
                        Height = height;

                        // 화면 중앙에 위치
                        Left = (SystemParameters.PrimaryScreenWidth - width) / 2;
                        Top = (SystemParameters.PrimaryScreenHeight - height) / 2;
                        break;
                }

                // UI 스케일 조정
                AdjustUIScale(graphics.Resolution);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"디스플레이 설정 적용 실패: {ex.Message}");
                ApplyDefaultDisplaySettings();
            }
        }

        /// <summary>
        /// 기본 디스플레이 설정 적용
        /// </summary>
        private void ApplyDefaultDisplaySettings()
        {
            WindowStyle = WindowStyle.SingleBorderWindow;
            WindowState = WindowState.Normal;
            ResizeMode = ResizeMode.NoResize;
            Width = 1920;
            Height = 1080;

            // 화면 중앙에 위치
            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;
        }

        /// <summary>
        /// 해상도에 따른 창 크기 반환
        /// </summary>
        private (double width, double height) GetWindowSizeForResolution(GraphicsQuality quality)
        {
            return quality switch
            {
                GraphicsQuality.FHD => (1920, 1080),
                GraphicsQuality.QHD => (2560, 1440),
                GraphicsQuality.UHD => (3840, 2160),
                _ => (1920, 1080)
            };
        }

        /// <summary>
        /// UI 스케일 조정 (해상도에 따라)
        /// </summary>
        private void AdjustUIScale(GraphicsQuality quality)
        {
            double scale = quality switch
            {
                GraphicsQuality.FHD => 1.0,
                GraphicsQuality.QHD => 1.33,  // 2560/1920
                GraphicsQuality.UHD => 2.0,   // 3840/1920
                _ => 1.0
            };

            // 전체 UI 스케일 적용
            var scaleTransform = new ScaleTransform(scale, scale);
            RenderTransform = scaleTransform;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            // 배경 이미지 로딩 확인 및 설정
            InitializeBackgroundImage();

            // 페이드인 애니메이션
            var fadeInAnimation = new DoubleAnimation(0, 1, TimeSpan.FromSeconds(1.5));
            BeginAnimation(OpacityProperty, fadeInAnimation);

            // 동적 파티클 생성 시작
            StartParticleAnimation();

            // 버튼들에 스테거드 애니메이션 적용
            ApplyStaggeredButtonAnimation();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            _particleTimer?.Stop();
        }

        /// <summary>
        /// 동적 파티클 효과 시작
        /// </summary>
        private void StartParticleAnimation()
        {
            _particleTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(800)
            };

            _particleTimer.Tick += (s, e) => CreateParticle();
            _particleTimer.Start();
        }

        /// <summary>
        /// 새로운 파티클 생성
        /// </summary>
        private void CreateParticle()
        {
            var particle = new Ellipse
            {
                Width = _random.Next(2, 6),
                Height = _random.Next(2, 6),
                Fill = new SolidColorBrush(Color.FromArgb(
                    (byte)_random.Next(50, 150),
                    (byte)_random.Next(100, 255),
                    (byte)_random.Next(150, 255),
                    (byte)_random.Next(200, 255)
                ))
            };

            var startX = _random.Next(0, (int)ActualWidth);
            var startY = ActualHeight + 20;
            var endY = -20;
            var drift = _random.Next(-50, 50);

            Canvas.SetLeft(particle, startX);
            Canvas.SetTop(particle, startY);
            ParticleCanvas.Children.Add(particle);

            // 애니메이션 생성
            var storyboard = new Storyboard();

            // Y축 이동
            var moveYAnimation = new DoubleAnimation
            {
                From = startY,
                To = endY,
                Duration = TimeSpan.FromSeconds(_random.Next(6, 12))
            };
            Storyboard.SetTarget(moveYAnimation, particle);
            Storyboard.SetTargetProperty(moveYAnimation, new PropertyPath("(Canvas.Top)"));
            storyboard.Children.Add(moveYAnimation);

            // X축 드리프트
            if (Math.Abs(drift) > 10)
            {
                var moveXAnimation = new DoubleAnimation
                {
                    From = startX,
                    To = startX + drift,
                    Duration = TimeSpan.FromSeconds(_random.Next(8, 15))
                };
                Storyboard.SetTarget(moveXAnimation, particle);
                Storyboard.SetTargetProperty(moveXAnimation, new PropertyPath("(Canvas.Left)"));
                storyboard.Children.Add(moveXAnimation);
            }

            // 투명도 애니메이션
            var opacityAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(2),
                AutoReverse = true
            };
            Storyboard.SetTarget(opacityAnimation, particle);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
            storyboard.Children.Add(opacityAnimation);

            // 애니메이션 완료 시 파티클 제거
            storyboard.Completed += (s, e) =>
            {
                ParticleCanvas.Children.Remove(particle);
            };

            storyboard.Begin();
        }

        /// <summary>
        /// 버튼들에 순차적 등장 애니메이션 적용
        /// </summary>
        private void ApplyStaggeredButtonAnimation()
        {
            var buttonsPanel = FindName("ChoicesPanel") as StackPanel;
            if (buttonsPanel?.Parent is StackPanel mainPanel)
            {
                for (int i = 0; i < mainPanel.Children.Count; i++)
                {
                    if (mainPanel.Children[i] is Button button)
                    {
                        // 초기 상태 설정
                        button.Opacity = 0;
                        button.RenderTransform = new TranslateTransform(0, 50);

                        // 지연 시간을 두고 애니메이션 시작
                        var timer = new DispatcherTimer
                        {
                            Interval = TimeSpan.FromMilliseconds(200 * i + 500)
                        };

                        timer.Tick += (s, e) =>
                        {
                            timer.Stop();
                            AnimateButtonIn(button);
                        };

                        timer.Start();
                    }
                }
            }
        }

        /// <summary>
        /// 개별 버튼 등장 애니메이션
        /// </summary>
        private void AnimateButtonIn(Button button)
        {
            var storyboard = new Storyboard();

            // 투명도 애니메이션
            var opacityAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromSeconds(0.8),
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
            };
            Storyboard.SetTarget(opacityAnimation, button);
            Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath("Opacity"));
            storyboard.Children.Add(opacityAnimation);

            // Y축 이동 애니메이션
            var translateAnimation = new DoubleAnimation
            {
                From = 50,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.8),
                EasingFunction = new BackEase { EasingMode = EasingMode.EaseOut, Amplitude = 0.3 }
            };
            Storyboard.SetTarget(translateAnimation, button);
            Storyboard.SetTargetProperty(translateAnimation, new PropertyPath("(RenderTransform).(TranslateTransform.Y)"));
            storyboard.Children.Add(translateAnimation);

            storyboard.Begin();
        }

        /// <summary>
        /// 윈도우 크기 변경 시 파티클 정리
        /// </summary>
        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);

            // 화면 밖의 파티클들 제거
            for (int i = ParticleCanvas.Children.Count - 1; i >= 0; i--)
            {
                if (ParticleCanvas.Children[i] is Ellipse particle)
                {
                    var top = Canvas.GetTop(particle);
                    var left = Canvas.GetLeft(particle);

                    if (top < -50 || top > ActualHeight + 50 ||
                        left < -50 || left > ActualWidth + 50)
                    {
                        ParticleCanvas.Children.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// 배경 이미지 초기화 및 로딩 확인
        /// </summary>
        private void InitializeBackgroundImage()
        {
            try
            {
                // 여러 경로 시도
                string[] imagePaths = {
                    "/Resources/Images/main_background.png",
                    "pack://application:,,,/Resources/Images/main_background.png",
                    System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", "main_background.png")
                };

                bool imageLoaded = false;
                foreach (string path in imagePaths)
                {
                    try
                    {
                        if (path.StartsWith("/") || path.StartsWith("pack://"))
                        {
                            // 리소스 URI 시도
                            var uri = new Uri(path, UriKind.RelativeOrAbsolute);
                            var bitmap = new System.Windows.Media.Imaging.BitmapImage(uri);
                            BackgroundImage.Source = bitmap;
                            imageLoaded = true;
                            System.Diagnostics.Debug.WriteLine($"배경 이미지 로드 성공: {path}");
                            break;
                        }
                        else if (System.IO.File.Exists(path))
                        {
                            // 파일 경로 시도
                            var uri = new Uri(path, UriKind.Absolute);
                            var bitmap = new System.Windows.Media.Imaging.BitmapImage(uri);
                            BackgroundImage.Source = bitmap;
                            imageLoaded = true;
                            System.Diagnostics.Debug.WriteLine($"배경 이미지 로드 성공: {path}");
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"배경 이미지 로드 실패 - {path}: {ex.Message}");
                        continue;
                    }
                }

                if (!imageLoaded)
                {
                    System.Diagnostics.Debug.WriteLine("모든 배경 이미지 로드 실패 - 기본 배경 사용");
                    CreateFallbackBackground();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"배경 이미지 초기화 실패: {ex.Message}");
                CreateFallbackBackground();
            }
        }

        /// <summary>
        /// 이미지 로드 실패 시 대체 배경 생성
        /// </summary>
        private void CreateFallbackBackground()
        {
            try
            {
                var drawingVisual = new DrawingVisual();
                using (var context = drawingVisual.RenderOpen())
                {
                    var brush = new LinearGradientBrush();
                    brush.StartPoint = new Point(0, 0);
                    brush.EndPoint = new Point(1, 1);
                    brush.GradientStops.Add(new GradientStop(Color.FromRgb(15, 15, 35), 0));
                    brush.GradientStops.Add(new GradientStop(Color.FromRgb(26, 26, 46), 0.3));
                    brush.GradientStops.Add(new GradientStop(Color.FromRgb(22, 33, 62), 0.7));
                    brush.GradientStops.Add(new GradientStop(Color.FromRgb(15, 52, 96), 1));

                    context.DrawRectangle(brush, null, new Rect(0, 0, 1920, 1080));

                    // 별 효과 추가
                    var starBrush = new SolidColorBrush(Colors.White);
                    var random = new Random(42); // 고정 시드로 일정한 패턴
                    for (int i = 0; i < 100; i++)
                    {
                        var x = random.Next(0, 1920);
                        var y = random.Next(0, 1080);
                        var size = random.Next(1, 4);
                        context.DrawEllipse(starBrush, null, new Point(x, y), size, size);
                    }
                }

                var bitmap = new RenderTargetBitmap(1920, 1080, 96, 96, PixelFormats.Pbgra32);
                bitmap.Render(drawingVisual);
                bitmap.Freeze();

                BackgroundImage.Source = bitmap;
                System.Diagnostics.Debug.WriteLine("대체 배경 생성 완료");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"대체 배경 생성 실패: {ex.Message}");
                // 최종적으로 이미지를 숨김
                BackgroundImage.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 메뉴 버튼 호버 사운드 효과 (추후 구현)
        /// </summary>
        private void PlayHoverSound()
        {
            // 추후 사운드 시스템 구현 시 여기에 호버 사운드 재생
        }

        /// <summary>
        /// 메뉴 버튼 클릭 사운드 효과 (추후 구현)  
        /// </summary>
        private void PlayClickSound()
        {
            // 추후 사운드 시스템 구현 시 여기에 클릭 사운드 재생
        }
    }
}