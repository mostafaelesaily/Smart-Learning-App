namespace Smart_Learning_App.Services.IService
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderName);

        Task<string> UpdateFileAsync(IFormFile newFile, string oldFilePath, string folderName);

        Task DeleteFileAsync(string filePath);
    }
}
