using Microsoft.AspNetCore.Http;

namespace AI_Marketplace.Application.Common.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folderName);
        Task DeleteFileAsync(string filePath);
    }
}
