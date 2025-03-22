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
    public async Task<IActionResult> List()
    {
        string userId = "1"; // Hardcoded user id

        List<CommunityPost> posts = await _communityPost.viewPosts();
        var postList = posts
        .Select(s => s.getDetails())
        .ToList();

        var allGroups = (await _communityGroup.getAllgroups()).Select(s => s.getDetails()).ToList();
        List<Dictionary<string, object>> userGroups = new List<Dictionary<string, object>>();

        foreach (var group in allGroups)
        {
            if (group.ContainsKey("MemberIds") && group["MemberIds"] is List<string> memberList)
            {
                // Check if the MemberList contains the target ID
                if (memberList.Contains(userId))
                {
                    userGroups.Add(group); // Add the group if the ID is found
                }
            }
        }

        var viewModel = new CommunityViewModel
        {
            Posts = postList,
            AllGroups = allGroups,
            UserGroups = userGroups
        };

        return View("~/Views/M3T1/Community/List.cshtml", viewModel);
    }

    [HttpPost]
    [Route("Group/Create")]
    public async Task<IActionResult> submitCreateGroup(string userId, string name, string description)
    {
        List<string> memberIds = new List<string>();
        userId = "1"; // Hardcoded user id
        memberIds.Add(userId);

        string id = await _communityGroup.createGroup(userId, name, description, memberIds);

        if (!string.IsNullOrEmpty(id))
        {
            TempData["SuccessMessage"] = "Group created successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Error in creating group";
        }

        return RedirectToAction("List");
    }

    [HttpGet]
    [Route("Group/{groupId}")]
    public async Task<IActionResult> displayGroup(string groupId)
    {
        // to display group posts, details
        return View("~/Views/M3T1/Community/Group.cshtml");
    }

    //[HttpPost]
    //[Route("Index")]
    //public async Task<IActionResult> deleteGroup(string groupId)
    //{
    //    await _communityGroup.deleteGroup(groupId);
    //    return RedirectToAction("Index");
    //}

    //[HttpGet]
    //[Route("EditGroup")]
    //public IActionResult displayEditGroupForm()
    //{
    //    return View("~/Views/M3T1/Community/EditGroup.cshtml");
    //}

    //[HttpPost]
    //[Route("EditGroup")]
    //public async Task<IActionResult> editGroup(string groupId, string name, string description)
    //{
    //    await _communityGroup.updateGroup(groupId, name, description);
    //    return RedirectToAction("Index");
    //}

    [HttpPost]
    [Route("Post/Create")]
    public async Task<IActionResult> submitPost(string title, string content, string postedBy, string groupId)
    {
        string id = await _communityPost.createPost(title, content, postedBy);
        if (!string.IsNullOrEmpty(id))
        {
            TempData["SuccessMessage"] = "Post created successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Error in creating post";
        }

        return RedirectToAction("List");
    }

    //[HttpPost]
    //[Route("GroupDetail")]
    //public async Task<IActionResult> deletePost(string postId)
    //{
    //    await _communityPost.deletePost(postId);
    //    return RedirectToAction("displayGroup");
    //}

    //[HttpGet]
    //[Route("EditPost")]
    //public IActionResult displayEditPostForm()
    //{
    //    return View("~/Views/M3T1/Community/EditGroup.cshtml");
    //}

    //[HttpPost]
    //[Route("EditPost")]
    //public async Task<IActionResult> editPost(string postId, string title, string content)
    //{
    //    await _communityPost.updatePost(postId, title, content);
    //    return RedirectToAction("displayGroup");
    //}

    [HttpGet]
    [Route("Post/{id}")]
    public async Task<IActionResult> displayPost(string id)
    {
        // To display post details
        var post = await _communityPost.viewPost(id);
        var details = post.getDetails();

        string? postId = details["Id"]?.ToString();
        if (!string.IsNullOrEmpty(postId))
        {
            List<CommunityComment> comments = await _communityComment.viewPostComments(postId);
            var commentsList = comments.Select(s => s.getDetails()).ToList();

            details["Comments"] = commentsList;
        }

        return View("~/Views/M3T1/Community/Index.cshtml", details);
    }

    [HttpPost]
    [Route("Comment/Create")]
    public async Task<IActionResult> submitComment(string content, string createdBy, string postId)
    {
        createdBy = "1"; // Hardcoded user id
        string id = await _communityComment.createComment(content, createdBy, postId);

        if (!string.IsNullOrEmpty(id))
        {
            TempData["SuccessMessage"] = "Comment created successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Error in creating comment";
        }

        return RedirectToAction("displayPost", postId);
    }

    //[HttpPost]
    //[Route("PostDetail")]
    //public async Task<IActionResult> deleteComment(string commentId)
    //{
    //    await _communityComment.deleteComment(commentId);
    //    return RedirectToAction("displayPost");
    //}

    //[HttpPost]
    //[Route("PostDetail")]
    //public async Task<IActionResult> editComment(string userId, string commentId, string content)
    //{
    //    await _communityComment.updateComment(userId, commentId, content);
    //    return RedirectToAction("displayPost");
    //}
}