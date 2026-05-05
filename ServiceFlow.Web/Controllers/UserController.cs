using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ServiceFlow.Class.Models;
using ServiceFlow.Class.Repositories;
using ServiceFlow.Web.ViewModels;

namespace ServiceFlow.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly IRepository<RequestModel> requestRepo;
        private readonly IRepository<CategoryModel> categoryRepo;

        public UserController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IRepository<RequestModel> requestRepo, IRepository<CategoryModel> categoryRepo)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.requestRepo = requestRepo;
            this.categoryRepo = categoryRepo;
        }

        public async Task<IActionResult> Index(string? filter, string? search, int page = 1)
        {
            var users = userManager.Users.ToList();

            if (!string.IsNullOrEmpty(search))
            {
                var s = search.ToLower();
                users = users.Where(u =>
                    (u.FirstName + " " + u.PaternalSurname + " " + u.MaternalSurname).ToLower().Contains(s) ||
                    u.Email!.ToLower().Contains(s)
                ).ToList();
            }

            ViewBag.CurrentSearch = search;

            var allUsersWithRoles = new List<UserListViewModel>();
            foreach (var user in userManager.Users.ToList())
            {
                var roles = await userManager.GetRolesAsync(user);
                allUsersWithRoles.Add(new UserListViewModel
                {
                    Id = user.Id,
                    FullName = user.FirstName + " " + user.PaternalSurname + " " + user.MaternalSurname,
                    Role = roles.FirstOrDefault() ?? "Sin rol",
                    Email = user.Email!
                });
            }

            var filtered = allUsersWithRoles.AsEnumerable();

            if (!string.IsNullOrEmpty(search))
            {
                var s = search.ToLower();
                filtered = filtered.Where(u =>
                    u.FullName.ToLower().Contains(s) ||
                    u.Email.ToLower().Contains(s)
                );
            }

            if (filter != null)
                filtered = filtered.Where(u => u.Role.Equals(filter, StringComparison.OrdinalIgnoreCase));

            ViewBag.CurrentFilter = filter;
            ViewBag.CountAll = allUsersWithRoles.Count;
            ViewBag.CountAdmin = allUsersWithRoles.Count(u => u.Role == "Admin");
            ViewBag.CountAgent = allUsersWithRoles.Count(u => u.Role == "Agent");
            ViewBag.CountUser = allUsersWithRoles.Count(u => u.Role == "User");

            const int pageSize = 2;
            var totalItems = filtered.Count();
            var items = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new PagedResult<UserListViewModel>
            {
                Items = items,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling((double)totalItems / pageSize),
                TotalItems = totalItems,
                PageSize = pageSize
            };

            return View(vm);
        }

        public async Task<IActionResult> Detail(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "Sin rol";

            var allRequests = await requestRepo.GetAll();

            List<RequestListViewModel> requests = new();

            if (role == "User")
            {
                var userRequests = allRequests.Where(r => r.RequesterId == user.Id).ToList();
                foreach (var r in userRequests)
                {
                    var category = await categoryRepo.GetById(r.CategoryId);
                    requests.Add(new RequestListViewModel
                    {
                        Id = r.Id,
                        Title = r.Title,
                        CategoryName = category.Name,
                        RequesterName = user.FirstName + " " + user.PaternalSurname,
                        Status = r.Status,
                        Priority = r.Priority,
                        Creation = r.Creation
                    });
                }
            }
            else if (role == "Agent")
            {
                var agentRequests = allRequests.Where(r => r.AssigneeId == user.Id).ToList();
                foreach (var r in agentRequests)
                {
                    var category = await categoryRepo.GetById(r.CategoryId);
                    var requester = await userManager.FindByIdAsync(r.RequesterId);
                    requests.Add(new RequestListViewModel
                    {
                        Id = r.Id,
                        Title = r.Title,
                        CategoryName = category.Name,
                        RequesterName = requester.FirstName + " " + requester.PaternalSurname,
                        AssigneeName = user.FirstName + " " + user.PaternalSurname,
                        Status = r.Status,
                        Priority = r.Priority,
                        Creation = r.Creation
                    });
                }
            }

            var vm = new UserDetailViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                PaternalSurname = user.PaternalSurname,
                MaternalSurname = user.MaternalSurname,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                Role = role,
                AccessFailedCount = user.AccessFailedCount,
                TotalRequests = requests.Count,
                Requests = requests
            };

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

            var requests = await requestRepo.GetAll();
            var hasRequests = requests.Any(r => r.RequesterId == id || r.AssigneeId == id);

            if (hasRequests)
            {
                TempData["Error"] = "No puedes eliminar este usuario porque tiene solicitudes relacionadas.";
                return RedirectToAction("Index");
            }

            await userManager.DeleteAsync(user);
            TempData["Success"] = "Usuario eliminado correctamente.";
            return RedirectToAction("Index");
        }
    }
}
