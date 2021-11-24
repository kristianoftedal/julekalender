using ChristmasCalendar.Data;
using ChristmasCalendar.Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChristmasCalendar.Pages.Doors
{
    public class PreviousDoorsModel : PageModel
    {
        public IList<Door> PreviousDoors { get; set; } = null!;

        private readonly IDatabaseQueries _databaseQueries;

        public PreviousDoorsModel(IDatabaseQueries databaseQueries)
        {
            _databaseQueries = databaseQueries;
        }

        public async Task OnGetAsync()
        {
            PreviousDoors = await _databaseQueries.GetHistoricDoors(DateTime.Today);
        }
    }
}
