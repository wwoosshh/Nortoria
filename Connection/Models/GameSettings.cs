using Newtonsoft.Json;
using System.Collections.Generic;
using System.Windows;

namespace Connection.Models
{
    public class GameSettings
    {
        [JsonProperty("graphics")]
        public GraphicsSettings Graphics { get; set; } = new GraphicsSettings();

        [JsonProperty("controls")]
        public ControlsSettings Controls { get; set; } = new ControlsSettings();

        [JsonProperty("audio")]
        public AudioSettings Audio { get; set; } = new AudioSettings();

        [JsonProperty("language")]
        public LanguageSettings Language { get; set; } = new LanguageSettings();

        [JsonProperty("resources")]
        public ResourceSettings Resources { get; set; } = new ResourceSettings();
    }

    public class GraphicsSettings
    {
        [JsonProperty("displayMode")]
        public DisplayMode DisplayMode { get; set; } = DisplayMode.Windowed;

        [JsonProperty("frameRate")]
        public FrameRate FrameRate { get; set; } = FrameRate.FPS60;

        [JsonProperty("resolution")]
        public GraphicsQuality Resolution { get; set; } = GraphicsQuality.FHD;

        [JsonProperty("windowWidth")]
        public int WindowWidth { get; set; } = 0; // 0이면 자동으로 화면 크기의 80%로 설정

        [JsonProperty("windowHeight")]
        public int WindowHeight { get; set; } = 0; // 0이면 자동으로 화면 크기의 80%로 설정

        /// <summary>
        /// 안전한 창 너비를 반환합니다
        /// </summary>
        public int GetSafeWindowWidth()
        {
            if (WindowWidth <= 0)
            {
                try
                {
                    return (int)(System.Windows.SystemParameters.PrimaryScreenWidth * 0.8);
                }
                catch
                {
                    return 800; // 기본값
                }
            }
            return WindowWidth;
        }

        /// <summary>
        /// 안전한 창 높이를 반환합니다
        /// </summary>
        public int GetSafeWindowHeight()
        {
            if (WindowHeight <= 0)
            {
                try
                {
                    return (int)(System.Windows.SystemParameters.PrimaryScreenHeight * 0.8);
                }
                catch
                {
                    return 600; // 기본값
                }
            }
            return WindowHeight;
        }
    }

    public class ControlsSettings
    {
        [JsonProperty("keyBindings")]
        public Dictionary<GameAction, string> KeyBindings { get; set; } = GetDefaultKeyBindings();

        private static Dictionary<GameAction, string> GetDefaultKeyBindings()
        {
            return new Dictionary<GameAction, string>
            {
                { GameAction.NextScript, "Space" },
                { GameAction.FastForward, "Q" },
                { GameAction.ShowMenu, "Escape" },
                { GameAction.ShowLog, "L" },
                { GameAction.ShowSettings, "S" },
                { GameAction.Screenshot, "F12" },
                { GameAction.Skip, "Ctrl" },
                { GameAction.Auto, "A" }
            };
        }
    }

    public class AudioSettings
    {
        [JsonProperty("masterVolume")]
        public float MasterVolume { get; set; } = 1.0f;

        [JsonProperty("musicVolume")]
        public float MusicVolume { get; set; } = 0.8f;

        [JsonProperty("voiceVolume")]
        public float VoiceVolume { get; set; } = 0.9f;

        [JsonProperty("effectVolume")]
        public float EffectVolume { get; set; } = 0.7f;

        [JsonProperty("isMuted")]
        public bool IsMuted { get; set; } = false;
    }

    public class LanguageSettings
    {
        [JsonProperty("gameLanguage")]
        public Language GameLanguage { get; set; } = Language.Korean;

        [JsonProperty("voiceLanguage")]
        public Language VoiceLanguage { get; set; } = Language.Korean;
    }

    public class ResourceSettings
    {
        [JsonProperty("autoDownload")]
        public bool AutoDownload { get; set; } = true;

        [JsonProperty("autoDelete")]
        public bool AutoDelete { get; set; } = false;

        [JsonProperty("cacheSize")]
        public long CacheSize { get; set; } = 1073741824; // 1GB

        [JsonProperty("downloadQuality")]
        public ResourceQuality DownloadQuality { get; set; } = ResourceQuality.High;
    }

    // Enums
    public enum DisplayMode
    {
        Fullscreen,
        Windowed,
        BorderlessWindowed
    }

    public enum FrameRate
    {
        FPS30 = 30,
        FPS60 = 60,
        FPS120 = 120,
        Unlimited = -1
    }

    public enum GraphicsQuality
    {
        FHD,    // 1920x1080
        QHD,    // 2560x1440
        UHD     // 3840x2160
    }

    public enum Language
    {
        Korean,
        English,
        Japanese
    }

    public enum ResourceQuality
    {
        Low,
        Medium,
        High
    }

    public enum GameAction
    {
        NextScript,
        FastForward,
        ShowMenu,
        ShowLog,
        ShowSettings,
        Screenshot,
        Skip,
        Auto
    }
}