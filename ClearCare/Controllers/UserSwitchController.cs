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
                    HttpContext.Session.SetString("UserUUID", "uuid-doctor-john");
                    HttpContext.Session.SetString("UserName", "Dr. John");
                    HttpContext.Session.SetString("UserRole", "Doctor");
                    break;

                case "uuid-patient-sara":
                    HttpContext.Session.SetString("UserUUID", "uuid-patient-sara");
                    HttpContext.Session.SetString("UserName", "Sara");
                    HttpContext.Session.SetString("UserRole", "Patient");
                    break;

                case "uuid-patient-john":
                    HttpContext.Session.SetString("UserUUID", "uuid-patient-john");
                    HttpContext.Session.SetString("UserName", "John");
                    HttpContext.Session.SetString("UserRole", "Patient");
                    break;

                default:
                    HttpContext.Session.SetString("UserUUID", "uuid-doctor-john");
                    HttpContext.Session.SetString("UserName", "Dr. John");
                    HttpContext.Session.SetString("UserRole", "Doctor");
                    break;
            }

            return RedirectToAction("Index", "Home");
        }

    }

}