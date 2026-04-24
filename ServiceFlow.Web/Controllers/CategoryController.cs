using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceFlow.Class.Models;
using ServiceFlow.Class.Repositories;
using ServiceFlow.Web.ViewModels;

namespace ServiceFlow.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController : Controller
    {
        private readonly IRepository<CategoryModel> categoryRepo;
        private readonly IRepository<RequestModel> requestRepo;

        public CategoryController(IRepository<CategoryModel> categoryRepo, IRepository<RequestModel> requestRepo)
        {
            this.categoryRepo = categoryRepo;
            this.requestRepo = requestRepo;
        }

        public async Task<IActionResult> Index()
        {
            var categories = await categoryRepo.GetAll();
            var vm = categories.Select(c => new CategoryViewModel
            {
                Id = c.Id,
                Name = c.Name
            }).ToList();
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryViewModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.Name))
            {
                TempData["Error"] = "El nombre de la categoría no puede estar vacío.";
                return RedirectToAction("Index");
            }

            var category = new CategoryModel { Name = model.Name };
            await categoryRepo.Create(category);
            TempData["Success"] = "Categoría creada exitosamente.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryViewModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

            var category = await categoryRepo.GetById(model.Id);
            if (category == null) return NotFound();

            category.Name = model.Name;
            await categoryRepo.Update(category);
            TempData["Success"] = "Categoría actualizada.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var requests = await requestRepo.GetAll();
            var hasRequests = requests.Any(r => r.CategoryId == id);

            if (hasRequests)
            {
                TempData["Error"] = "No puedes eliminar esta categoría porque tiene solicitudes asignadas.";
                return RedirectToAction("Index");
            }

            await categoryRepo.Delete(id);
            TempData["Success"] = "Categoría eliminada.";
            return RedirectToAction("Index");
        }
    }
}