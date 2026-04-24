using System.ComponentModel.DataAnnotations;

namespace ServiceFlow.Web.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "La contraseña actual es obligatoria.")]
        [Display(Name = "Contraseña actual")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; } = null!;

        [Required(ErrorMessage = "La nueva contraseña es obligatoria.")]
        [Display(Name = "Nueva contraseña")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Confirma la nueva contraseña.")]
        [Display(Name = "Confirmar nueva contraseña")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Las contraseñas no coinciden.")]
        public string ConfirmNewPassword { get; set; } = null!;
    }
}