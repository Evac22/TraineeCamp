using Azure.Storage.Blobs;
using TraineeCamp.Models;

namespace TraineeCamp.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;

        public FileStorageService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }

        public async Task SaveFile(FileEmailModel file)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient("myProjectContainer");
            var blob = blobContainer.GetBlobClient(file.UploadedFile.FileName);

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "email", file.Email }
            };

            await blob.UploadAsync(file.UploadedFile.OpenReadStream());
            blob.SetMetadataAsync(data);
        }

        public async Task<Stream> GetFileStream(string fileName)
        {
            var blobContainer = _blobServiceClient.GetBlobContainerClient("myProjectContainer");
            var blob = blobContainer.GetBlobClient(fileName);
            var downloadContent = await blob.DownloadAsync();

            return downloadContent.Value.Content;
        }
    }
}
