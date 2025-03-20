using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ClearCare.Models;
using ClearCare.Models.Control;
using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Control.M3T1;

namespace ClearCare.Controllers.M3T1;

[Route("Community")]
public class CommunityController : Controller
{
    private readonly CommunityGroupManagement _communityGroup = new CommunityGroupManagement();
    private readonly CommunityPostManagement _communityPost = new CommunityPostManagement();
    private readonly CommunityCommentManagement _communityComment = new CommunityCommentManagement();

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> Index()
    {
        var groups = await _communityGroup.viewGroups();
        return View("~/Views/M3T1/Community/Index.cshtml", groups);
    }

    // Route: /Community/CreateGroup (GET)
    [HttpGet]
    [Route("CreateGroup")]
    public IActionResult displayCreateGroupForm()
    {
        return View("~/Views/M3T1/Community/CreateGroup.cshtml");
    }

    // Route: /Community/CreateGroup (POST)
    [HttpPost]
    [Route("CreateGroup")]
    public async Task<IActionResult> submitCreateGroup(string userId, string name, string description)
    {
        DateTime creationDate = DateTime.UtcNow;
        await _communityGroup.createGroup(userId, name, creationDate, description);
        return RedirectToAction("Index");
    }

    // Display group details with all posts
    [HttpGet]
    [Route("GroupDetail")]
    public async Task<IActionResult> displayGroup(string groupId)
    {
        var group = await _communityGroup.viewGroupById(groupId);
        if (group == null)
        {
            return NotFound();
        }
        var posts = await _communityPost.viewGroupPosts(groupId);
        var viewModel = new CommunityGroupViewModel(group, posts);

        return View("~/Views/M3T1/Community/GroupDetail.cshtml", viewModel);
    }

    // Route: /Community/CreatePost (GET)
    [HttpGet]
    [Route("CreatePost/{groupId}/{userId}")]
    public IActionResult displayPostForm(string groupId, string userId)
    {
        ViewData["GroupId"] = groupId;
        ViewData["UserId"] = userId;
        return View("~/Views/M3T1/Community/CreatePost.cshtml");
    }

    // Route: /Community/CreatePost (POST)
    [HttpPost]
    [Route("CreatePost")]
    public async Task<IActionResult> submitPost(string title, string content, string postedBy, string groupId)
    {
        DateTime postedAt = DateTime.UtcNow;
        await _communityPost.createPost(title, content, postedBy, postedAt, groupId);
        return RedirectToAction("displayGroup");
    }
}