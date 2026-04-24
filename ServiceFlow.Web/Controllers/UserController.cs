using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceFlow.Class.Models;
using ServiceFlow.Web.ViewModels;

namespace ServiceFlow.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<IActionResult> Index(string? filter)
        {
            var users = userManager.Users.ToList();

            var vm = new List<UserListViewModel>();
            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user);
                var role = roles.FirstOrDefault() ?? "Sin rol";

                if (filter != null && !role.Equals(filter, StringComparison.OrdinalIgnoreCase))
                    continue;

                vm.Add(new UserListViewModel
                {
                    Id = user.Id,
                    FullName = user.FirstName + " " + user.PaternalSurname + " " + user.MaternalSurname,
                    Role = role,
                    Email = user.Email!
                });
            }

            ViewBag.CurrentFilter = filter;
            ViewBag.CountAll = userManager.Users.Count();
            ViewBag.CountAdmin = (await userManager.GetUsersInRoleAsync("Admin")).Count;
            ViewBag.CountAgent = (await userManager.GetUsersInRoleAsync("Agent")).Count;
            ViewBag.CountUser = (await userManager.GetUsersInRoleAsync("User")).Count;

            return View(vm);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await userManager.GetRolesAsync(user);

            var vm = new EditUserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                PaternalSurname = user.PaternalSurname,
                MaternalSurname = user.MaternalSurname,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber!,
                Role = roles.FirstOrDefault() ?? "User"
            };

            ViewBag.Roles = new SelectList(new[] { "Admin", "Agent", "User" });
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new SelectList(new[] { "Admin", "Agent", "User" });
                return View(model);
            }

            var user = await userManager.FindByIdAsync(model.Id);
            if (user == null) return NotFound();

            user.FirstName = model.FirstName;
            user.PaternalSurname = model.PaternalSurname;
            user.MaternalSurname = model.MaternalSurname;
            user.Email = model.Email;
            user.UserName = model.Email;
            user.PhoneNumber = model.PhoneNumber;

            await userManager.UpdateAsync(user);

            // Actualizar rol
            var currentRoles = await userManager.GetRolesAsync(user);
            await userManager.RemoveFromRolesAsync(user, currentRoles);
            await userManager.AddToRoleAsync(user, model.Role);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await userManager.DeleteAsync(user);
            return RedirectToAction("Index");
        }
    }
}
