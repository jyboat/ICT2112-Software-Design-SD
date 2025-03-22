using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ClearCare.Models;
using ClearCare.Models.ViewModels.M3T1;
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

    [HttpGet]
    [Route("CreateGroup")]
    public IActionResult displayCreateGroupForm()
    {
        return View("~/Views/M3T1/Community/CreateGroup.cshtml");
    }

    [HttpPost]
    [Route("CreateGroup")]
    public async Task<IActionResult> submitCreateGroup(string userId, string name, string description)
    {
        DateTime creationDate = DateTime.UtcNow;
        await _communityGroup.createGroup(userId, name, creationDate, description);
        return RedirectToAction("Index");
    }

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
        var viewModel = new CommunityViewModel(group, posts);

        return View("~/Views/M3T1/Community/GroupDetail.cshtml", viewModel);
    }

    [HttpPost]
    [Route("Index")]
    public async Task<IActionResult> deleteGroup(string groupId)
    {
        await _communityGroup.deleteGroup(groupId);
        return RedirectToAction("Index");
    }

    [HttpGet]
    [Route("EditGroup")]
    public IActionResult displayEditGroupForm()
    {
        return View("~/Views/M3T1/Community/EditGroup.cshtml");
    }

    [HttpPost]
    [Route("EditGroup")]
    public async Task<IActionResult> editGroup(string groupId, string name, string description)
    {
        await _communityGroup.updateGroup(groupId, name, description);
        return RedirectToAction("Index");
    }

    [HttpGet]
    [Route("CreatePost/{groupId}/{userId}")]
    public IActionResult displayPostForm(string groupId, string userId)
    {
        ViewData["GroupId"] = groupId;
        ViewData["UserId"] = userId;
        return View("~/Views/M3T1/Community/CreatePost.cshtml");
    }

    [HttpPost]
    [Route("CreatePost")]
    public async Task<IActionResult> submitPost(string title, string content, string postedBy, string groupId)
    {
        DateTime postedAt = DateTime.UtcNow;
        await _communityPost.createPost(title, content, postedBy, postedAt, groupId);
        return RedirectToAction("displayGroup");
    }

    [HttpPost]
    [Route("GroupDetail")]
    public async Task<IActionResult> deletePost(string postId)
    {
        await _communityPost.deletePost(postId);
        return RedirectToAction("displayGroup");
    }

    [HttpGet]
    [Route("EditPost")]
    public IActionResult displayEditPostForm()
    {
        return View("~/Views/M3T1/Community/EditGroup.cshtml");
    }

    [HttpPost]
    [Route("EditPost")]
    public async Task<IActionResult> editPost(string postId, string title, string content)
    {
        await _communityPost.updatePost(postId, title, content);
        return RedirectToAction("displayGroup");
    }

    [HttpGet]
    [Route("PostDetail")]
    public async Task<IActionResult> displayPost(string postId)
    {
        var post = await _communityPost.viewPost(postId);
        if (post == null)
        {
            return NotFound();
        }
        var comments = await _communityComment.viewPostComments(postId);
        var viewModel = new CommunityViewModel(post, comments);

        return View("~/Views/M3T1/Community/PostDetail.cshtml", viewModel);
    }

    [HttpPost]
    [Route("PostDetail")]
    public async Task<IActionResult> submitComment(string content, string createdBy, string postId)
    {
        DateTime createdAt = DateTime.UtcNow;
        await _communityComment.createComment(content, createdBy, postId, createdAt);
        return RedirectToAction("displayPost");
    }

    [HttpPost]
    [Route("PostDetail")]
    public async Task<IActionResult> deleteComment(string commentId)
    {
        await _communityComment.deleteComment(commentId);
        return RedirectToAction("displayPost");
    }

    [HttpPost]
    [Route("PostDetail")]
    public async Task<IActionResult> editComment(string userId, string commentId, string content)
    {
        await _communityComment.updateComment(userId, commentId, content);
        return RedirectToAction("displayPost");
    }
}