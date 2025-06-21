using System.Collections.Generic;
using Connection.Models;

namespace Connection.Utils
{
    public static class LocalizationHelper
    {
        private static readonly Dictionary<Language, Dictionary<string, string>> _localizedStrings
            = new Dictionary<Language, Dictionary<string, string>>();

        static LocalizationHelper()
        {
            InitializeLocalizedStrings();
        }

        private static void InitializeLocalizedStrings()
        {
            // 한국어
            _localizedStrings[Language.Korean] = new Dictionary<string, string>
            {
                { "NewGame", "게임 시작" },
                { "Continue", "이어하기" },
                { "Settings", "옵션" },
                { "Exit", "게임 종료" },
                { "Save", "저장" },
                { "Cancel", "취소" },
                { "Load", "불러오기" },
                { "Resume", "게임으로 돌아가기" },
                { "ReturnToTitle", "타이틀로 돌아가기" },
                
                // 설정창 탭
                { "Graphics", "그래픽" },
                { "Controls", "조작" },
                { "Audio", "음향" },
                { "Language", "언어" },
                { "Resources", "리소스" },
                { "Misc", "기타" },
                
                // 그래픽 설정
                { "DisplayMode", "디스플레이 모드" },
                { "FrameRate", "프레임레이트" },
                { "GraphicsQuality", "그래픽 품질" },
                { "WindowSize", "창 크기" },
                
                // 조작 설정
                { "KeySettings", "키 설정" },
                { "Change", "변경" },
                
                // 음향 설정
                { "MasterVolume", "전체 음향" },
                { "Music", "음악" },
                { "Voice", "음성" },
                { "Effects", "효과음" },
                { "Mute", "음소거" },
                
                // 언어 설정
                { "GameLanguage", "게임 언어" },
                { "VoiceLanguage", "음성 언어" },
                
                // 리소스 설정
                { "ResourceManagement", "리소스 관리" },
                { "AutoDownload", "자동 다운로드" },
                { "AutoDeleteOldResources", "이전 스토리 리소스 자동 삭제" },
                { "DownloadQuality", "다운로드 품질" },
                { "CacheSizeLimit", "캐시 크기 제한" },
                { "ClearCache", "캐시 삭제" },
                { "RedownloadResources", "리소스 재다운로드" },
                
                // 기타 설정
                { "GameInfo", "게임 정보" },
                { "Developer", "개발" },
                { "Engine", "엔진" },
                { "CopyrightInfo", "저작권 정보" },
                { "ResetToDefault", "기본 설정으로 초기화" },

                { "GameCompleted", "게임이 완료되었습니다!" },
                { "EpisodeCompleted", "에피소드가 완료되었습니다!" },
                { "AutoSaved", "자동으로 저장되었습니다." },
                { "GameSaved", "게임이 저장되었습니다." },
                { "GameLoaded", "게임을 불러왔습니다." },
                { "SettingsSaved", "설정이 저장되었습니다." },
                { "Error", "오류" },
                { "Warning", "경고" },
                { "Information", "알림" },
                { "Confirm", "확인" },
                { "ConfirmNewGame", "기존 게임 데이터가 존재합니다. 정말로 새 게임을 시작하시겠습니까?\n기존 진행 상황이 모두 삭제됩니다." },
                { "ConfirmExit", "정말로 게임을 종료하시겠습니까?" },
                { "NoSaveData", "계속할 게임 데이터가 없습니다." },
                { "LastPlay", "마지막 플레이" }
            };

            // 영어
            _localizedStrings[Language.English] = new Dictionary<string, string>
            {
                { "NewGame", "New Game" },
                { "Continue", "Continue" },
                { "Settings", "Settings" },
                { "Exit", "Exit" },
                { "Save", "Save" },
                { "Cancel", "Cancel" },
                { "Load", "Load" },
                { "Resume", "Resume Game" },
                { "ReturnToTitle", "Return to Title" },
                
                // 설정창 탭
                { "Graphics", "Graphics" },
                { "Controls", "Controls" },
                { "Audio", "Audio" },
                { "Language", "Language" },
                { "Resources", "Resources" },
                { "Misc", "Misc" },
                
                // 그래픽 설정
                { "DisplayMode", "Display Mode" },
                { "FrameRate", "Frame Rate" },
                { "GraphicsQuality", "Graphics Quality" },
                { "WindowSize", "Window Size" },
                
                // 조작 설정
                { "KeySettings", "Key Settings" },
                { "Change", "Change" },
                
                // 음향 설정
                { "MasterVolume", "Master Volume" },
                { "Music", "Music" },
                { "Voice", "Voice" },
                { "Effects", "Effects" },
                { "Mute", "Mute" },
                
                // 언어 설정
                { "GameLanguage", "Game Language" },
                { "VoiceLanguage", "Voice Language" },
                
                // 리소스 설정
                { "ResourceManagement", "Resource Management" },
                { "AutoDownload", "Auto Download" },
                { "AutoDeleteOldResources", "Auto Delete Old Story Resources" },
                { "DownloadQuality", "Download Quality" },
                { "CacheSizeLimit", "Cache Size Limit" },
                { "ClearCache", "Clear Cache" },
                { "RedownloadResources", "Redownload Resources" },
                
                // 기타 설정
                { "GameInfo", "Game Information" },
                { "Developer", "Developer" },
                { "Engine", "Engine" },
                { "CopyrightInfo", "Copyright Information" },
                { "ResetToDefault", "Reset to Default" },

                { "GameCompleted", "Game Completed!" },
                { "EpisodeCompleted", "Episode Completed!" },
                { "AutoSaved", "Auto saved." },
                { "GameSaved", "Game saved." },
                { "GameLoaded", "Game loaded." },
                { "SettingsSaved", "Settings saved." },
                { "Error", "Error" },
                { "Warning", "Warning" },
                { "Information", "Information" },
                { "Confirm", "Confirm" },
                { "ConfirmNewGame", "Existing game data found. Do you really want to start a new game?\nAll existing progress will be deleted." },
                { "ConfirmExit", "Do you really want to exit the game?" },
                { "NoSaveData", "No save data to continue." },
                { "LastPlay", "Last Play" }
            };

            // 일본어
            _localizedStrings[Language.Japanese] = new Dictionary<string, string>
            {
                { "NewGame", "ニューゲーム" },
                { "Continue", "コンティニュー" },
                { "Settings", "設定" },
                { "Exit", "終了" },
                { "Save", "セーブ" },
                { "Cancel", "キャンセル" },
                { "Load", "ロード" },
                { "Resume", "ゲームに戻る" },
                { "ReturnToTitle", "タイトルに戻る" },
                
                // 설정창 탭
                { "Graphics", "グラフィック" },
                { "Controls", "操作" },
                { "Audio", "音響" },
                { "Language", "言語" },
                { "Resources", "リソース" },
                { "Misc", "その他" },
                
                // 그래픽 설정
                { "DisplayMode", "ディスプレイモード" },
                { "FrameRate", "フレームレート" },
                { "GraphicsQuality", "グラフィック品質" },
                { "WindowSize", "ウィンドウサイズ" },
                
                // 조작 설정
                { "KeySettings", "キー設定" },
                { "Change", "変更" },
                
                // 음향 설정
                { "MasterVolume", "マスターボリューム" },
                { "Music", "音楽" },
                { "Voice", "音声" },
                { "Effects", "効果音" },
                { "Mute", "ミュート" },
                
                // 언어 설정
                { "GameLanguage", "ゲーム言語" },
                { "VoiceLanguage", "音声言語" },
                
                // 리소스 설정
                { "ResourceManagement", "リソース管理" },
                { "AutoDownload", "自動ダウンロード" },
                { "AutoDeleteOldResources", "古いストーリーリソースの自動削除" },
                { "DownloadQuality", "ダウンロード品質" },
                { "CacheSizeLimit", "キャッシュサイズ制限" },
                { "ClearCache", "キャッシュクリア" },
                { "RedownloadResources", "リソース再ダウンロード" },
                
                // 기타 설정
                { "GameInfo", "ゲーム情報" },
                { "Developer", "開発" },
                { "Engine", "エンジン" },
                { "CopyrightInfo", "著作権情報" },
                { "ResetToDefault", "デフォルトにリセット" },

                { "GameCompleted", "ゲーム完了！" },
                { "EpisodeCompleted", "エピソード完了！" },
                { "AutoSaved", "自動保存されました。" },
                { "GameSaved", "ゲームが保存されました。" },
                { "GameLoaded", "ゲームをロードしました。" },
                { "SettingsSaved", "設定が保存されました。" },
                { "Error", "エラー" },
                { "Warning", "警告" },
                { "Information", "情報" },
                { "Confirm", "確認" },
                { "ConfirmNewGame", "既存のゲームデータが見つかりました。本当に新しいゲームを始めますか？\n既存の進行状況はすべて削除されます。" },
                { "ConfirmExit", "本当にゲームを終了しますか？" },
                { "NoSaveData", "続行するセーブデータがありません。" },
                { "LastPlay", "最後のプレイ" }
            };
        }

        /// <summary>
        /// 지정된 언어로 현지화된 문자열을 가져옵니다
        /// </summary>
        public static string GetLocalizedString(string key, Language language)
        {
            if (_localizedStrings.TryGetValue(language, out var languageDict))
            {
                if (languageDict.TryGetValue(key, out var localizedString))
                {
                    return localizedString;
                }
            }

            // 기본값으로 한국어 반환
            if (_localizedStrings.TryGetValue(Language.Korean, out var koreanDict))
            {
                if (koreanDict.TryGetValue(key, out var koreanString))
                {
                    return koreanString;
                }
            }

            // 키를 찾을 수 없는 경우 키 자체를 반환
            return key;
        }

        /// <summary>
        /// 지정된 언어로 현지화된 문자열을 가져옵니다 (기본값 포함)
        /// </summary>
        public static string GetLocalizedString(string key, Language language, string defaultValue)
        {
            var result = GetLocalizedString(key, language);
            return result == key ? defaultValue : result;
        }
    }
}