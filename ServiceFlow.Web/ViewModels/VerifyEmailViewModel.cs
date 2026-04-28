using System.ComponentModel.DataAnnotations;

namespace ServiceFlow.Web.ViewModels
{
    public class VerifyEmailViewModel
    {
        [Required(ErrorMessage = "El código es obligatorio.")]
        [Display(Name = "Código de verificación")]
        public string Code { get; set; } = null!;
    }
}