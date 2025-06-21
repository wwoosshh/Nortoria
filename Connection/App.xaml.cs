using System.Windows;

namespace Connection
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 전역 예외 처리
            DispatcherUnhandledException += App_DispatcherUnhandledException;
        }

        private void App_DispatcherUnhandledException(object sender,
            System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"예상치 못한 오류가 발생했습니다: {e.Exception.Message}",
                          "오류", MessageBoxButton.OK, MessageBoxImage.Error);

            // 로깅 시스템 추가 시 여기에 구현

            e.Handled = true;
        }
    }
}