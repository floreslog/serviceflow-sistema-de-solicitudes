using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceFlow.Class.Models;
using ServiceFlow.Web.ViewModels;
using System.Security.Claims;

namespace ServiceFlow.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
           this.userManager = userManager;
           this.signInManager = signInManager;
        }


        //Login GET
        public IActionResult Login()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
               return View(model);
            }

            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, true, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Email o contraseña incorrectos.");
                return View(model);
            }
            return RedirectToAction("Index", "Home");
        }

        //Register GET
        public IActionResult Register()
        {
            if (User.Identity!.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            return View(new RegisterViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                FirstName = model.FirstName,
                PaternalSurname = model.PaternalSurname,
                MaternalSurname = model.MaternalSurname,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                UserName = model.Email
            };

            var result = await userManager.CreateAsync(user, model.Password);
            if(!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View(model);
            }
            await userManager.AddToRoleAsync(user, "User");
            TempData["Success"] = "Cuenta creada exitosamente. Inicia sesion.";
            return RedirectToAction("Login");
        }

        //Logout
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        // google access functions
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
            if (info == null)
                return RedirectToAction("Login");

            var result = await signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            // si el usuario cancela o no llega email
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
