using System.ComponentModel.DataAnnotations;

namespace ServiceFlow.Web.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [Display(Name = "Nombre")]
        public string FirstName { get; set; } = null!;
        [Required]
        [Display(Name = "Apellido Paterno")]
        public string PaternalSurname { get; set; } = null!;
        [Required]
        [Display(Name = "Apellido Materno")]
        public string MaternalSurname { get; set; } = null!;
        [Required]
        [EmailAddress]
        [Display(Name = "Correo electronico")]
        public string Email { get; set; } = null!;
        [Required]
        [Display(Name = "Numero de Telefono")]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = null!;
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmPassword { get; set; } = null!;

    }
}
