using Microsoft.AspNetCore.Identity;

namespace ServiceFlow.Class.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; } = null!;
        public string PaternalSurname { get; set; } = null!;
        public string MaternalSurname { get; set; } = null!;

    }
}
