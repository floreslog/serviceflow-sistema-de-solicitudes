using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceFlow.Class.Models;
using ServiceFlow.Web.ViewModels;

namespace ServiceFlow.Web.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public ProfileController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var roles = await userManager.GetRolesAsync(user);

            var vm = new ProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                PaternalSurname = user.PaternalSurname,
                MaternalSurname = user.MaternalSurname,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                Role = roles.FirstOrDefault()
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ProfileViewModel model)
        {
            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            user.FirstName = model.FirstName;
            user.PaternalSurname = model.PaternalSurname;
            user.MaternalSurname = model.MaternalSurname;
            user.PhoneNumber = model.PhoneNumber;

            await userManager.UpdateAsync(user);
            TempData["Success"] = "Perfil actualizado correctamente.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Revisa los campos del formulario.";
                return RedirectToAction("Index");
            }

            var user = await userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var result = await userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (!result.Succeeded)
            {
                TempData["Error"] = "La contraseña actual no es correcta.";
                return RedirectToAction("Index");
            }

            await signInManager.RefreshSignInAsync(user);
            TempData["Success"] = "Contraseña actualizada correctamente.";
            return RedirectToAction("Index");
        }
    }
}