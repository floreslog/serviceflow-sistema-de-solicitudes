using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceFlow.Class.Models;
using ServiceFlow.Web.Services;
using ServiceFlow.Web.ViewModels;
using System.Security.Claims;

namespace ServiceFlow.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly EmailService emailService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, EmailService emailService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.emailService = emailService;
        }

        // LOGIN
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        // REGISTER
        public IActionResult Register()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Index", "Home");
            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var existingUser = await userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Ya existe una cuenta con ese correo.");
                return View(model);
            }

            var code = new Random().Next(100000, 999999).ToString();

            HttpContext.Session.SetString("RegisterCode", code);
            HttpContext.Session.SetString("RegisterEmail", model.Email);
            HttpContext.Session.SetString("RegisterFirstName", model.FirstName);
            HttpContext.Session.SetString("RegisterPaternalSurname", model.PaternalSurname);
            HttpContext.Session.SetString("RegisterMaternalSurname", model.MaternalSurname);
            HttpContext.Session.SetString("RegisterPhone", model.PhoneNumber ?? "");
            HttpContext.Session.SetString("RegisterPassword", model.Password);

            var body = $@"
                <div style='font-family: sans-serif; max-width: 480px; margin: 0 auto;'>
                    <h2 style='color: #25262a;'>Verifica tu cuenta</h2>
                    <p>Hola <strong>{model.FirstName}</strong>, gracias por registrarte en ServiceFlow.</p>
                    <p>Tu código de verificación es:</p>
                    <div style='font-size: 2rem; font-weight: 700; letter-spacing: 8px; color: #25262a; padding: 16px; background: #f5f5f5; border-radius: 8px; text-align: center;'>
                        {code}
                    </div>
                    <p style='color: #9aa0a6; font-size: 0.85rem; margin-top: 16px;'>Este código expira en 10 minutos.</p>
                </div>";

            await emailService.SendEmailAsync(model.Email, "Verifica tu cuenta — ServiceFlow", body);

            return RedirectToAction("VerifyEmail");
        }

        // VERIFY EMAIL
        [HttpGet]
        [AllowAnonymous]
        public IActionResult VerifyEmail()
        {
            return View(new VerifyEmailViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyEmail(VerifyEmailViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var storedCode = HttpContext.Session.GetString("RegisterCode");
            if (storedCode == null || storedCode != model.Code)
            {
                ModelState.AddModelError("Code", "El código es incorrecto o ha expirado.");
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = HttpContext.Session.GetString("RegisterEmail"),
                Email = HttpContext.Session.GetString("RegisterEmail"),
                FirstName = HttpContext.Session.GetString("RegisterFirstName")!,
                PaternalSurname = HttpContext.Session.GetString("RegisterPaternalSurname")!,
                MaternalSurname = HttpContext.Session.GetString("RegisterMaternalSurname")!,
                PhoneNumber = HttpContext.Session.GetString("RegisterPhone"),
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, HttpContext.Session.GetString("RegisterPassword")!);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            await userManager.AddToRoleAsync(user, "User");
            HttpContext.Session.Clear();
            TempData["Success"] = "Cuenta creada exitosamente. Inicia sesión.";
            return RedirectToAction("Login");
        }

        // FORGOT PASSWORD
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("Email", "No existe una cuenta con ese correo.");
                return View(model);
            }

            var code = new Random().Next(100000, 999999).ToString();
            HttpContext.Session.SetString("ResetCode", code);
            HttpContext.Session.SetString("ResetEmail", model.Email);

            var body = $@"
                <div style='font-family: sans-serif; max-width: 480px; margin: 0 auto;'>
                    <h2 style='color: #25262a;'>Restablecer contraseña</h2>
                    <p>Recibimos una solicitud para restablecer tu contraseña en ServiceFlow.</p>
                    <p>Tu código de verificación es:</p>
                    <div style='font-size: 2rem; font-weight: 700; letter-spacing: 8px; color: #25262a; padding: 16px; background: #f5f5f5; border-radius: 8px; text-align: center;'>
                        {code}
                    </div>
                    <p style='color: #9aa0a6; font-size: 0.85rem; margin-top: 16px;'>Si no solicitaste esto, ignora este correo.</p>
                </div>";

            await emailService.SendEmailAsync(model.Email, "Restablecer contraseña — ServiceFlow", body);
            return RedirectToAction("ResetPassword");
        }

        // RESET PASSWORD
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword()
        {
            return View(new ResetPasswordViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var storedCode = HttpContext.Session.GetString("ResetCode");
            if (storedCode == null || storedCode != model.Code)
            {
                ModelState.AddModelError("Code", "El código es incorrecto o ha expirado.");
                return View(model);
            }

            var email = HttpContext.Session.GetString("ResetEmail");
            var user = await userManager.FindByEmailAsync(email!);
            if (user == null) return RedirectToAction("Login");

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                return View(model);
            }

            HttpContext.Session.Clear();
            TempData["Success"] = "Contraseña actualizada correctamente. Inicia sesión.";
            return RedirectToAction("Login");
        }

        // LOGOUT
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        // GOOGLE
        [HttpGet]
        [AllowAnonymous]
        public IActionResult GoogleLogin()
        {
            var redirectUrl = Url.Action("GoogleCallback", "Account");
            var properties = signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, "Google");
        }

        [AllowAnonymous]
        public async Task<IActionResult> GoogleCallback()
        {
            var info = await signInManager.GetExternalLoginInfoAsync();
            if (info == null) return RedirectToAction("Login");

            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded) return RedirectToAction("Index", "Home");

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                TempData["Error"] = "No se pudo obtener el correo de Google.";
                return RedirectToAction("Login");
            }

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                var firstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? email.Split('@')[0];
                var lastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? "";

                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    PaternalSurname = lastName,
                    MaternalSurname = "",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(user);
                await userManager.AddToRoleAsync(user, "User");
            }

            await userManager.AddLoginAsync(user, info);
            await signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }
    }
}