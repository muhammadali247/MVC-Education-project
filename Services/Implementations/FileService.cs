using EduHome.Exceptions;
using EduHome.Services.Interfaces;
using EduHome.Utils;
using F = System.IO;

namespace EduHome.Services.Implementations;

public class FileService : IFileService
{
    public async Task<string> CreateFileAsync(IFormFile file, string path, int maxFileSize, string fileType)
    {
        if (!file.CheckFileType(fileType))
        {
            throw new FileSizeException("Way too big for image size");
        }

        if (file.CheckFileSize(maxFileSize))
        {
            throw new FileTypeException("Not an image");
        }

        string fileName = $"{Guid.NewGuid}-{file.FileName}";
        string resultPath = Path.Combine(path, fileName);
        using (FileStream fileStream = new FileStream(resultPath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }
        return fileName;
    }

    public void DeleteFile(string path)
    {
        if (F.File.Exists(path))
        {
            F.File.Delete(path);
        }
    }
}
