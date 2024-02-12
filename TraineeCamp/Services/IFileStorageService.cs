using TraineeCamp.Models;

namespace TraineeCamp.Services
{
    public interface IFileStorageService
    {
        Task SaveFileAsync(FileEmailModel file);

        Task<Stream> GetFileStreamAsync(string fileName);
    }
}
