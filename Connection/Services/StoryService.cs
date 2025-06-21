using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Connection.Models;

namespace Connection.Services
{
    public class StoryService
    {
        private readonly string _scriptsFolder;
        private List<StoryChapter> _chapters;

        public StoryService()
        {
            _scriptsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Scripts");
            _chapters = new List<StoryChapter>();

            // 기본 스크립트 폴더 생성
            if (!Directory.Exists(_scriptsFolder))
            {
                Directory.CreateDirectory(_scriptsFolder);
                CreateSampleStory();
            }
        }

        /// <summary>
        /// 모든 스토리 챕터를 로드합니다
        /// </summary>
        public async Task<List<StoryChapter>> LoadChaptersAsync()
        {
            try
            {
                var chaptersFile = Path.Combine(_scriptsFolder, "chapters.json");

                if (!File.Exists(chaptersFile))
                {
                    CreateSampleStory();
                }

                var jsonContent = await File.ReadAllTextAsync(chaptersFile);
                _chapters = JsonConvert.DeserializeObject<List<StoryChapter>>(jsonContent) ?? new List<StoryChapter>();

                return _chapters;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"챕터 로드 실패: {ex.Message}");
                return new List<StoryChapter>();
            }
        }

        /// <summary>
        /// 특정 스크립트를 로드합니다
        /// </summary>
        public async Task<ScriptData> LoadScriptAsync(int chapter, int episode)
        {
            try
            {
                var scriptFile = Path.Combine(_scriptsFolder, $"Chapter{chapter}_Episode{episode}.json");

                if (!File.Exists(scriptFile))
                {
                    throw new FileNotFoundException($"스크립트 파일을 찾을 수 없습니다: {scriptFile}");
                }

                var jsonContent = await File.ReadAllTextAsync(scriptFile);
                return JsonConvert.DeserializeObject<ScriptData>(jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"스크립트 로드 실패: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 다음 에피소드로 진행 가능한지 확인합니다
        /// </summary>
        public bool CanProgressToNext(StoryPosition currentPosition)
        {
            var chapter = _chapters.Find(c => c.ChapterNumber == currentPosition.Chapter);
            if (chapter == null) return false;

            var episode = chapter.Episodes.Find(e => e.EpisodeNumber == currentPosition.Episode);
            if (episode == null) return false;

            // 현재 에피소드가 완료되었는지 확인
            return episode.IsCompleted;
        }

        /// <summary>
        /// 스토리 진행도를 업데이트합니다
        /// </summary>
        public void UpdateProgress(UserData userData, int chapter, int episode, bool completed = false)
        {
            userData.CurrentStory.Chapter = chapter;
            userData.CurrentStory.Episode = episode;

            if (completed)
            {
                var storyKey = $"Chapter{chapter}_Episode{episode}";
                userData.CompletedStories.Add(storyKey);

                // 다음 에피소드 잠금 해제
                UnlockNextEpisode(chapter, episode);
            }
        }

        /// <summary>
        /// 다음 에피소드의 잠금을 해제합니다
        /// </summary>
        private void UnlockNextEpisode(int currentChapter, int currentEpisode)
        {
            var chapter = _chapters.Find(c => c.ChapterNumber == currentChapter);
            if (chapter == null) return;

            // 같은 챕터 내 다음 에피소드 잠금 해제
            var nextEpisode = chapter.Episodes.Find(e => e.EpisodeNumber == currentEpisode + 1);
            if (nextEpisode != null)
            {
                nextEpisode.IsUnlocked = true;
                return;
            }

            // 다음 챕터의 첫 에피소드 잠금 해제
            var nextChapter = _chapters.Find(c => c.ChapterNumber == currentChapter + 1);
            if (nextChapter != null && nextChapter.Episodes.Count > 0)
            {
                nextChapter.IsUnlocked = true;
                nextChapter.Episodes[0].IsUnlocked = true;
            }
        }

        /// <summary>
        /// 샘플 스토리를 생성합니다
        /// </summary>
        private void CreateSampleStory()
        {
            // 샘플 챕터 구조 생성
            var chapters = new List<StoryChapter>
            {
                new StoryChapter
                {
                    ChapterNumber = 1,
                    Title = "프롤로그",
                    IsUnlocked = true,
                    Episodes = new List<StoryEpisode>
                    {
                        new StoryEpisode
                        {
                            EpisodeNumber = 1,
                            Title = "시작",
                            ScriptFile = "Chapter1_Episode1.json",
                            IsUnlocked = true
                        }
                    }
                }
            };

            // 챕터 정보 저장
            var chaptersJson = JsonConvert.SerializeObject(chapters, Formatting.Indented);
            File.WriteAllText(Path.Combine(_scriptsFolder, "chapters.json"), chaptersJson);

            // 샘플 스크립트 생성
            var sampleScript = new ScriptData
            {
                Chapter = 1,
                Episode = 1,
                Scripts = new List<ScriptLine>
                {
                    new ScriptLine
                    {
                        Index = 0,
                        Type = ScriptType.Narration,
                        Text = new Dictionary<Language, string>
                        {
                            { Language.Korean, "Connection 게임에 오신 것을 환영합니다." },
                            { Language.English, "Welcome to Connection game." },
                            { Language.Japanese, "Connectionゲームへようこそ。" }
                        }
                    },
                    new ScriptLine
                    {
                        Index = 1,
                        Type = ScriptType.Dialogue,
                        Speaker = "시스템",
                        Text = new Dictionary<Language, string>
                        {
                            { Language.Korean, "이것은 샘플 스크립트입니다. 스페이스바를 눌러 진행하세요." },
                            { Language.English, "This is a sample script. Press spacebar to continue." },
                            { Language.Japanese, "これはサンプルスクリプトです。スペースバーを押して進んでください。" }
                        }
                    }
                }
            };

            var scriptJson = JsonConvert.SerializeObject(sampleScript, Formatting.Indented);
            File.WriteAllText(Path.Combine(_scriptsFolder, "Chapter1_Episode1.json"), scriptJson);
        }
    }
}