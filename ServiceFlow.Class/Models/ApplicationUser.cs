using Microsoft.AspNetCore.Identity;

namespace ServiceFlow.Class.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string PaternalSurname { get; set; }
        public string MaternalSurname { get; set; }

    }
}
