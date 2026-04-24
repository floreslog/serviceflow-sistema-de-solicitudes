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

        public CategoryController(IRepository<CategoryModel> categoryRepo)
        {
            this.categoryRepo = categoryRepo;
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
            if (!ModelState.IsValid)
                return RedirectToAction("Index");

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
            await categoryRepo.Delete(id);
            TempData["Success"] = "Categoría eliminada.";
            return RedirectToAction("Index");
        }
    }
}