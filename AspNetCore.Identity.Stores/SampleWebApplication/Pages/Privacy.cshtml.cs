using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SampleWebApplication.Pages
{
    public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly UserManager<IdentityUser> userManager;

        public PrivacyModel(ILogger<PrivacyModel> logger, UserManager<IdentityUser> userManager)
        {
            _logger = logger;
            this.userManager = userManager;
        }

        public void OnGet()
        {
            
        }
    }
}