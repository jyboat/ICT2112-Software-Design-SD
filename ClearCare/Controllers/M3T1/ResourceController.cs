using ClearCare.DataSource.M3T1;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.Models.Control.M3T1;
using ClearCare.Models.Entities.M3T1;
using ClearCare.Interfaces.M3T1;


[Route("Resource")] // Updated to singular "Resource"
public class ResourceController : Controller
{
    private readonly ResourceManager _manager;

    public ResourceController()
    {
        _manager = new ResourceManager();
    }

    [Route("")]
    [HttpGet]
    public async Task<IActionResult> List()
    {
        List<Dictionary<string, object>> resourceList = (await _manager.viewResource())
            .Select(r => r.GetDetails())
            .ToList();

        return View("~/Views/M3T1/Resource/List.cshtml", resourceList);
    }

    [Route("Add")]
    [HttpGet]
    public IActionResult Add()
    {
        return View("~/Views/M3T1/Resource/Add.cshtml");
    }

    [Route("Add")]
    [HttpPost]
    public async Task<IActionResult> Add(
    IFormFile coverImage,
    string title,
    string description,
    string? articleUrl,
    IFormFile? videoFile,
    string resourceType,
    int uploadedBy)
    {
        // 1. Upload the cover image to Firebase Storage
        string coverImageUrl = await UploadCoverImageAsync(coverImage);

        // 2. Choose the correct strategy based on resourceType
        IResourceStrategy strategy = resourceType.ToLower() switch
        {
            "article" => new ArticleUrlStrategy(),
            "video" => new VideoUploadStrategy(),
            _ => throw new ArgumentException("Unsupported resource type")
        };

        // 3. Prepare video file (if any)
        Stream? fileStream = videoFile?.OpenReadStream();
        string? fileName = videoFile?.FileName;
        string? contentType = videoFile?.ContentType;

        // 4. Upload + Save to Firestore via Strategy (includes using the gateway)
        string resourceId = await strategy.UploadAsync(
            title,
            description,
            uploadedBy,
            DateTime.Now.ToString("yyyy-MM-dd"),
            fileStream,
            fileName,
            contentType,
            articleUrl,
            coverImageUrl
        );

        // 5. Redirect to the resource list view
        return RedirectToAction("List");
    }




    [Route("Edit/{resourceId}")]
    [HttpGet]
    public async Task<IActionResult> Edit(string resourceId)
    {
        if (string.IsNullOrEmpty(resourceId))
        {
            ViewBag.ErrorMessage = "Invalid resource ID.";
            return RedirectToAction("List");
        }

        Resource resource = await _manager.getResource(resourceId);

        if (resource == null)
        {
            ViewBag.ErrorMessage = "Resource not found.";
            return RedirectToAction("List");
        }

        return View("~/Views/M3T1/Resource/Edit.cshtml", resource); // Ensure the model is passed
    }


    [Route("Delete/{resourceId}")]
    [HttpPost]
    public async Task<IActionResult> Delete(string resourceId)
    {
        bool success = await _manager.deleteResource(resourceId);

        if (!success)
        {
            ViewBag.ErrorMessage = "Failed to delete resource. Please try again.";
        }

        return RedirectToAction("List");
    }


    private async Task<string> UploadCoverImageAsync(IFormFile coverImage)
    {
        if (coverImage == null || coverImage.Length == 0)
            throw new ArgumentException("Cover image is required.");

        var storage = Google.Cloud.Storage.V1.StorageClient.Create();
        string bucketName = "your-bucket.appspot.com";  // Replace with actual Firebase Storage bucket
        string objectName = $"{Guid.NewGuid()}{Path.GetExtension(coverImage.FileName)}";

        using var stream = coverImage.OpenReadStream();
        await storage.UploadObjectAsync(
            bucketName,
            objectName,
            coverImage.ContentType,
            stream,
            new Google.Cloud.Storage.V1.UploadObjectOptions { PredefinedAcl = Google.Cloud.Storage.V1.PredefinedObjectAcl.PublicRead }
        );

        return $"https://storage.googleapis.com/{bucketName}/{objectName}";
    }

}


