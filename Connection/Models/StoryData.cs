using Newtonsoft.Json;
using System.Collections.Generic;

namespace Connection.Models
{
    public class StoryChapter
    {
        [JsonProperty("chapterNumber")]
        public int ChapterNumber { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("episodes")]
        public List<StoryEpisode> Episodes { get; set; } = new List<StoryEpisode>();

        [JsonProperty("isUnlocked")]
        public bool IsUnlocked { get; set; } = false;
    }

    public class StoryEpisode
    {
        [JsonProperty("episodeNumber")]
        public int EpisodeNumber { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("scriptFile")]
        public string ScriptFile { get; set; }

        [JsonProperty("isUnlocked")]
        public bool IsUnlocked { get; set; } = false;

        [JsonProperty("isCompleted")]
        public bool IsCompleted { get; set; } = false;

        [JsonProperty("requiredResources")]
        public List<string> RequiredResources { get; set; } = new List<string>();
    }
}