using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TechMoveCRM.Services
{
    public interface IFileService
    {
        Task<string> SavePdfAsync(IFormFile file, string subfolder);
        bool IsValidPdf(IFormFile file);
        void DeleteFile(string filePath);
    }
}