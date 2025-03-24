using Microsoft.AspNetCore.Mvc;
using ClearCare.Models.Entities;
using ClearCare.Models.Control;
using ClearCare.Models.Interface;
using System.Threading.Tasks;
using ClearCare.DataSource;

namespace ClearCare.Controllers
{
    public class LoginController : Controller
    {
        private readonly LoginManagement LoginManagement;
        private readonly AccountManagement _accountManagement;

        private readonly AuditManagement _auditManagement; // Add AuditManagement

        public LoginController(IPassword passwordService, IEmail emailService)
        {
            var userGateway = new UserGateway(); 

            LoginManagement = new LoginManagement(passwordService, emailService);
            _accountManagement = new AccountManagement(userGateway, passwordService);   
            _auditManagement = new AuditManagement(); // ✅ Initialize AuditManagement 
        }

        public IActionResult displayLogin()
        {
            return View("Login");
        }

        [HttpPost]
        public async Task<IActionResult> authenticate(string email, string password)
        {
            // For OTP Flow
            // var tempUser = await LoginManagement.authenticateUser(email, password);
            // if (tempUser != null)
            // {
            //     // Store only UserID temporarily for OTP verification
            //     var (userID, _) = tempUser.getSessionData();
            //     HttpContext.Session.SetString("TempUserID", userID);

            //     return RedirectToAction("displayChooseEmail");
            // }

            // Dev purposes, straight login
            var authenticatedUser = await LoginManagement.authenticateUser(email, password);
            if (authenticatedUser != null)
            {
                var (userID, role) = authenticatedUser.getSessionData();
                HttpContext.Session.SetString("UserID", userID);
                HttpContext.Session.SetString("Role", role);

                TempData["SuccessMessage"] = "Logged in.";
                return RedirectToAction("Index", "Home");
            }
                
            TempData["ErrorMessage"] = "Invalid login credentials";
            return View("Login");
        }

        // Display page to let users choose which email to send OTP to
        public IActionResult displayChooseEmail()
        {
            return View("ChooseEmail");
        }

        // Send OTP to the chosen email
        [HttpPost]
        public async Task<IActionResult> sendOTPToEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Email cannot be empty.";                
                //ViewBag.Error = "Email cannot be empty.";
                return View("ChooseEmail");
            }

            // Generate OTP
            var otpCode = new Random().Next(100000, 999999).ToString();

            // Store OTP and chosen email in session
            HttpContext.Session.SetString("OTP", otpCode);
            HttpContext.Session.SetString("ChosenEmail", email);
            HttpContext.Session.SetString("OTP_Expiry", DateTime.UtcNow.AddMinutes(10).ToString()); // Expiry time

            // Send OTP via Gmail SMTP
            bool isSent = await LoginManagement.sendOTP(email, otpCode);

            if (isSent)
            {
                return RedirectToAction("displayVerifyOTP");
            }

