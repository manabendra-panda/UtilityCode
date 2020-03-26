using FileUpload.Models;
using FileUpload.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FileUpload.Controllers
{
    public class FileUploadController : Controller
    {
        private readonly IBlobStorageHelper _blobStorageHelper;
        private readonly ILogger<FileUploadController> _logger;
        private readonly IConfiguration _configuration;
        public FileUploadController(IBlobStorageHelper blobStorageHelper, ILogger<FileUploadController> logger, IConfiguration configuration)
        {
            _blobStorageHelper = blobStorageHelper;
            _logger = logger;
            _configuration = configuration;
        }
        [HttpPost("UploadFiles")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            var uploadSuccess = false;
            List<string> uploadedUriList = new List<string>();
            var strorageConnection = _configuration["StorageConnectionString"];
            var containerName = _configuration.GetValue<string>("ContainerName");

            foreach (var formFile in files)
            {
                using (var ms = new MemoryStream())
                {
                    _logger.LogInformation($"File Posted {formFile.FileName}");

                    formFile.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    string uploadedUri = null;
                    (uploadSuccess, uploadedUri) = await _blobStorageHelper.UploadFileToBlobStorageAsync(fileBytes, Path.GetFileName(formFile.FileName), containerName, strorageConnection);
                    if (uploadedUri != null)
                    {
                        uploadedUriList.Add(uploadedUri);
                    }

                    _logger.LogInformation($"File UploadUri {uploadedUri}");
                }
            }
            if (uploadSuccess)
                return View("UploadSuccess",uploadedUriList);
            else
                return View("UploadError");

        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}