using employee_management.Models;
using employee_management.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;

namespace employee_management.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // Add role
                    await _userManager.AddToRoleAsync(user, "User");

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Action("ConfirmEmail", "Account",
                        new { userId = user.Id, token }, Request.Scheme);

                    await _emailSender.SendEmailAsync(model.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    return RedirectToAction("RegisterConfirmation", new { email = model.Email });
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        public IActionResult RegisterConfirmation(string email)
        {
            ViewBag.Email = email;
            return View();
        }

        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null && !await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError(string.Empty, "You need to confirm your email.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

            if (result.Succeeded)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    return RedirectToAction("Index", "Admin");
                }
                else if (await _userManager.IsInRoleAsync(user, "Employee"))
                {
                    return RedirectToAction("Index", "User");
                }

                // Default fallback
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View(model);
        }


        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account");
        }

        // GET: /Account/ForgotPassword
        public IActionResult ForgotPassword() => View();

        // POST: /Account/ForgotPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "No user associated with this email address.");
                return View(model);
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                ModelState.AddModelError(string.Empty, "Email is not confirmed. Please confirm your email before resetting your password.");
                return View(model);
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { token, email = model.Email }, Request.Scheme);

            await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                $"Reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

            return RedirectToAction("ForgotPasswordConfirmation");
        }

        [Authorize]
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return RedirectToAction("Login");

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Your password has been changed successfully.";
                return RedirectToAction("ChangePassword");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        public IActionResult ForgotPasswordConfirmation() => View();

        // GET: /Account/ResetPassword
        public IActionResult ResetPassword(string token, string email) => View(new ResetPasswordViewModel { Token = token, Email = email });

        // POST: /Account/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return RedirectToAction("ResetPasswordConfirmation");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
                return RedirectToAction("ResetPasswordConfirmation");

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View();
        }

        public IActionResult ResetPasswordConfirmation() => View();
    }
}
