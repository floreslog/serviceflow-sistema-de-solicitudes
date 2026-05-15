using System.ComponentModel.DataAnnotations;

namespace ServiceFlow.Web.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; } = null!;

        [Required]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; } = null!;

        [Required]
        [Display(Name = "Apellido Paterno")]
        public string PaternalSurname { get; set; } = null!;

        [Display(Name = "Apellido Materno")]
        public string? MaternalSurname { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = null!;

        [Display(Name = "Número de teléfono")]
        public string? PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Rol")]
        public string Role { get; set; } = null!;
    }
}