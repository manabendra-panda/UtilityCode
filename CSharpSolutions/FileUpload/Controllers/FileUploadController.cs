using FileUpload.Models;
using FileUpload.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        public FileUploadController(IBlobStorageHelper blobStorageHelper)
        {
            _blobStorageHelper = blobStorageHelper;
        }
        [HttpPost("UploadFiles")]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Post(List<IFormFile> files)
        {
            var uploadSuccess = false;
            string uploadedUri = null;

            foreach (var formFile in files)
            {
                using (var ms=new MemoryStream())
                {
                    formFile.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                   (uploadSuccess,uploadedUri)= await _blobStorageHelper.UploadFileToBlobStorageAsync(fileBytes, Path.GetFileName(formFile.FileName));
                    TempData["UploadUri"] = uploadedUri;
                }
            }
            if (uploadSuccess)
                return View("UploadSuccess");
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