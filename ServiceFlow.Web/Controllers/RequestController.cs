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

        public async Task<IActionResult> Detail(int id)
        {
            var request = await requestRepo.GetById(id);
            var comments = await commentRepo.GetAll();

            var requestdetailvm = new RequestDetailViewModel();

            requestdetailvm.Id = id;
            requestdetailvm.Title = request.Title;
            requestdetailvm.Description = request.Description;
            requestdetailvm.Location = request.Location;
            requestdetailvm.Priority = request.Priority;
            requestdetailvm.CategoryId = request.CategoryId;
            requestdetailvm.Status = request.Status;
            requestdetailvm.Creation = request.Creation;
            if (request.AssigneeId != null)
            {
                var assignee = await userManager.FindByIdAsync(request.AssigneeId);
                requestdetailvm.AssigneeName = assignee.FirstName + " " + assignee.PaternalSurname;
            }

            var requester = await userManager.FindByIdAsync(request.RequesterId);
            requestdetailvm.RequesterName = requester.FirstName + " " + requester.PaternalSurname;

            //llenar lista de comentarios
            var filteredComments = comments.Where(c => c.RequestId == id).ToList();
            foreach (var comment in filteredComments)
            {
                var author = await userManager.FindByIdAsync(comment.AuthorId);
                requestdetailvm.Comments.Add(new CommentViewModel
                {
                    Text = comment.Text,
                    CreatedAt = comment.CreatedAt,
                    AuthorName = author.FirstName + " " + author.PaternalSurname
                });
            }

            return View(requestdetailvm);
        }



        [HttpPost]
        public async Task<IActionResult> AddComment(int requestId, string text)
        {
            if (!string.IsNullOrWhiteSpace(text))
            {
                var comment = new CommentModel
                {
                    Text = text,
                    CreatedAt = DateTime.Now,
                    RequestId = requestId,
                    AuthorId = User.FindFirstValue(ClaimTypes.NameIdentifier)!
                };
                await commentRepo.Create(comment);
            }
            return RedirectToAction("Detail", new { id = requestId });
        }

    }
}
