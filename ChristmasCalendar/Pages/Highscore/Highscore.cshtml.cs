using ChristmasCalendar.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ChristmasCalendar.Pages.Highscore
{
    public class HighscoreModel : PageModel
    {
        public IList<HighscoreViewModel> Scores { get; set; } = null!;

        public DateTime? LastUpdated { get; set; }

        private readonly IDatabaseQueries _databaseQueries;

        public HighscoreModel(IDatabaseQueries databaseQueries)
        {
            _databaseQueries = databaseQueries;
        }

        public async Task OnGetAsync()
        {
            Scores = await _databaseQueries.GetScores(DateTime.Now.Year);

            var lastUpdated = (await _databaseQueries.GetWhenScoreWasLastUpdated());

            LastUpdated = lastUpdated != DateTime.MinValue ? lastUpdated : null;
        }
    }
}
