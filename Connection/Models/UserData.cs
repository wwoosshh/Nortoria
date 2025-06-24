using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Connection.Models
{
    public class UserData
    {
        [JsonProperty("playerId")]
        public string PlayerId { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("playerName")]
        public string PlayerName { get; set; } = "플레이어";

        [JsonProperty("currentStory")]
        public StoryPosition CurrentStory { get; set; } = new StoryPosition();

        [JsonProperty("gameSettings")]
        public GameSettings GameSettings { get; set; } = new GameSettings();

        [JsonProperty("inventory")]
        public Inventory Inventory { get; set; } = new Inventory();

        [JsonProperty("lastPlayTime")]
        public DateTime LastPlayTime { get; set; } = DateTime.Now;

        [JsonProperty("totalPlayTime")]
        public TimeSpan TotalPlayTime { get; set; } = TimeSpan.Zero;

        [JsonProperty("hasCompletedStories")]
        public HashSet<string> CompletedStories { get; set; } = new HashSet<string>();

        [JsonProperty("isFirstTime")]
        public bool IsFirstTime { get; set; } = true;

        // 새로 추가: 플래그 시스템 (분기점 저장용)
        [JsonProperty("storyFlags")]
        public Dictionary<string, int> StoryFlags { get; set; } = new Dictionary<string, int>();

        // 새로 추가: 관계도 시스템 (캐릭터와의 관계)
        [JsonProperty("relationships")]
        public Dictionary<string, int> Relationships { get; set; } = new Dictionary<string, int>();

        // 새로 추가: 선택 기록 (어떤 선택을 했는지 저장)
        [JsonProperty("choiceHistory")]
        public List<StoryChoice> ChoiceHistory { get; set; } = new List<StoryChoice>();

        // 플래그 관련 메서드들
        public int GetFlag(string flagName)
        {
            return StoryFlags.GetValueOrDefault(flagName, 0);
        }

        public void SetFlag(string flagName, int value)
        {
            StoryFlags[flagName] = value;
        }

        public bool HasFlag(string flagName)
        {
            return StoryFlags.ContainsKey(flagName) && StoryFlags[flagName] > 0;
        }

        public void IncrementFlag(string flagName, int amount = 1)
        {
            StoryFlags[flagName] = GetFlag(flagName) + amount;
        }

        // 관계도 관련 메서드들
        public int GetRelationship(string characterId)
        {
            return Relationships.GetValueOrDefault(characterId, 0);
        }

        public void SetRelationship(string characterId, int value)
        {
            Relationships[characterId] = Math.Max(-100, Math.Min(100, value)); // -100 ~ 100 범위로 제한
        }

        public void ChangeRelationship(string characterId, int change)
        {
            int currentValue = GetRelationship(characterId);
            SetRelationship(characterId, currentValue + change);
        }

        // 선택 기록 관련 메서드들
        public void RecordChoice(int chapter, int episode, int scriptIndex, int choiceIndex, string choiceId)
        {
            var choice = new StoryChoice
            {
                Chapter = chapter,
                Episode = episode,
                ScriptIndex = scriptIndex,
                ChoiceIndex = choiceIndex,
                ChoiceId = choiceId,
                Timestamp = DateTime.Now
            };

            ChoiceHistory.Add(choice);
        }

        public bool HasMadeChoice(string choiceId)
        {
            return ChoiceHistory.Exists(c => c.ChoiceId == choiceId);
        }

        public StoryChoice GetChoice(string choiceId)
        {
            return ChoiceHistory.Find(c => c.ChoiceId == choiceId);
        }

        // 특정 캐릭터의 생사 확인 (예시)
        public bool IsCharacterAlive(string characterId)
        {
            return GetFlag($"{characterId}_alive") > 0;
        }

        public void SetCharacterAlive(string characterId, bool isAlive)
        {
            SetFlag($"{characterId}_alive", isAlive ? 1 : 0);
        }

        // 루트별 진행도 추적
        public void SetRouteProgress(string routeName, int progress)
        {
            SetFlag($"route_{routeName}_progress", progress);
        }

        public int GetRouteProgress(string routeName)
        {
            return GetFlag($"route_{routeName}_progress");
        }
    }

    // 이 클래스를 추가하세요!
    public class StoryPosition
    {
        [JsonProperty("chapter")]
        public int Chapter { get; set; } = 1;

        [JsonProperty("episode")]
        public int Episode { get; set; } = 1;

        [JsonProperty("scriptIndex")]
        public int ScriptIndex { get; set; } = 0;

        public string GetPositionKey()
        {
            return $"Chapter{Chapter}_Episode{Episode}";
        }

        public override string ToString()
        {
            return $"{Chapter}장 {Episode}편";
        }
    }

    // 새로 추가: 선택 기록 클래스
    public class StoryChoice
    {
        [JsonProperty("chapter")]
        public int Chapter { get; set; }

        [JsonProperty("episode")]
        public int Episode { get; set; }

        [JsonProperty("scriptIndex")]
        public int ScriptIndex { get; set; }

        [JsonProperty("choiceIndex")]
        public int ChoiceIndex { get; set; }

        [JsonProperty("choiceId")]
        public string ChoiceId { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }
    }

    // 인벤토리 클래스들
    public class Inventory
    {
        [JsonProperty("items")]
        public Dictionary<string, int> Items { get; set; } = new Dictionary<string, int>();

        [JsonProperty("currency")]
        public long Currency { get; set; } = 0;

        public bool AddItem(string itemId, int quantity = 1)
        {
            if (Items.ContainsKey(itemId))
                Items[itemId] += quantity;
            else
                Items[itemId] = quantity;
            return true;
        }

        public bool HasItem(string itemId, int minQuantity = 1)
        {
            return Items.GetValueOrDefault(itemId, 0) >= minQuantity;
        }

        public int GetItemCount(string itemId)
        {
            return Items.GetValueOrDefault(itemId, 0);
        }
    }
}