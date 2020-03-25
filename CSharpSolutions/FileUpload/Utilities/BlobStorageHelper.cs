using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Threading.Tasks;

namespace FileUpload.Utilities
{
    public class BlobStorageHelper : IBlobStorageHelper
    {
        private readonly IConfiguration _configuration;
        public BlobStorageHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<(bool, string)> UploadFileToBlobStorageAsync(byte[] fileBytes, string fileName)
        {
            try
            {
                String strorageconn = _configuration["StorageConnectionString"];

                CloudStorageAccount storageacc = CloudStorageAccount.Parse(strorageconn);

                CloudBlobClient blobClient = storageacc.CreateCloudBlobClient();

                CloudBlobContainer container = blobClient.GetContainerReference(_configuration.GetValue<string>("ContainerName"));

                await container.CreateIfNotExistsAsync();

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

                if (fileBytes != null)
                {
                    await blockBlob.UploadFromByteArrayAsync(fileBytes, 0, fileBytes.Length);
                }

                return (true, blockBlob.SnapshotQualifiedStorageUri.PrimaryUri.ToString());
            }
            catch (StorageException)
            {
                return (false, null);
            }
        }
    }
}
