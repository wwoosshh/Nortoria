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

        // 조건부 실행
        [JsonProperty("conditions")]
        public List<ScriptCondition> Conditions { get; set; } = new List<ScriptCondition>();

        // 스크립트 효과
        [JsonProperty("effects")]
        public List<ScriptEffect> Effects { get; set; } = new List<ScriptEffect>();

        // 새로 추가: 대체 텍스트 (조건에 따라 다른 텍스트 출력)
        [JsonProperty("alternativeTexts")]
        public List<ConditionalText> AlternativeTexts { get; set; } = new List<ConditionalText>();
    }

    public class Choice
    {
        [JsonProperty("id")]
        public string Id { get; set; } // 새로 추가: 선택지의 고유 ID

        [JsonProperty("text")]
        public Dictionary<Language, string> Text { get; set; } = new Dictionary<Language, string>();

        [JsonProperty("nextScriptIndex")]
        public int NextScriptIndex { get; set; }

        [JsonProperty("conditions")]
        public List<ScriptCondition> Conditions { get; set; } = new List<ScriptCondition>();

        // 선택지 효과
        [JsonProperty("effects")]
        public List<ScriptEffect> Effects { get; set; } = new List<ScriptEffect>();

        // 새로 추가: 선택지 표시 조건 (이 조건을 만족해야 선택지가 보임)
        [JsonProperty("displayConditions")]
        public List<ScriptCondition> DisplayConditions { get; set; } = new List<ScriptCondition>();

        // 새로 추가: 선택지 비용 (돈이나 아이템이 필요한 선택지)
        [JsonProperty("cost")]
        public ChoiceCost Cost { get; set; }
    }

    /// <summary>
    /// 조건부 텍스트 (플래그에 따라 다른 텍스트 출력)
    /// </summary>
    public class ConditionalText
    {
        [JsonProperty("conditions")]
        public List<ScriptCondition> Conditions { get; set; } = new List<ScriptCondition>();

        [JsonProperty("text")]
        public Dictionary<Language, string> Text { get; set; } = new Dictionary<Language, string>();

        [JsonProperty("speaker")]
        public string Speaker { get; set; }
    }

    /// <summary>
    /// 선택지 비용
    /// </summary>
    public class ChoiceCost
    {
        [JsonProperty("currency")]
        public long Currency { get; set; } = 0;

        [JsonProperty("items")]
        public Dictionary<string, int> Items { get; set; } = new Dictionary<string, int>();
    }

    /// <summary>
    /// 스크립트 조건 (플래그 기반)
    /// </summary>
    public class ScriptCondition
    {
        [JsonProperty("type")]
        public string Type { get; set; } // "flag", "item", "relationship", "choice", "character_alive"

        [JsonProperty("target")]
        public string Target { get; set; } // 플래그명, 아이템ID, 캐릭터ID, 선택지ID

        [JsonProperty("operator")]
        public string Operator { get; set; } // "equals", "greater", "less", "has", "true", "false"

        [JsonProperty("value")]
        public string Value { get; set; } // 비교값

        // 새로 추가: 조건 그룹 (AND/OR 논리 연산)
        [JsonProperty("logicOperator")]
        public string LogicOperator { get; set; } = "AND"; // "AND", "OR"

        [JsonProperty("subConditions")]
        public List<ScriptCondition> SubConditions { get; set; } = new List<ScriptCondition>();
    }

    /// <summary>
    /// 스크립트 효과 (데이터 변경)
    /// </summary>
    public class ScriptEffect
    {
        [JsonProperty("type")]
        public string Type { get; set; } // "item", "currency", "flag", "relationship", "character_state"

        [JsonProperty("action")]
        public string Action { get; set; } // "give", "take", "set", "increase", "decrease"

        [JsonProperty("target")]
        public string Target { get; set; } // 아이템 ID, 플래그 이름, 캐릭터 ID 등

        [JsonProperty("amount")]
        public int Amount { get; set; } = 1;

        [JsonProperty("message")]
        public Dictionary<Language, string> Message { get; set; } = new Dictionary<Language, string>();

        [JsonProperty("silent")]
        public bool Silent { get; set; } = false;

        // 새로 추가: 확률 기반 효과
        [JsonProperty("probability")]
        public float Probability { get; set; } = 1.0f; // 0.0 ~ 1.0

        // 새로 추가: 지연 실행
        [JsonProperty("delay")]
        public float Delay { get; set; } = 0.0f; // 초 단위
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
        Live2D,         // Live2D 애니메이션
        Jump,           // 다른 스크립트로 점프
        Conditional     // 조건부 분기 (새로 추가)
    }
}