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

        // 새로운 시스템: 조건부 실행
        [JsonProperty("conditions")]
        public List<ScriptCondition> Conditions { get; set; } = new List<ScriptCondition>();

        // 새로운 시스템: 스크립트 효과
        [JsonProperty("effects")]
        public List<ScriptEffect> Effects { get; set; } = new List<ScriptEffect>();
    }

    public class Choice
    {
        [JsonProperty("text")]
        public Dictionary<Language, string> Text { get; set; } = new Dictionary<Language, string>();

        [JsonProperty("nextScriptIndex")]
        public int NextScriptIndex { get; set; }

        [JsonProperty("conditions")]
        public List<ScriptCondition> Conditions { get; set; } = new List<ScriptCondition>();

        // 선택지 효과
        [JsonProperty("effects")]
        public List<ScriptEffect> Effects { get; set; } = new List<ScriptEffect>();
    }

    /// <summary>
    /// 스크립트 조건 (플래그 기반)
    /// </summary>
    public class ScriptCondition
    {
        [JsonProperty("type")]
        public string Type { get; set; } // "flag", "item", "relationship"

        [JsonProperty("target")]
        public string Target { get; set; } // 플래그명, 아이템ID, 캐릭터ID

        [JsonProperty("operator")]
        public string Operator { get; set; } // "equals", "greater", "less", "has"

        [JsonProperty("value")]
        public string Value { get; set; } // 비교값
    }

    /// <summary>
    /// 스크립트 효과 (데이터 변경)
    /// </summary>
    public class ScriptEffect
    {
        [JsonProperty("type")]
        public string Type { get; set; } // "item", "currency", "flag", "relationship"

        [JsonProperty("action")]
        public string Action { get; set; } // "give", "take", "set"

        [JsonProperty("target")]
        public string Target { get; set; } // 아이템 ID, 플래그 이름 등

        [JsonProperty("amount")]
        public int Amount { get; set; } = 1;

        [JsonProperty("message")]
        public Dictionary<Language, string> Message { get; set; } = new Dictionary<Language, string>();

        [JsonProperty("silent")]
        public bool Silent { get; set; } = false;
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