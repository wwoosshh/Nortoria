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