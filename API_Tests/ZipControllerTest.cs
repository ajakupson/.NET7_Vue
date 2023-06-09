using backend.Controllers;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using Ionic.Zip;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using backend.Models;
using System.IO.Pipelines;
using Moq;
using System.IO;

namespace API_Tests
{
    public class ZipControllerTest
    {
        private readonly ZipService _zipService;
        private readonly CultureService _cultureService;
        private readonly ZipController _zipController;

        public ZipControllerTest() {
            _cultureService = new CultureService();
            _zipService = new ZipService(_cultureService);
            _zipController = new ZipController(_zipService);
        }

        [Fact]
        public void ZipsFolderExists()
        {
            var actionResult = _zipController.GetZipArchives();
            Assert.IsType<OkObjectResult>(actionResult);
        }

        [Fact]
        public void ZipsArchiveDoesntExist()
        {
            var actionResult = _zipController.GetZipContents("ABC");
            Assert.IsType<NotFoundObjectResult>(actionResult);
        }

        [Fact]
        public void CatGameExists()
        {
            var actionResult = _zipController.GetZipContents("CatGame.zip");
            Assert.IsType<OkObjectResult>(actionResult);
        }

        [Fact]
        public async void ValidZip()
        {
            var filePath = @"zips/CatGame.zip";
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var fileContent = new byte[stream.Length];
            await stream.ReadAsync(fileContent, 0, (int)stream.Length);
            var fileName = Path.GetFileName(filePath);
            var file = new FormFile(new MemoryStream(fileContent), 0, fileContent.Length, fileName, fileName);

            var actionResult = (OkObjectResult)_zipController.ValidateZip(file);
            var json = JsonConvert.SerializeObject(actionResult.Value);
            var result = JsonConvert.DeserializeObject<ZipValidationResult>(json);

            Assert.Equal(0, result.errors.Count);

        }

        [Fact]
        public async void InvalidZip()
        {
            var filePath = @"zips/CatGame - Copy.zip";
            using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            var fileContent = new byte[stream.Length];
            await stream.ReadAsync(fileContent, 0, (int)stream.Length);
            var fileName = Path.GetFileName(filePath);
            var file = new FormFile(new MemoryStream(fileContent), 0, fileContent.Length, fileName, fileName);

            var actionResult = (OkObjectResult)_zipController.ValidateZip(file);
            var json = JsonConvert.SerializeObject(actionResult.Value);
            var result = JsonConvert.DeserializeObject<ZipValidationResult>(json);

            Assert.NotEqual(0, result.errors.Count);

        }

        [Fact]
        public void DeleteSuccess()
        {
            var actionResult = _zipController.DeleteZip("CatGame.zip");
            Assert.IsType<OkResult>(actionResult);
        }

        [Fact]
        public void DeleteNotSuccess()
        {
            var actionResult = _zipController.DeleteZip("ABC");
            Assert.IsType<NotFoundObjectResult>(actionResult);
        }

        private class ZipValidationResult
        {
            public List<Node> content { get; set; }
            public List<string> errors { get; set; }
        }
    }
}