            TempData["ErrorMessage"] = "Failed to send OTP. Please try again.";                
            //ViewBag.Error = "Failed to send OTP. Please try again.";
            return View("ChooseEmail");
        }

        // Display OTP Verification Page
        public IActionResult displayVerifyOTP()
        {
            return View("VerifyOTP");
        }

        // Verify OTP and Email input
        [HttpPost]
        public async Task<IActionResult> verifyOTP(string email, string otp)
        {
            var storedOtp = HttpContext.Session.GetString("OTP");
            var otpExpiry = HttpContext.Session.GetString("OTP_Expiry");

            // Check if OTP expired
            if (!string.IsNullOrEmpty(otpExpiry) && DateTime.UtcNow > DateTime.Parse(otpExpiry))
            {
                HttpContext.Session.Remove("OTP");
                TempData["ErrorMessage"] = "OTP has expired. Please request a new one.";
                //ViewBag.Error = "OTP has expired. Please request a new one.";
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
                var authenticatedUser = await LoginManagement.getUserByID(tempuserID);

                if (authenticatedUser != null)
                {
                    var (userID, role) = authenticatedUser.getSessionData();
                    HttpContext.Session.SetString("UserID", userID);
                    HttpContext.Session.SetString("Role", role);
                }

                return RedirectToAction("Index", "Home");
            }
            
            TempData["ErrorMessage"] = "Invalid OTP or email. Please try again.";
            //ViewBag.Error = "Invalid OTP or email. Please try again.";
            return View();
        }

        // Display the reset password page
        public IActionResult displayResetPassword()
        {
            return View("ResetPassword");
        }

        // Handle the reset password request
        [HttpPost]
        public async Task<IActionResult> requestResetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Email cannot be empty.";
                //ViewBag.Error = "Email cannot be empty.";
                return View("ResetPassword");
            }

            // Generate OTP
            var otpCode = new Random().Next(100000, 999999).ToString();

            // Store OTP and email in session
            HttpContext.Session.SetString("ResetOTP", otpCode);
            HttpContext.Session.SetString("ResetEmail", email);
            HttpContext.Session.SetString("ResetOTP_Expiry", DateTime.UtcNow.AddMinutes(10).ToString()); // Expiry time

            // Send OTP via Gmail SMTP
            bool isSent = await LoginManagement.sendOTP(email, otpCode);

            if (isSent)
            {
                return RedirectToAction("displayVerifyResetOTP");
            }

            TempData["ErrorMessage"] = "Failed to send OTP. Please try again.";
            //ViewBag.Error = "Failed to send OTP. Please try again.";
            return View("ResetPassword");
        }

        // Display the OTP verification page for reset password
        public IActionResult displayVerifyResetOTP()
        {
            return View("VerifyResetOTP");
        }

        // Verify the OTP for reset password
        [HttpPost]
        public async Task<IActionResult> verifyResetOTP(string email, string otp)
        {
            var storedOtp = HttpContext.Session.GetString("ResetOTP");
            var otpExpiry = HttpContext.Session.GetString("ResetOTP_Expiry");

            // Check if OTP expired
            if (!string.IsNullOrEmpty(otpExpiry) && DateTime.UtcNow > DateTime.Parse(otpExpiry))
            {
                HttpContext.Session.Remove("ResetOTP");
                ViewBag.Error = "OTP has expired. Please request a new one.";
                return View("VerifyResetOTP");
            }

            if (storedOtp == otp)
            {
                // OTP is correct, clear all OTP session data and allow password reset
                HttpContext.Session.Remove("ResetOTP");
                HttpContext.Session.Remove("ResetOTP_Expiry");
                HttpContext.Session.SetString("ResetConfirmed", "true");

                return RedirectToAction("displayNewPassword");
            }

            TempData["ErrorMessage"] = "Invalid OTP or email. Please try again.";
            //ViewBag.Error = "Invalid OTP or email. Please try again.";
            return View("VerifyResetOTP");
        }

        // Display the new password page
        public IActionResult displayNewPassword()
        {
            return View("NewPassword");
        }

        // Handle the new password submission
       [HttpPost]
        public async Task<IActionResult> submitNewPassword(string newPassword, string confirmPassword)
        {
            // Server-side validation to ensure passwords match
            if (newPassword != confirmPassword)
            {
                TempData["ErrorMessage"] = "Passwords do not match. Please try again.";
                //ViewBag.Error = "Passwords do not match. Please try again.";
                return View("NewPassword");
            }

            var resetEmail = HttpContext.Session.GetString("ResetEmail");
            if (string.IsNullOrEmpty(resetEmail))
            {
                TempData["ErrorMessage"] = "Session expired. Please try again.";
                //ViewBag.Error = "Session expired. Please try again.";
                return View("NewPassword");
            }

            bool isReset = await _accountManagement.ResetPassword(resetEmail, newPassword, HttpContext, _auditManagement);
            if (isReset)
            {
                HttpContext.Session.Remove("ResetEmail");
                HttpContext.Session.Remove("ResetConfirmed");
                return RedirectToAction("Index", "Home");
            }

            TempData["ErrorMessage"] = "Failed to reset password. Please try again.";
            //ViewBag.Error = "Failed to reset password. Please try again.";
            return View("NewPassword");
        }
    }
}