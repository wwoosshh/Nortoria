using System;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Connection.Models;

namespace Connection.Services
{
    public class DataService
    {
        private static readonly string AppDataFolder =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Connection");
        private static readonly string UserDataFile = Path.Combine(AppDataFolder, "userdata.json");

        public DataService()
        {
            // 앱 데이터 폴더가 없으면 생성
            if (!Directory.Exists(AppDataFolder))
            {
                Directory.CreateDirectory(AppDataFolder);
            }
        }

        /// <summary>
        /// 유저 데이터를 로드합니다
        /// </summary>
        public async Task<UserData> LoadUserDataAsync()
        {
            try
            {
                if (!File.Exists(UserDataFile))
                {
                    // 첫 실행 시 기본 데이터 생성
                    var newUserData = new UserData();
                    await SaveUserDataAsync(newUserData);
                    return newUserData;
                }

                var jsonContent = await File.ReadAllTextAsync(UserDataFile);
                var userData = JsonConvert.DeserializeObject<UserData>(jsonContent);

                // 데이터 무결성 검사
                if (userData == null)
                {
                    userData = new UserData();
                    await SaveUserDataAsync(userData);
                }

                return userData;
            }
            catch (Exception ex)
            {
                // 오류 발생 시 기본 데이터 반환
                Console.WriteLine($"유저 데이터 로드 실패: {ex.Message}");
                return new UserData();
            }
        }

        /// <summary>
        /// 유저 데이터를 저장합니다
        /// </summary>
        public async Task SaveUserDataAsync(UserData userData)
        {
            try
            {
                userData.LastPlayTime = DateTime.Now;
                var jsonContent = JsonConvert.SerializeObject(userData, Formatting.Indented);
                await File.WriteAllTextAsync(UserDataFile, jsonContent);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"유저 데이터 저장 실패: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 유저 데이터 파일이 존재하는지 확인합니다
        /// </summary>
        public bool HasUserData()
        {
            return File.Exists(UserDataFile) && new FileInfo(UserDataFile).Length > 0;
        }

        /// <summary>
        /// 유저 데이터를 삭제합니다 (게임 초기화)
        /// </summary>
        public async Task DeleteUserDataAsync()
        {
            try
            {
                if (File.Exists(UserDataFile))
                {
                    File.Delete(UserDataFile);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"유저 데이터 삭제 실패: {ex.Message}");
                throw;
            }
        }
    }
}