using System.Windows;

namespace Connection.Utils
{
    public static class Constants
    {
        // 파일 경로
        public const string GAME_TITLE = "Connection";
        public const string VERSION = "1.0.0";

        // 기본 설정값
        public static class DefaultSettings
        {
            public static int DEFAULT_WINDOW_WIDTH => (int)(System.Windows.SystemParameters.PrimaryScreenWidth * 0.8);
            public static int DEFAULT_WINDOW_HEIGHT => (int)(System.Windows.SystemParameters.PrimaryScreenHeight * 0.8);
            public const float DEFAULT_MASTER_VOLUME = 1.0f;
            public const float DEFAULT_MUSIC_VOLUME = 0.8f;
            public const float DEFAULT_VOICE_VOLUME = 0.9f;
            public const float DEFAULT_EFFECT_VOLUME = 0.7f;
        }

        // 리소스 경로
        public static class Paths
        {
            public const string RESOURCES_FOLDER = "Resources";
            public const string SCRIPTS_FOLDER = "Scripts";
            public const string IMAGES_FOLDER = "Images";
            public const string AUDIO_FOLDER = "Audio";
            public const string LOCALIZATION_FOLDER = "Localization";
        }

        // 파일 확장자
        public static class FileExtensions
        {
            public const string JSON = ".json";
            public const string PNG = ".png";
            public const string JPG = ".jpg";
            public const string MP3 = ".mp3";
            public const string WAV = ".wav";
            public const string MP4 = ".mp4";
        }

        // 게임 설정
        public static class GameSettings
        {
            public const int AUTO_SAVE_INTERVAL = 30; // 초
            public const int AUTO_PLAY_SPEED = 2; // 초
            public const double TEXT_DISPLAY_SPEED = 0.05; // 초당 문자
        }
    }
}