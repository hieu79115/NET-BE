using Microsoft.AspNetCore.Identity;

namespace NET_BE.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string Role { get; set; }
    }
}
