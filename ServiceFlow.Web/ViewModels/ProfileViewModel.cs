using System.ComponentModel.DataAnnotations;

namespace ServiceFlow.Web.ViewModels
{
    public class ProfileViewModel
    {
        public string Id { get; set; } = null!;

        [Display(Name = "Nombre")]
        public string FirstName { get; set; } = null!;

        [Display(Name = "Apellido paterno")]
        public string PaternalSurname { get; set; } = null!;

        [Display(Name = "Apellido materno")]
        public string MaternalSurname { get; set; } = null!;

        [Display(Name = "Correo electronico")]
        public string Email { get; set; } = null!;

        [Display(Name = "Telefono")]
        public string? PhoneNumber { get; set; }

        public string? Role { get; set; }
    }
}