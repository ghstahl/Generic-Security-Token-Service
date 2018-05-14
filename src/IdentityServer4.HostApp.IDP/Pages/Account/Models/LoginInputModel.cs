using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace IdentityServer4.HostApp.IDP.Pages.Account.Models
{
    public class LoginInputModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        public bool RememberLogin { get; set; }
        public string ReturnUrl { get; set; }
    }
}
