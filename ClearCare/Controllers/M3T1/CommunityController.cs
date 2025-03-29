using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ClearCare.Models;
using ClearCare.Models.ViewModels.M3T1;
using ClearCare.Models.Entities.M3T1;
using ClearCare.Models.Control.M3T1;
using ClearCare.DataSource.M3T1;

namespace ClearCare.Controllers.M3T1;

[Route("Community")]
public class CommunityController : Controller
{
    private readonly CommunityGroupManagement _communityGroup;
    private readonly CommunityPostManagement _communityPost;
    private readonly CommunityCommentManagement _communityComment;
    public string userId = "1"; // Hardcoded user id

    public CommunityController()
    {
        var mapper = new CommunityDataMapper();
        _communityGroup = new CommunityGroupManagement(mapper);
        _communityPost = new CommunityPostManagement(mapper);
        _communityComment = new CommunityCommentManagement(mapper);
    }

    [HttpGet]
    [Route("")]
    public async Task<IActionResult> list()
    {
        List<CommunityPost> posts = await _communityPost.viewPosts();
        var postList = posts
        .Select(s => s.getDetails())
        .ToList();

        List<CommunityPost> userPosts = await _communityPost.viewUserPosts(userId); // Change to current user id
        var userPostList = userPosts.Select(s => s.getDetails()).ToList();

        var userGroups = (await _communityGroup.getUserGroups(userId)).Select(s => s.getDetails()).ToList();
        var allGroups = (await _communityGroup.getNonUserGroups(userId)).Select(s => s.getDetails()).ToList();

        var viewModel = new CommunityViewModel
        {
            Posts = postList,
            UserPosts = userPostList,
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

        string id = await _communityGroup.createGroup(userId, name, description, memberIds);

        if (!string.IsNullOrEmpty(id))
        {
            TempData["SuccessMessage"] = "Group created successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Error in creating group";
        }

        return RedirectToAction("list");
    }

    [HttpGet]
    [Route("Group/{groupId}")]
    public async Task<IActionResult> displayGroup(string groupId)
    {
        List<CommunityPost> posts = await _communityPost.viewGroupPosts(groupId);
        var postList = posts
        .Select(s => s.getDetails())
        .ToList();

        List<CommunityPost> userPosts = await _communityPost.viewUserGroupPosts(userId, groupId); // Change to current user id
        var userPostList = userPosts.Select(s => s.getDetails()).ToList();

        var userGroups = (await _communityGroup.getUserGroups(userId)).Select(s => s.getDetails()).ToList();
        var allGroups = (await _communityGroup.getNonUserGroups(userId)).Select(s => s.getDetails()).ToList();

        var group = (await _communityGroup.viewGroupById(groupId)).getDetails();
        var members = group["MemberIds"] as List<string>;

        var viewModel = new CommunityViewModel
        {
            Posts = postList,
            UserPosts = userPostList,
            AllGroups = allGroups,
            UserGroups = userGroups,
            Group = group,
            GroupView = true,
            GroupMembers = members
        };

        return View("~/Views/M3T1/Community/List.cshtml", viewModel);
    }

    //[HttpPost]
    //[Route("Index")]
    //public async Task<IActionResult> deleteGroup(string groupId)
    //{
    //    await _communityGroup.deleteGroup(groupId);
    //    return RedirectToAction("Index");
    //}

    [HttpPost]
    [Route("Group/Edit")]
    public async Task<IActionResult> editGroup(string groupId, string name, string description)
    {
        bool success = await _communityGroup.updateGroup(groupId, name, description);
        if (success)
        {
            TempData["SuccessMessage"] = "Group updated successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Error in updating group";
        }

        return RedirectToAction("displayGroup", new { groupId = groupId });
    }

    [HttpPost]
    [Route("Group/Join")]
    public async Task<IActionResult> addGroupMember(string groupId)
    {
        bool success = await _communityGroup.addMember(groupId, userId);
        if (success)
        {
            TempData["SuccessMessage"] = "Joined Group successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Error joining group, please check if you have already joined.";
        }
        return RedirectToAction("list");
    }

    [HttpPost]
    [Route("Group/Member/Remove")]
    public async Task<IActionResult> removeGroupMember(string groupId, List<string> selectedMembers)
    {
        if (selectedMembers == null || !selectedMembers.Any())
        {
            TempData["ErrorMessage"] = "No members selected for removal.";
            return RedirectToAction("displayGroup", new { groupId = groupId });
        }
        bool success = await _communityGroup.removeMember(groupId, selectedMembers);
        if (success)
        {
            TempData["SuccessMessage"] = "Removed member successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Error removing member from group.";
        }
        return RedirectToAction("displayGroup", new { groupId = groupId });
    }

    [HttpPost]
    [Route("Post/Create")]
    public async Task<IActionResult> submitPost(string userId, string title, string content, string? groupId)
    {
        string id = await _communityPost.createPost(title, content, userId, groupId); 
        if (!string.IsNullOrEmpty(id))
        {
            TempData["SuccessMessage"] = "Post created successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Error in creating post";
        }

        if (string.IsNullOrEmpty(groupId))
        {
            return RedirectToAction("list");
        }
        else
        {
            return RedirectToAction("displayGroup", new { groupId = groupId });
        }
    }

    [HttpPost]
    [Route("Post/Delete")]
    public async Task<IActionResult> deletePost(string postId)
    {
        bool success = await _communityPost.deletePost(postId);
        if (success)
        {
            TempData["SuccessMessage"] = "Post deleted successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Error in deleting post";
        }
        return RedirectToAction("list");
    }

    [HttpPost]
    [Route("Post/Edit")]
    public async Task<IActionResult> editPost(string postId, string title, string content)
    {
        bool success = await _communityPost.updatePost(postId, title, content);
        if (success)
        {
            TempData["SuccessMessage"] = "Post edited successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Error in editing post";
        }
        return RedirectToAction("list");
    }

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
    public async Task<IActionResult> submitComment(string content, string userId, string postId)
    {
        string id = await _communityComment.createComment(content, userId, postId);

        if (!string.IsNullOrEmpty(id))
        {
            TempData["SuccessMessage"] = "Comment created successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Error in creating comment";
        }

        return RedirectToAction("displayPost", new { id = postId });
    }

    [HttpPost]
    [Route("Comment/Delete")]
    public async Task<IActionResult> deleteComment(string commentId, string postId)
    {
        bool success = await _communityComment.deleteComment(commentId);
        if (success)
        {
            TempData["SuccessMessage"] = "Comment deleted successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Error in deleting comment";
        }
        return RedirectToAction("displayPost", new { id = postId });
    }

    [HttpPost]
    [Route("Comment/Edit")]
    public async Task<IActionResult> editComment(string commentId, string content, string postId)
    {
        bool success = await _communityComment.updateComment(commentId, content);

        if (success)
        {
            TempData["SuccessMessage"] = "Comment edited successfully!";
        }
        else
        {
            TempData["ErrorMessage"] = "Error in editing comment";
        }

        return RedirectToAction("displayPost", new { id = postId });
    }
}