using Microsoft.AspNetCore.Mvc;

namespace ClearCare.Controllers
{
    public class UserSwitcherController : Controller
    {
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