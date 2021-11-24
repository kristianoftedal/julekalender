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
            var scores = await _databaseQueries.GetScores(DateTime.Now.Year);

            int currentRank = 0;
            int prevPoints = -1;

            for (int i = 0; i < scores.Count; i++)
            {
                if (scores[i].PointsTotal != prevPoints)
                {
                    currentRank = i + 1;
                    prevPoints = scores[i].PointsTotal;
                }
                scores[i].Rank = currentRank;
            }

            Scores = scores;

            var lastUpdated = (await _databaseQueries.GetWhenScoreWasLastUpdated());

            LastUpdated = lastUpdated != DateTime.MinValue ? lastUpdated : (DateTime?)null;
        }
    }
}
