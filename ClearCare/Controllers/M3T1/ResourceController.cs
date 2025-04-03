using ClearCare.DataSource.M3T1;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.Models.Control.M3T1;
using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Interfaces.M3T1;


[Route("Resource")] // Updated to singular "Resource"
public class ResourceController : Controller
{
    private readonly ResourceManager _manager;


    public ResourceController()
    {
        var gateway = new ResourceGateway();
        _manager = new ResourceManager(gateway);
            gateway.receiver = _manager;

    }

    [Route("")]
    [HttpGet]
    public async Task<IActionResult> List()
    {
        // Fetch resource from db and convert to dictionary
        List<Dictionary<string, object>> resourceList = (await _manager.viewResource())
            .Select(r =>
            {
                var details = r.getDetails();
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

    [Route("Add")]
    [HttpPost]
    public async Task<IActionResult> Add(
    IFormFile coverImage,
    IFormFile? videoFile,
    string title,
    string description,
    string? url,
    string resourceType,
    string uploadedBy)
    {
        byte[] imageFileBytes = Array.Empty<byte>();
        string imageFileName = string.Empty;

        // Handle cover image
        if (coverImage != null && coverImage.Length > 0)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await coverImage.CopyToAsync(memoryStream);
                    imageFileBytes = memoryStream.ToArray();
                }
                imageFileName = coverImage.FileName;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Failed to process cover image.";
                return RedirectToAction("List");
            }
        }

        // Decide what to pass based on resource type
        object? fileOrUrl = resourceType.ToLower() switch
        {
            "video" => videoFile,
            "article" => url,
            _ => null
        };

        if (fileOrUrl == null)
        {
            TempData["ErrorMessage"] = "Missing input for the selected resource type.";
            return RedirectToAction("List");
        }

        try
        {
            await _manager.processResourceWithStrategy(
                title,
                description,
                uploadedBy,
                DateTime.Now.ToString("yyyy-MM-dd"),
                imageFileBytes,
                imageFileName,
                fileOrUrl,
                resourceType
            );

            TempData["SuccessMessage"] = "Resource added successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Failed to add resource: " + ex.Message;
        }

        return RedirectToAction("List");
    }

    [HttpPost]
    [Route("Edit/{resourceId}")]
    public async Task<IActionResult> Edit(
    string resourceId,
    IFormFile? coverImage,
    IFormFile? videoFile,
    string title,
    string description,
    string? url,
    string resourceType,
    string? existingUrl,
    string uploadedBy) 
    {
        byte[] imageFileBytes = Array.Empty<byte>();
        string imageFileName = string.Empty;

        if (coverImage != null && coverImage.Length > 0)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await coverImage.CopyToAsync(memoryStream);
                    imageFileBytes = memoryStream.ToArray();
                }
                imageFileName = coverImage.FileName;
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Failed to process cover image.";
                return RedirectToAction("Edit", new { resourceId });
            }
        }

        // Decide what to use for the URL depending on resource type
        string? finalUrl = null;

        if (resourceType.ToLower() == "video")
        {
            if (videoFile != null && videoFile.Length > 0)
            {
                string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "videos");
                Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(videoFile.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await videoFile.CopyToAsync(stream);
                }

                finalUrl = "/videos/" + uniqueFileName;
            }
            else
            {
                // Use existing video URL if no new file is uploaded
    finalUrl = existingUrl;
            }
        }
        else if (resourceType.ToLower() == "article")
        {
            finalUrl = url;
        }

        try
        {
            await _manager.updateResource(
                resourceId,
                title,
                description,
                uploadedBy, 
                image: imageFileBytes,
                coverImageName: imageFileName,
                url: finalUrl
            );

            TempData["SuccessMessage"] = "Resource updated successfully!";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = "Failed to update resource: " + ex.Message;
            return RedirectToAction("Edit", new { resourceId });
        }

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


