using Microsoft.AspNetCore.Mvc;

namespace ClearCare.Controllers
{
    public class UserSwitcherController : Controller
    {
        /// <summary>
        ///   Switches the current user session to the specified user.
        /// </summary>
        /// <param name="userIdentifier">
        ///   The identifier of the user to switch to.
        /// </param>
        /// <returns>
        ///   A redirect to the Home page after setting the user's session
        ///   data. Returns a BadRequest if the userIdentifier is null or
        ///   empty.
        /// </returns>
        [HttpPost]
        public IActionResult SwitchUser(string userIdentifier)
        {
            if (string.IsNullOrEmpty(userIdentifier))
            {
                return BadRequest("UserIdentifier is required");
            }

            switch (userIdentifier)
            {
                case "uuid-doctor-john":
                    HttpContext.Session.SetString("UserID", "uuid-doctor-john");
                    HttpContext.Session.SetString("Name", "Dr. John");
                    HttpContext.Session.SetString("Role", "Doctor");
                    break;

                case "uuid-patient-sara":
                    HttpContext.Session.SetString("UserID", "uuid-patient-sara");
                    HttpContext.Session.SetString("Name", "Sara");
                    HttpContext.Session.SetString("Role", "Patient");
                    break;

                case "uuid-patient-john":
                    HttpContext.Session.SetString("UserID", "uuid-patient-john");
                    HttpContext.Session.SetString("Name", "John");
                    HttpContext.Session.SetString("Role", "Patient");
                    break;

                default:
                    HttpContext.Session.SetString("UserID", "uuid-doctor-john");
                    HttpContext.Session.SetString("Name", "Dr. John");
                    HttpContext.Session.SetString("Role", "Doctor");
                    break;
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
