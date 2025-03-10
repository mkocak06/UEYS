using System.IO;
using System.Threading.Tasks;

namespace UI.Services;

public interface IFileUploadService
{
    Task<string> PostImage(Stream fileStream, string fileName);
    Task<string> PostImageByte(Stream fileStream, string fileName, long userId);
}

public class FileUploadService : IFileUploadService
{
    private readonly IHttpService _httpService;

    public FileUploadService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<string> PostImage(Stream fileStream, string fileName)
    {
        return await _httpService.PostImage(fileStream, fileName);
    }
    public async Task<string> PostImageByte(Stream fileStream, string fileName, long userId)
    {
        return await _httpService.PostImageByte(fileStream, fileName, userId);
    }
}