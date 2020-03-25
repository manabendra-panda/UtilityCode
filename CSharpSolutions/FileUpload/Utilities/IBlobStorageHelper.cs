using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileUpload.Utilities
{
    public interface IBlobStorageHelper
    {
        Task<(bool,string)> UploadFileToBlobStorageAsync(byte[] fileBytes, string fileName);
    }
}
