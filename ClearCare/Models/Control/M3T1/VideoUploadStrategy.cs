using ClearCare.DataSource.M3T1;
using ClearCare.Models.Interfaces.M3T1;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using System.Threading.Tasks;

public class VideoUploadStrategy : IResourceStrategy
{
    private readonly ResourceGateway _gateway = new ResourceGateway();

    public async Task<string> uploadAsync(
        string title,
        string description,
        int uploadedBy,
        string dateCreated,
        byte[] image,
        string coverImageName,
        object? fileOrUrl)
    {
            var videoFile = fileOrUrl as IFormFile;

        if (videoFile == null || videoFile.Length == 0)
            throw new ArgumentException("Video file is required.");

        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos");
        Directory.CreateDirectory(uploadsFolder);

        string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(videoFile.FileName);
        string filePath = Path.Combine(uploadsFolder, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await videoFile.CopyToAsync(stream);
        }

        string videoUrl = "/videos/" + uniqueFileName;

        return await _gateway.insertResource(title, description, uploadedBy, dateCreated, image, coverImageName, videoUrl);
    }
}
