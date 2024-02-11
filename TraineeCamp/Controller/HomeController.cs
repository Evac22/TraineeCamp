using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using System.Diagnostics;
using TraineeCamp.Models;

namespace TraineeCamp.Controllers
{
    public class HomeController : Microsoft.AspNetCore.Mvc.Controller

    {
        private readonly IHostEnvironment _environment;
        private readonly IConfiguration _configuration;
        private readonly string _blobConnectionString;
        private readonly string _blobContainerName;

        public HomeController(IHostEnvironment environment, IConfiguration configuration)
        {
            _environment = environment;
            _configuration = configuration;
            _blobConnectionString = _configuration.GetConnectionString("BlobConnectionString");
            _blobContainerName = _configuration.GetValue<string>("BlobContainerName");
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile(FileEmailModel model)
        {
            string connectionString = _blobConnectionString;

            if (model.UploadedFile != null && model.UploadedFile.Length > 0)
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connectionString);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer container = blobClient.GetContainerReference(_blobContainerName);
                container.CreateIfNotExistsAsync();

                CloudBlockBlob blob = container.GetBlockBlobReference(Path.GetFileName(model.UploadedFile.FileName));
                await using (var fileStream = model.UploadedFile.OpenReadStream())
                {
                    blob.Metadata.Add("email", model.Email);
                    await blob.UploadFromStreamAsync(fileStream);
                }
            }

            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
