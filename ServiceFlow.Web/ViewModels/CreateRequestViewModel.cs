using ServiceFlow.Class.Models;
using System.ComponentModel.DataAnnotations;

namespace ServiceFlow.Web.ViewModels
{
    public class CreateRequestViewModel
    {
        [Required]
        [Display(Name = "Titulo")]
        public string Title { get; set; } = null!;
        [Required]
        [Display(Name = "Descripción")]
        public string Description { get; set; } = null!;
        [Required]
        [Display(Name = "Ubicación")]
        public string Location { get; set; } = null!;
        [Display(Name = "Prioridad sugerida")]
        public Priority Priority { get; set; }
        [Display(Name = "Categoria")]
        public int CategoryId { get; set; }
    }
}
