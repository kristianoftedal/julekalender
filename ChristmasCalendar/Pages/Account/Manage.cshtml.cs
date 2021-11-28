using ChristmasCalendar.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChristmasCalendar.Pages.Account
{
    public class ManageModel : PageModel
    {
        public string Name { get; set; } = null!;

        [BindProperty]
        public bool WantsDailyNotification { get; set; }

        private readonly UserManager<ApplicationUser> _userManager;

        public ManageModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        
        public async Task OnGet()
        {
            var user = await _userManager.GetUserAsync(User);

            Name = user.Name!;
            WantsDailyNotification = user.WantsDailyNotification;
        }

        public async Task OnPost()
        {
            var user = await _userManager.GetUserAsync(User);

            user.WantsDailyNotification = WantsDailyNotification;

            await _userManager.UpdateAsync(user);
        }
    }
}
