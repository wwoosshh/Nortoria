using Newtonsoft.Json;
using System.Collections.Generic;

namespace Connection.Models
{
    public class ScriptData
    {
        [JsonProperty("chapter")]
        public int Chapter { get; set; }

        [JsonProperty("episode")]
        public int Episode { get; set; }

        [JsonProperty("scripts")]
        public List<ScriptLine> Scripts { get; set; } = new List<ScriptLine>();
    }

    public class ScriptLine
    {
        [JsonProperty("index")]
        public int Index { get; set; }

        [JsonProperty("type")]
        public ScriptType Type { get; set; }

        [JsonProperty("speaker")]
        public string Speaker { get; set; }

        [JsonProperty("text")]
        public Dictionary<Language, string> Text { get; set; } = new Dictionary<Language, string>();

        [JsonProperty("backgroundImage")]
        public string BackgroundImage { get; set; }

        [JsonProperty("characterImage")]
        public string CharacterImage { get; set; }

        [JsonProperty("voiceFile")]
        public Dictionary<Language, string> VoiceFile { get; set; } = new Dictionary<Language, string>();

        [JsonProperty("bgm")]
        public string BackgroundMusic { get; set; }

        [JsonProperty("effect")]
        public string Effect { get; set; }

        [JsonProperty("choices")]
        public List<Choice> Choices { get; set; } = new List<Choice>();

        [JsonProperty("conditions")]
        public List<string> Conditions { get; set; } = new List<string>();
    }

    public class Choice
    {
        [JsonProperty("text")]
        public Dictionary<Language, string> Text { get; set; } = new Dictionary<Language, string>();

        [JsonProperty("nextScriptIndex")]
        public int NextScriptIndex { get; set; }

        [JsonProperty("conditions")]
        public List<string> Conditions { get; set; } = new List<string>();
    }

    public enum ScriptType
    {
        Dialogue,       // 일반 대사
        Narration,      // 나레이션
        Choice,         // 선택지
        Background,     // 배경 변경
        Character,      // 캐릭터 표시
        Effect,         // 특수 효과
        Music,          // 음악 변경
        Video,          // 동영상 재생
        Live2D          // Live2D 애니메이션
    }
}