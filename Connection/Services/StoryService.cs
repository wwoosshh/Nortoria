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
                CreateChapterStructure();
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
                    CreateChapterStructure();
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
        /// 챕터 구조만 생성합니다 (스크립트는 별도로 작성)
        /// </summary>
        private void CreateChapterStructure()
        {
            // 챕터 구조 생성 (10개 나라 + 추가 챕터들)
            var chapters = new List<StoryChapter>
            {
                new StoryChapter
                {
                    ChapterNumber = 1,
                    Title = "노토리얼 메시지",
                    IsUnlocked = true,
                    Episodes = new List<StoryEpisode>
                    {
                        new StoryEpisode
                        {
                            EpisodeNumber = 1,
                            Title = "이야기의 시작",
                            ScriptFile = "Chapter1_Episode1.json",
                            IsUnlocked = true
                        },
                        new StoryEpisode
                        {
                            EpisodeNumber = 2,
                            Title = "나무의 나라로",
                            ScriptFile = "Chapter1_Episode2.json",
                            IsUnlocked = false
                        },
                        new StoryEpisode
                        {
                            EpisodeNumber = 3,
                            Title = "라시 치우비와의 만남",
                            ScriptFile = "Chapter1_Episode3.json",
                            IsUnlocked = false
                        },
                        new StoryEpisode
                        {
                            EpisodeNumber = 4,
                            Title = "첫 번째 시험",
                            ScriptFile = "Chapter1_Episode4.json",
                            IsUnlocked = false
                        },
                        new StoryEpisode
                        {
                            EpisodeNumber = 5,
                            Title = "나무의 권한",
                            ScriptFile = "Chapter1_Episode5.json",
                            IsUnlocked = false
                        }
                    }
                },
                new StoryChapter
                {
                    ChapterNumber = 2,
                    Title = "풀의 나라",
                    IsUnlocked = false,
                    Episodes = new List<StoryEpisode>
                    {
                        new StoryEpisode
                        {
                            EpisodeNumber = 1,
                            Title = "세밀리아 시밀리의 영역",
                            ScriptFile = "Chapter2_Episode1.json",
                            IsUnlocked = false
                        }
                        // 추후 추가 예정
                    }
                },
                new StoryChapter
                {
                    ChapterNumber = 3,
                    Title = "태양의 나라",
                    IsUnlocked = false,
                    Episodes = new List<StoryEpisode>
                    {
                        new StoryEpisode
                        {
                            EpisodeNumber = 1,
                            Title = "그루빗의 시험",
                            ScriptFile = "Chapter3_Episode1.json",
                            IsUnlocked = false
                        }
                    }
                },
                new StoryChapter
                {
                    ChapterNumber = 4,
                    Title = "촛불의 나라",
                    IsUnlocked = false,
                    Episodes = new List<StoryEpisode>
                    {
                        new StoryEpisode
                        {
                            EpisodeNumber = 1,
                            Title = "아세르의 마을",
                            ScriptFile = "Chapter4_Episode1.json",
                            IsUnlocked = false
                        }
                    }
                },
                new StoryChapter
                {
                    ChapterNumber = 5,
                    Title = "태산의 나라",
                    IsUnlocked = false,
                    Episodes = new List<StoryEpisode>
                    {
                        new StoryEpisode
                        {
                            EpisodeNumber = 1,
                            Title = "하츄비의 산",
                            ScriptFile = "Chapter5_Episode1.json",
                            IsUnlocked = false
                        }
                    }
                },
                new StoryChapter
                {
                    ChapterNumber = 6,
                    Title = "정원의 나라",
                    IsUnlocked = false,
                    Episodes = new List<StoryEpisode>
                    {
                        new StoryEpisode
                        {
                            EpisodeNumber = 1,
                            Title = "아츠미의 정원",
                            ScriptFile = "Chapter6_Episode1.json",
                            IsUnlocked = false
                        }
                    }
                },
                new StoryChapter
                {
                    ChapterNumber = 7,
                    Title = "총칼의 나라",
                    IsUnlocked = false,
                    Episodes = new List<StoryEpisode>
                    {
                        new StoryEpisode
                        {
                            EpisodeNumber = 1,
                            Title = "치리프 강의 결투",
                            ScriptFile = "Chapter7_Episode1.json",
                            IsUnlocked = false
                        }
                    }
                },
                new StoryChapter
                {
                    ChapterNumber = 8,
                    Title = "보석의 나라",
                    IsUnlocked = false,
                    Episodes = new List<StoryEpisode>
                    {
                        new StoryEpisode
                        {
                            EpisodeNumber = 1,
                            Title = "하리프의 보석",
                            ScriptFile = "Chapter8_Episode1.json",
                            IsUnlocked = false
                        }
                    }
                },
                new StoryChapter
                {
                    ChapterNumber = 9,
                    Title = "바다의 나라",
                    IsUnlocked = false,
                    Episodes = new List<StoryEpisode>
                    {
                        new StoryEpisode
                        {
                            EpisodeNumber = 1,
                            Title = "헤리쉬의 바다",
                            ScriptFile = "Chapter9_Episode1.json",
                            IsUnlocked = false
                        }
                    }
                },
                new StoryChapter
                {
                    ChapterNumber = 10,
                    Title = "계곡의 나라",
                    IsUnlocked = false,
                    Episodes = new List<StoryEpisode>
                    {
                        new StoryEpisode
                        {
                            EpisodeNumber = 1,
                            Title = "고리프의 계곡",
                            ScriptFile = "Chapter10_Episode1.json",
                            IsUnlocked = false
                        }
                    }
                },
                new StoryChapter
                {
                    ChapterNumber = 11,
                    Title = "초월자의 길",
                    IsUnlocked = false,
                    Episodes = new List<StoryEpisode>
                    {
                        new StoryEpisode
                        {
                            EpisodeNumber = 1,
                            Title = "각성",
                            ScriptFile = "Chapter11_Episode1.json",
                            IsUnlocked = false
                        }
                    }
                },
                new StoryChapter
                {
                    ChapterNumber = 12,
                    Title = "레메게톤의 열쇠",
                    IsUnlocked = false,
                    Episodes = new List<StoryEpisode>
                    {
                        new StoryEpisode
                        {
                            EpisodeNumber = 1,
                            Title = "셀몬과의 최종 결전",
                            ScriptFile = "Chapter12_Episode1.json",
                            IsUnlocked = false
                        }
                    }
                }
            };

            // 챕터 정보 저장
            var chaptersJson = JsonConvert.SerializeObject(chapters, Formatting.Indented);
            File.WriteAllText(Path.Combine(_scriptsFolder, "chapters.json"), chaptersJson);

            Console.WriteLine("챕터 구조가 생성되었습니다. 이제 각 Chapter*_Episode*.json 파일을 직접 작성하세요.");
        }
    }
}