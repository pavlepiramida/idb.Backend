using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using System;

namespace idb.Backend.Storage
{
    public interface IImageStorage
    {
        (string uploadUrl, string imageUrl) GetImageUrls(string fileName);
    }
    public class AzureImageStorage : IImageStorage
    {
        private readonly BlobContainerClient _blobContainerClient;

        public AzureImageStorage(BlobServiceClient blobServiceClient)
        {
            var imageContainerName = Environment.GetEnvironmentVariable("AzureImageContainer");
            _blobContainerClient = blobServiceClient.GetBlobContainerClient(imageContainerName);
        }

        public (string uploadUrl, string imageUrl) GetImageUrls(string fileName)
        {
            var blob = _blobContainerClient.GetBlobClient(fileName);
            var uploadUrl = blob.GenerateSasUri(BlobSasPermissions.All, DateTime.Now.AddMinutes(1))
                .ToString();
            var imageUrl = blob.Uri.ToString();

            return (uploadUrl, imageUrl);
        }
    }
}
