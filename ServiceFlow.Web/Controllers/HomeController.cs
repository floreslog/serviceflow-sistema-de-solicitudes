using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceFlow.Class.Models;
using ServiceFlow.Class.Repositories;
using ServiceFlow.Web.Models;
using ServiceFlow.Web.ViewModels;
using System.Diagnostics;
using System.Security.Claims;

namespace ServiceFlow.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IRepository<RequestModel> requestRepo;

        public HomeController(IRepository<RequestModel> requestRepo)
        {
            this.requestRepo = requestRepo;
        }
        public async Task<IActionResult> Index()
        {
            var requests = await requestRepo.GetAll();
            var vm = new DashboardViewModel();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (User.IsInRole("Admin"))
            {
                vm.TotalRequests = requests.Count;
                vm.OpenRequests = requests.Count(r => r.Status == Status.Open);
                vm.ResolvedRequests = requests.Count(r => r.Status == Status.Resolved);
                vm.UrgentRequests = requests.Count(r => r.Priority == Priority.Urgent);
            }
            else if (User.IsInRole("Agent"))
            {
                vm.TotalAssignedRequests = requests.Count(r => r.AssigneeId == userId);
                vm.InProgressRequests = requests.Count(r => r.AssigneeId == userId && r.Status == Status.InProgress);
                vm.PendingRequests = requests.Count(r => r.AssigneeId == userId && r.Status == Status.OnHold);
            }
            else
            {
                vm.TotalRequests = requests.Count(r => r.RequesterId == userId);
                vm.OpenRequests = requests.Count(r => r.RequesterId == userId && r.Status == Status.Open);
                vm.ResolvedRequests = requests.Count(r => r.RequesterId == userId && r.Status == Status.Resolved);
            }
                return View(vm);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
