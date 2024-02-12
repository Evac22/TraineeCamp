using Microsoft.AspNetCore.Mvc;
using TraineeCamp.Models;
using TraineeCamp.Services;

namespace TraineeCamp.Controller
{
    public class FileController : ControllerBase
    {
       private readonly IFileStorageService _fileStorageService;

        public FileController(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        [HttpPost]
        [Route("upload")]
        public async Task<IActionResult> Upload([FromForm] FileEmailModel file)
        {
            if (file.UploadedFile.FileName.EndsWith(".docx") && file.UploadedFile.Length > 0)
            {
                await _fileStorageService.SaveFileAsync(file);
                return Ok("success");
            }
            else
            {
                return BadRequest("You can only upload .docx files");
            }
        }

        [HttpGet]
        [Route("get")]
        public async Task<IActionResult> Get(string fileName)
        {
            var docxFileStream = await _fileStorageService.GetFileStreamAsync(fileName);
            return File(docxFileStream, ContentTypes.Application.Docx, $"{fileName}.docx");
        }

        [HttpGet]
        [Route("download")]
        public async Task<IActionResult> Download(string fileName)
        {
            var docxFileStream = await _fileStorageService.GetFileStreamAsync(fileName);
            return File(docxFileStream, ContentTypes.Application.Docx, $"{fileName}.docx");
        }

    }
}
