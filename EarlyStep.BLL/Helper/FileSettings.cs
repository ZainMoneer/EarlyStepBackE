using Microsoft.AspNetCore.Http;
namespace EarlyStep.BLL.Helper;

public class FileSettings
{

    public static async Task<string?> UploadFileAsync(IFormFile file, string folderName, CancellationToken cancellationToken)
    {
        if (file == null || file.Length == 0) return null;


        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folderName);

        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);


        var filePath = Path.Combine(folderPath, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream, cancellationToken);
        }

        return $"/uploads/{folderName}/{fileName}";
    }


    public static bool RemoveFile(string fileName, string folderName)
    {
        if (string.IsNullOrEmpty(fileName))
            return false;

        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", folderName, fileName);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return true;
        }

        return false;
    }

}
