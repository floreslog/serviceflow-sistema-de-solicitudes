using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServiceFlow.Class.Models;
using ServiceFlow.Class.Repositories;
using ServiceFlow.Web.ViewModels;
using System.Security.Claims;

namespace ServiceFlow.Web.Controllers
{
    [Authorize]
    public class RequestController : Controller
    {
        private readonly IRepository<RequestModel> requestRepo;
        private readonly IRepository<CommentModel> commentRepo;
        private readonly IRepository<CategoryModel> categoryRepo;
        private readonly UserManager<ApplicationUser> userManager;
        public RequestController(IRepository<RequestModel> requestRepo, IRepository<CommentModel> commentRepo, IRepository<CategoryModel> categoryRepo, UserManager<ApplicationUser> userManager)
        {
            this.requestRepo = requestRepo;
            this.commentRepo = commentRepo;
            this.categoryRepo = categoryRepo;
            this.userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var requests = await requestRepo.GetAll();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IEnumerable<RequestModel> filtered;

            if (User.IsInRole("User"))
            {
                filtered = requests.Where(r => r.RequesterId == userId).ToList();
            }
            else if(User.IsInRole("Agent"))
            {
                filtered = requests.Where(r => r.AssigneeId == userId).ToList();
            }
            else
            {
                filtered = requests;
            }


            var vm = filtered.Select(r => new RequestListViewModel
            {
                Id = r.Id,
                Title = r.Title,
                CategoryName = r.Category.Name,
                RequesterName = r.Requester.FirstName + " " + r.Requester.PaternalSurname,
                AssigneeName = r.Assignee != null ? r.Assignee.FirstName + " " + r.Assignee.PaternalSurname : "Sin asignar",
                Status = r.Status,
                Priority = r.Priority,
                Creation = r.Creation
            }).ToList();

            return View(vm);
        }

        [Authorize(Roles ="User")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await categoryRepo.GetAll();
            return View(new CreateRequestViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Create(CreateRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await categoryRepo.GetAll();
                return View(model);
            }

            var rm = new RequestModel
            {
                Title = model.Title,
                Description = model.Description,
                Location = model.Location,
                Priority = model.Priority,
                CategoryId = model.CategoryId,
                Status = Status.Open,
                Creation = DateTime.Now,
                RequesterId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            };

            await requestRepo.Create(rm);
            return RedirectToAction("Index");
        }
    }
}
