using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Connection.Utils
{
    public static class Helpers
    {
        /// <summary>
        /// 리소스 파일의 전체 경로를 가져옵니다
        /// </summary>
        public static string GetResourcePath(string relativePath)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(basePath, Constants.Paths.RESOURCES_FOLDER, relativePath);
        }

        /// <summary>
        /// 이미지 파일을 로드합니다
        /// </summary>
        public static async Task<BitmapImage> LoadImageAsync(string imagePath)
        {
            try
            {
                if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
                    return null;

                var bitmap = new BitmapImage();

                using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    await Task.Run(() =>
                    {
                        bitmap.BeginInit();
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.StreamSource = stream;
                        bitmap.EndInit();
                    });
                }

                bitmap.Freeze(); // UI 스레드 외부에서 사용 가능하도록
                return bitmap;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"이미지 로드 실패: {imagePath}, 오류: {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// 바이트 크기를 사람이 읽기 쉬운 형태로 변환합니다
        /// </summary>
        public static string FormatBytes(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = bytes;
            int order = 0;

            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }

            return $"{len:0.##} {sizes[order]}";
        }

        /// <summary>
        /// 시간을 사람이 읽기 쉬운 형태로 변환합니다
        /// </summary>
        public static string FormatTimeSpan(TimeSpan timeSpan)
        {
            if (timeSpan.TotalDays >= 1)
                return $"{(int)timeSpan.TotalDays}일 {timeSpan.Hours}시간 {timeSpan.Minutes}분";
            else if (timeSpan.TotalHours >= 1)
                return $"{timeSpan.Hours}시간 {timeSpan.Minutes}분";
            else
                return $"{timeSpan.Minutes}분 {timeSpan.Seconds}초";
        }

        /// <summary>
        /// 안전하게 디렉토리를 생성합니다
        /// </summary>
        public static void EnsureDirectoryExists(string path)
        {
            try
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"디렉토리 생성 실패: {path}, 오류: {ex.Message}");
            }
        }

        /// <summary>
        /// 파일 크기를 가져옵니다
        /// </summary>
        public static long GetFileSize(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    return new FileInfo(filePath).Length;
                }
                return 0;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 디렉토리의 총 크기를 계산합니다
        /// </summary>
        public static long GetDirectorySize(string dirPath)
        {
            try
            {
                if (!Directory.Exists(dirPath))
                    return 0;

                var dirInfo = new DirectoryInfo(dirPath);
                long size = 0;

                // 현재 디렉토리의 파일들
                foreach (var file in dirInfo.GetFiles())
                {
                    size += file.Length;
                }

                // 하위 디렉토리 재귀 계산
                foreach (var dir in dirInfo.GetDirectories())
                {
                    size += GetDirectorySize(dir.FullName);
                }

                return size;
            }
            catch
            {
                return 0;
            }
        }

        /// <summary>
        /// 유효한 파일명인지 확인합니다
        /// </summary>
        public static bool IsValidFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            var invalidChars = Path.GetInvalidFileNameChars();
            return fileName.IndexOfAny(invalidChars) == -1;
        }

        /// <summary>
        /// 문자열을 안전한 파일명으로 변환합니다
        /// </summary>
        public static string MakeSafeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "Untitled";

            var invalidChars = Path.GetInvalidFileNameChars();
            var safeName = fileName;

            foreach (var c in invalidChars)
            {
                safeName = safeName.Replace(c, '_');
            }

            return safeName;
        }
    }
}