using ClearCare.DataSource.M3T1;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClearCare.Models.Control.M3T1;
using ClearCare.Models.Entities.M3T1;

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
    public async Task<IActionResult> Add(string title, string description, int uploadedBy)
    {
        if (string.IsNullOrEmpty(title) || string.IsNullOrEmpty(description) || uploadedBy < 1)
        {
            ViewBag.ErrorMessage = "Please fill in all required fields";
            return View("~/Views/M3T1/Resource/Add.cshtml");
        }

        string dateCreated = DateTime.Now.ToString("yyyy-MM-dd");
        string id = await _manager.addResource(title, description, uploadedBy, dateCreated);

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
}
