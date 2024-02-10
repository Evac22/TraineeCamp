using TraineeCamp.Models;

namespace TraineeCamp.Services
{
    public interface IFileStorageService
    {
        Task SaveFile(FileEmailModel file);

        Task<Stream> GetFileStream(string fileName);
    }
}
