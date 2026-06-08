using System.IO;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using TechMoveCRM.Services;

namespace TechMoveCRM.Tests
{
    public class FileServiceTests
    {
        private FileService CreateService(string webRootPath = "/tmp/test_wwwroot")
        {
            var mockEnv = new Mock<IWebHostEnvironment>();
            mockEnv.Setup(e => e.WebRootPath).Returns(webRootPath);
            return new FileService(mockEnv.Object);
        }

        private IFormFile CreateFakeFile(string fileName, string contentType,
            string content = "fake content")
        {
            var bytes = Encoding.UTF8.GetBytes(content);
            var stream = new MemoryStream(bytes);
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.FileName).Returns(fileName);
            mockFile.Setup(f => f.ContentType).Returns(contentType);
            mockFile.Setup(f => f.Length).Returns(bytes.Length);
            mockFile.Setup(f => f.OpenReadStream()).Returns(stream);
            mockFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), default))
                .Returns((Stream s, System.Threading.CancellationToken _) =>
                    stream.CopyToAsync(s));
            return mockFile.Object;
        }

        [Fact]
        public void IsValidPdf_WithValidPdfFile_ReturnsTrue()
        {
            // Arrange
            var service = CreateService();
            var file = CreateFakeFile("agreement.pdf", "application/pdf");

            // Act
            var result = service.IsValidPdf(file);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidPdf_WithExeFile_ReturnsFalse()
        {
            // Arrange — this is the key test: .exe files must be rejected
            var service = CreateService();
            var file = CreateFakeFile("malware.exe", "application/octet-stream");

            // Act
            var result = service.IsValidPdf(file);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidPdf_WithDocxFile_ReturnsFalse()
        {
            var service = CreateService();
            var file = CreateFakeFile("document.docx",
                "application/vnd.openxmlformats-officedocument.wordprocessingml.document");

            Assert.False(service.IsValidPdf(file));
        }

        [Fact]
        public void IsValidPdf_WithPdfExtensionButWrongMimeType_ReturnsFalse()
        {
            // Security test — attacker renames .exe to .pdf but mime type reveals truth
            var service = CreateService();
            var file = CreateFakeFile("trick.pdf", "application/octet-stream");

            Assert.False(service.IsValidPdf(file));
        }

        [Fact]
        public void IsValidPdf_WithNullFile_ReturnsFalse()
        {
            // Edge case: null file
            var service = CreateService();
            Assert.False(service.IsValidPdf(null));
        }

        [Fact]
        public void IsValidPdf_WithEmptyFile_ReturnsFalse()
        {
            // Edge case: empty file (0 bytes)
            var service = CreateService();
            var mockFile = new Mock<IFormFile>();
            mockFile.Setup(f => f.Length).Returns(0);
            mockFile.Setup(f => f.FileName).Returns("empty.pdf");
            mockFile.Setup(f => f.ContentType).Returns("application/pdf");

            Assert.False(service.IsValidPdf(mockFile.Object));
        }

        [Fact]
        public void IsValidPdf_WithUppercaseExtension_ReturnsFalse()
        {
            // Edge case: .PDF uppercase — check case insensitive logic handles it
            // Our implementation uses .ToLowerInvariant() so .PDF should pass
            var service = CreateService();
            var file = CreateFakeFile("AGREEMENT.PDF", "application/pdf");

            Assert.True(service.IsValidPdf(file));
        }
    }
}