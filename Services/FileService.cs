using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace TechMoveCRM.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public bool IsValidPdf(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return false;

            // Check file extension (case-insensitive)
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (extension != ".pdf")
                return false;

            // Also check the MIME type for extra security
            if (file.ContentType.ToLowerInvariant() != "application/pdf")
                return false;

            return true;
        }

        public async Task<string> SavePdfAsync(IFormFile file, string subfolder)
        {
            if (!IsValidPdf(file))
                throw new InvalidOperationException("Only PDF files are allowed.");

            // Build save directory: wwwroot/uploads/contracts/
            var uploadDir = Path.Combine(_env.WebRootPath, "uploads", subfolder);
            Directory.CreateDirectory(uploadDir); // Creates if not exists

            // Use a GUID filename to prevent overwrites and path traversal
            var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var fullPath = Path.Combine(uploadDir, uniqueFileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the relative web-accessible path
            return $"/uploads/{subfolder}/{uniqueFileName}";
        }

        public void DeleteFile(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath)) return;

            var fullPath = Path.Combine(_env.WebRootPath, filePath.TrimStart('/'));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}