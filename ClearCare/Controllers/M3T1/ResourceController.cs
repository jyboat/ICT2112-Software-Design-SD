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
            .Select(r => {
                var details = r.GetDetails();
                if (details.ContainsKey("CoverImage") && details["CoverImage"] is byte[] fileBytes && fileBytes.Length > 0)
                {
                    string base64String = Convert.ToBase64String(fileBytes);
                    details["ImageSrc"] = $"data:image/jpeg;base64,{base64String}"; // Adjust MIME type if needed
                }
                else
                {
                    details["ImageSrc"] = "/img/default.jpg"; // Default image
                }
                return details;
            })
            .ToList();

        return View("~/Views/M3T1/Resource/List.cshtml", resourceList);
    }

    [Route("Add")]
    [HttpGet]
    public IActionResult Add()
    {
        return View("~/Views/M3T1/Resource/Add.cshtml");
    }

    //[Route("Add")]
    //[HttpPost]
    //public async Task<IActionResult> Add(
    //IFormFile coverImage,
    //string title,
    //string description,
    //string? articleUrl,
    //IFormFile? videoFile,
    //string resourceType,
    //int uploadedBy)
    //{
    //    // 1. Upload the cover image to Firebase Storage
    //    CoverImageUploadStrategy coverImageUploader = new CoverImageUploadStrategy();
    //    string coverImageUrl = await coverImageUploader.UploadCoverImageAsync(coverImage);

    //    // 2. Choose the correct strategy based on resourceType
    //    IResourceStrategy strategy = resourceType.ToLower() switch
    //    {
    //        "article" => new ArticleUrlStrategy(),
    //        "video" => new VideoUploadStrategy(),
    //        _ => throw new ArgumentException("Unsupported resource type")
    //    };

    //    // 3. Prepare video file (if any)
    //    Stream? fileStream = videoFile?.OpenReadStream();
    //    string? fileName = videoFile?.FileName;
    //    string? contentType = videoFile?.ContentType;

    //    // 4. Upload + Save to Firestore via Strategy (includes using the gateway)
    //    string resourceId = await strategy.UploadAsync(
    //        title,
    //        description,
    //        uploadedBy,
    //        DateTime.Now.ToString("yyyy-MM-dd"),
    //        fileStream,
    //        fileName,
    //        contentType,
    //        articleUrl,
    //        coverImageUrl
    //    );

    //    // 5. Redirect to the resource list view
    //    return RedirectToAction("List");
    //}

    [Route("Add")]
    [HttpPost]
    public async Task<IActionResult> Add(
    IFormFile coverImage,
    string title,
    string description,
    IFormFile resourceFile,
    string? articleUrl,
    IFormFile? videoFile,
    string resourceType,
    int uploadedBy)
    {
        byte[] fileBytes = Array.Empty<byte>();
        string fileName = string.Empty;

        if (resourceFile != null && resourceFile.Length > 0)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await resourceFile.CopyToAsync(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }
                fileName = resourceFile.FileName;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to process image";
                return RedirectToAction("List");
            }
        }

        await _manager.addResource(title, description, uploadedBy, DateTime.Now.ToString("yyyy-MM-dd"), fileBytes, fileName, articleUrl);

        TempData["SuccessMessage"] = "Resource added successfully!";

        return RedirectToAction("List");
    }




    [Route("Edit/{resourceId}")]
    [HttpGet]
    public async Task<IActionResult> Edit(string resourceId)
    {
        if (string.IsNullOrEmpty(resourceId))
        {
            TempData["ErrorMessage"] = "Invalid resource ID.";
            return RedirectToAction("List");
        }

        Resource resource = await _manager.getResource(resourceId);

        if (resource == null)
        {
            TempData["ErrorMessage"] = "Resource not found.";
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
            TempData["ErrorMessage"] = "Failed to delete resource. Please try again.";
        }

        return RedirectToAction("List");
    }

}


