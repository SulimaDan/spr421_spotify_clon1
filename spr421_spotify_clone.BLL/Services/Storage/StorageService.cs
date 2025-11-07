using Microsoft.AspNetCore.Http;

namespace spr421_spotify_clone.BLL.Services.Storage
{
    public class StorageService : IStorageService
    {
        public async Task<string?> SaveAudioFileAsync(IFormFile audioFile, string folderPath)
        {
            return await SaveFileAsync(audioFile, folderPath, "audio");
        }

        public async Task<string?> SaveImageFileAsync(IFormFile imageFile, string folderPath)
        {
            return await SaveFileAsync(imageFile, folderPath, "image");
        }

        private async Task<string?> SaveFileAsync(IFormFile file, string folderPath, string fileType)
        {
            var types = file.ContentType.Split("/");

            if (types.Length != 2 || types[0] != fileType)
            {
                return null;
            }

            string extenstion = Path.GetExtension(file.FileName);
            string fileName = $"{Guid.NewGuid().ToString()}{extenstion}";
            string filePath = Path.Combine(folderPath, fileName);

            using (var fileStream = File.Create(filePath))
            {
                await file.CopyToAsync(fileStream);
            }

            return fileName;
        }
    }
}
