namespace EduHome.Utils;

public static class Extensions
{
    public static bool CheckFileSize(this IFormFile file, double fileSize) => file.Length / 1024 > fileSize;
    public static bool CheckFileType(this IFormFile file, string fileType) => file.ContentType.Contains(fileType);
}
