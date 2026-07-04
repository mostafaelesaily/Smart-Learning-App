using Microsoft.Extensions.Logging;
using Smart_Learning_App.Services.IService;

namespace Smart_Learning_App.Services.MainService
{

    namespace Smart_Learning_App.Services.MainService
    {
        public class FileService : IFileService
        {
            private readonly ILogger<FileService> logger;
            public FileService(ILogger<FileService> logger)
            {
                this.logger = logger;
                this.logger.LogInformation("FileService created");
            }
            public async Task<string> UploadFileAsync(IFormFile file, string folderName)
            {
                if (file == null)
                    throw new ArgumentNullException(nameof(file));

                if (string.IsNullOrWhiteSpace(folderName))
                    throw new ArgumentNullException(nameof(folderName));

                var folderPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    folderName
                );

                if (!Directory.Exists(folderPath))
                    Directory.CreateDirectory(folderPath);

                var fileExtension = Path.GetExtension(file.FileName);
                var fileName = $"{Guid.NewGuid()}{fileExtension}";

                var filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                logger.LogInformation("Uploaded file {FileName} to {Path}", file.FileName, filePath);
                return Path.Combine(folderName, fileName);
            }

            public async Task<string> UpdateFileAsync(
                IFormFile file,
                string folderName,
                string existingFilePath)
            {
                if (file == null)
                    throw new ArgumentNullException(nameof(file));

                if (string.IsNullOrWhiteSpace(existingFilePath))
                    throw new ArgumentNullException(nameof(existingFilePath));
                var newFilePath = await UploadFileAsync(file, folderName);
                var fullOldPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    existingFilePath
                );

                if (File.Exists(fullOldPath))
                {
                    File.Delete(fullOldPath);
                    logger.LogInformation("Deleted old file at {OldPath}", fullOldPath);
                }

                return newFilePath;
            }
            public  Task DeleteFileAsync(string filePath)
            {
                if (string.IsNullOrWhiteSpace(filePath))
                    throw new ArgumentNullException(nameof(filePath));

                var fullPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    filePath
                );

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    logger.LogInformation("Deleted file at {FullPath}", fullPath);
                }

                return Task.CompletedTask;
            }
        }
    }
}
