using ServiceFlow.Class.Models;
using System.ComponentModel.DataAnnotations;

namespace ServiceFlow.Web.ViewModels
{
    public class EditRequestViewModel
    {
        public int Id { get; set; }
        [Required]
        [Display(Name = "Título")]
        public string Title { get; set; } = null!;
        [Required]
        [Display(Name = "Descripción")]
        public string Description { get; set; } = null!;
        [Required]
        [Display(Name = "Ubicación")]
        public string Location { get; set; } = null!;
        [Display(Name = "Categoría")]
        public int CategoryId { get; set; }
        [Display(Name = "Prioridad")]
        public Priority Priority { get; set; }
        [Display(Name = "Estado")]
        public Status Status { get; set; }
        [Display(Name = "Agente asignado")]
        public string? AssigneeId { get; set; }
    }
}
