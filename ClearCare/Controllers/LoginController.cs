using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Entities;
using ClearCare.Models.Control;
using System.Threading.Tasks;

namespace ClearCare.Controllers
{
    public class LoginController : Controller
    {
        private readonly LoginManagement LoginManagement;
        private readonly EmailService emailService;

        public LoginController()
        {
            LoginManagement = new LoginManagement();
            emailService = new EmailService();
        }

        public IActionResult displayLogin()
        {
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> Authenticate(string email, string password)
        {
            var tempUser = await LoginManagement.AuthenticateUser(email, password);
            if (tempUser != null)
            {
                // Store only UserID temporarily for OTP verification
                var (userID, _) = tempUser.GetSessionData();
                HttpContext.Session.SetString("TempUserID", userID);

                return RedirectToAction("displayChooseEmail");
            }
            ViewBag.Error = "Invalid login credentials";
            return View("Login");
        }

        // Display page to let users choose which email to send OTP to
        public IActionResult displayChooseEmail()
        {
            return View("ChooseEmail");
        }

        // Send OTP to the chosen email
        [HttpPost]
        public async Task<IActionResult> SendOTPToEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "Email cannot be empty.";
                return View("ChooseEmail");
            }

            // Generate OTP
            var otpCode = new Random().Next(100000, 999999).ToString();

            // Store OTP and chosen email in session
            HttpContext.Session.SetString("OTP", otpCode);
            HttpContext.Session.SetString("ChosenEmail", email);
            HttpContext.Session.SetString("OTP_Expiry", DateTime.UtcNow.AddMinutes(10).ToString()); // Expiry time

            // Send OTP via Gmail SMTP
            bool isSent = await emailService.SendOtpEmail(email, otpCode);

            if (isSent)
            {
                return RedirectToAction("displayVerifyOTP");
            }

            ViewBag.Error = "Failed to send OTP. Please try again.";
            return View("ChooseEmail");
        }

        // Display OTP Verification Page
        public IActionResult displayVerifyOTP()
        {
            return View("VerifyOTP");
        }

        // Verify OTP and Email input
        [HttpPost]
        public async Task<IActionResult> VerifyOTP(string email, string otp)
        {
            var storedOtp = HttpContext.Session.GetString("OTP");
            var otpExpiry = HttpContext.Session.GetString("OTP_Expiry");

            // Check if OTP expired
            if (!string.IsNullOrEmpty(otpExpiry) && DateTime.UtcNow > DateTime.Parse(otpExpiry))
            {
                HttpContext.Session.Remove("OTP");
                ViewBag.Error = "OTP has expired. Please request a new one.";
                return View();
            }

            if (storedOtp == otp)
            {
                // OTP is correct, clear all OTP session data and log user in
                HttpContext.Session.Remove("OTP");
                HttpContext.Session.Remove("OTP_Expiry");
                HttpContext.Session.Remove("ChosenEmail");

                // Store UserID and Role in Session
                var tempuserID = HttpContext.Session.GetString("TempUserID");
                var authenticatedUser = await LoginManagement.GetUserByID(tempuserID);

                if (authenticatedUser != null)
                {
                    var (userID, role) = authenticatedUser.GetSessionData();
                    HttpContext.Session.SetString("UserID", userID);
                    HttpContext.Session.SetString("Role", role);
                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid OTP or email. Please try again.";
            return View();
        }
    }
}