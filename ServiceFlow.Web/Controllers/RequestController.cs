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
        public async Task<IActionResult> Index(string? status, string? priority, string? filter)
        {
            var requests = await requestRepo.GetAll();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            IEnumerable<RequestModel> filtered;

            if (User.IsInRole("User"))
                filtered = requests.Where(r => r.RequesterId == userId);
            else if (User.IsInRole("Agent"))
                filtered = requests.Where(r => r.AssigneeId == userId);
            else
                filtered = requests;

            // Filtro rápido (sin atender / atendidas)
            if (filter == "pending")
                filtered = filtered.Where(r => r.Status == Status.Open || r.Status == Status.Assigned || r.Status == Status.InProgress || r.Status == Status.OnHold);
            else if (filter == "resolved")
                filtered = filtered.Where(r => r.Status == Status.Resolved || r.Status == Status.Closed);

            // Filtro por estado
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<Status>(status, out var parsedStatus))
                filtered = filtered.Where(r => r.Status == parsedStatus);

            // Filtro por prioridad
            if (!string.IsNullOrEmpty(priority) && Enum.TryParse<Priority>(priority, out var parsedPriority))
                filtered = filtered.Where(r => r.Priority == parsedPriority);

            // Conteos para la sidebar
            var baseList = User.IsInRole("User") ? requests.Where(r => r.RequesterId == userId) :
                           User.IsInRole("Agent") ? requests.Where(r => r.AssigneeId == userId) :
                           requests;

            ViewBag.CountAll = baseList.Count();
            ViewBag.CountPending = baseList.Count(r => r.Status == Status.Open || r.Status == Status.Assigned || r.Status == Status.InProgress || r.Status == Status.OnHold);
            ViewBag.CountResolved = baseList.Count(r => r.Status == Status.Resolved || r.Status == Status.Closed);
            ViewBag.CountOpen = baseList.Count(r => r.Status == Status.Open);
            ViewBag.CountAssigned = baseList.Count(r => r.Status == Status.Assigned);
            ViewBag.CountInProgress = baseList.Count(r => r.Status == Status.InProgress);
            ViewBag.CountOnHold = baseList.Count(r => r.Status == Status.OnHold);
            ViewBag.CountResolved2 = baseList.Count(r => r.Status == Status.Resolved);
            ViewBag.CountClosed = baseList.Count(r => r.Status == Status.Closed);
            ViewBag.CountCancelled = baseList.Count(r => r.Status == Status.Cancelled);
            ViewBag.CountLow = baseList.Count(r => r.Priority == Priority.Low);
            ViewBag.CountMedium = baseList.Count(r => r.Priority == Priority.Medium);
            ViewBag.CountHigh = baseList.Count(r => r.Priority == Priority.High);
            ViewBag.CountUrgent = baseList.Count(r => r.Priority == Priority.Urgent);

            ViewBag.CurrentStatus = status;
            ViewBag.CurrentPriority = priority;
            ViewBag.CurrentFilter = filter;

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
            TempData["Success"] = "Solicitud creada exitosamente.";
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

            var agents = await userManager.GetUsersInRoleAsync("Agent");
            ViewBag.Asignees = agents
                .Select(a => new { Id = a.Id, Name = a.FirstName + " " + a.PaternalSurname })
                .ToList();

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


        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var request = await requestRepo.GetById(id);
            if (request == null) return NotFound();
            ViewBag.Categories = await categoryRepo.GetAll();

            var agents = await userManager.GetUsersInRoleAsync("Agent");
            ViewBag.Asignees = agents
                .Select(a => new { Id = a.Id, Name = a.FirstName + " " + a.PaternalSurname })
                .ToList();

            var vm = new EditRequestViewModel
            {
                Id = request.Id,
                Title = request.Title,
                Description = request.Description,
                Location = request.Location,
                CategoryId = request.CategoryId,
                Priority = request.Priority,
                Status = request.Status,
                AssigneeId = request.AssigneeId
            };
            return View(vm);
        }

        [HttpPost]
        [Authorize(Roles = "User,Admin")]
        public async Task<IActionResult> Edit(EditRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await categoryRepo.GetAll();

                var agents = await userManager.GetUsersInRoleAsync("Agent");
                ViewBag.Asignees = agents
                    .Select(a => new { Id = a.Id, Name = a.FirstName + " " + a.PaternalSurname })
                    .ToList();

                return View(model);
            }
            var request = await requestRepo.GetById(model.Id);
            if (request == null) return NotFound();
            request.Title = model.Title;
            request.Description = model.Description;
            request.Location = model.Location;
            request.CategoryId = model.CategoryId;
            request.LastUpdated = DateTime.Now;
            if (User.IsInRole("Admin"))
            {
                request.Priority = model.Priority;
                request.Status = model.Status;
                request.AssigneeId = model.AssigneeId;
            }
            await requestRepo.Update(request);
            TempData["Success"] = "Solicitud actualizada correctamente.";
            return RedirectToAction("Detail", new { id = model.Id });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await requestRepo.Delete(id);
            TempData["Success"] = "Solicitud eliminada.";
            return RedirectToAction("Index");
        }
    }
}
