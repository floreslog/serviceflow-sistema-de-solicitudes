using System.ComponentModel.DataAnnotations;

namespace ServiceFlow.Web.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [Display(Name = "Nombre")]
        public string Name { get; set; } = null!;
    }
}
