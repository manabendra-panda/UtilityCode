using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Threading.Tasks;

namespace FileUpload.Utilities
{
    public class BlobStorageHelper : IBlobStorageHelper
    {
        private readonly ILogger<BlobStorageHelper> _logger;

        public BlobStorageHelper(ILogger<BlobStorageHelper> logger)
        {
            _logger = logger;
        }

        public async Task<(bool, string)> UploadFileToBlobStorageAsync(byte[] fileBytes, string fileName, string containerName, string storageConnection)
        {
            try
            {
                CloudStorageAccount storageacc = CloudStorageAccount.Parse(storageConnection);

                CloudBlobClient blobClient = storageacc.CreateCloudBlobClient();

                CloudBlobContainer container = blobClient.GetContainerReference(containerName);

                await container.CreateIfNotExistsAsync();

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(fileName);

                if (fileBytes != null)
                {
                    _logger.LogInformation($"File upload started: {fileName}");
                    await blockBlob.UploadFromByteArrayAsync(fileBytes, 0, fileBytes.Length);
                }

                return (true, blockBlob.SnapshotQualifiedStorageUri.PrimaryUri.ToString());
            }
            catch (StorageException ex)
            {
                _logger.LogError($"File upload error: {ex.ToString()}");
                return (false, null);
            }
        }
    }
}
