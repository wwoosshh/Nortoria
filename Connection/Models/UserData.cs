using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Connection.Models
{
    public class UserData
    {
        [JsonProperty("playerId")]
        public string PlayerId { get; set; } = Guid.NewGuid().ToString();

        [JsonProperty("currentStory")]
        public StoryPosition CurrentStory { get; set; } = new StoryPosition();

        [JsonProperty("gameSettings")]
        public GameSettings GameSettings { get; set; } = new GameSettings();

        [JsonProperty("lastPlayTime")]
        public DateTime LastPlayTime { get; set; } = DateTime.Now;

        [JsonProperty("totalPlayTime")]
        public TimeSpan TotalPlayTime { get; set; } = TimeSpan.Zero;

        [JsonProperty("hasCompletedStories")]
        public HashSet<string> CompletedStories { get; set; } = new HashSet<string>();

        [JsonProperty("isFirstTime")]
        public bool IsFirstTime { get; set; } = true;
    }

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
}