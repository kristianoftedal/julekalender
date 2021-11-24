using ChristmasCalendar.Domain;
using ChristmasCalendar.Pages.Highscore;
using Microsoft.EntityFrameworkCore;

namespace ChristmasCalendar.Data
{
    public interface IDatabaseQueries
    {
        Task<List<Door>> GetHistoricDoors(DateTime now);
        Task<List<HighscoreViewModel>> GetScores(int year);
        Task<Door?> GetTodaysDoor(DateTime today);
        Task<Door?> GetNextDoor(DateTime today);
        Task<List<Answer>> GetRegisteredAnswersForDoor(string userId, int doorId);
        Task<bool> HasOpenedDoor(string userId, int doorId);
        Task<FirstTimeOpeningDoor?> GetFirstTimeOpeningDoor(string userId, int doorId);
        Task<DateTime> GetWhenScoreWasLastUpdated();
        Task<List<HighscoreViewModel>> GetScoresSortedByTime(int year);
    }

    public class DatabaseQueries : IDatabaseQueries
    {
        private readonly ApplicationDbContext _context;

        public DatabaseQueries(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task<List<Door>> GetHistoricDoors(DateTime now)
        {
            return _context.Doors
                .Where(x => x.ForDate.Year == now.Year && x.ForDate < now)
                .OrderByDescending(x => x.ForDate)
                .ToListAsync();
        }

        public Task<List<Answer>> GetRegisteredAnswersForDoor(string userId, int doorId)
        {
            return _context.Answers
                .Where(x => x.DoorId == doorId && x.UserId == userId)
                .OrderByDescending(x => x.When)
                .ToListAsync();
        }

        public Task<List<HighscoreViewModel>> GetScores(int year)
        {
            var doorNumber = _context.DailyScore.Where(x => x.Year == year).Max(x => x.DoorNumber);

            return _context.DailyScore
                .Where(x => x.Year == year)
                .GroupBy(x => new { x.UserId, x.NameOfUser })
                .Select(x => new HighscoreViewModel
                {
                    NameOfUser = x.Key.NameOfUser,
                    PointsTotal = x.Sum(y => y.Points),
                    PointsLastDoor = x.Where(y => y.DoorNumber == doorNumber).Sum(y => y.Points),
                    Rank = x.OrderByDescending(y => y.DoorNumber).First().Rank,
                    Bonus = x.Sum(y => y.Bonus),
                    AverageSecondsSpentPerCorrectDoor = (int)x.Where(y => y.Points == 2).DefaultIfEmpty().Average(y => y!.TimeToAnswer)
                })
                .OrderByDescending(x => x.PointsTotal)
                .ThenBy(x => x.NameOfUser)
                .ToListAsync();
        }
        public Task<List<HighscoreViewModel>> GetScoresSortedByTime(int year)
        {
            var doorNumberLastDoor = _context.DailyScore.Where(x => x.Year == year).Max(x => x.DoorNumber);

            return _context.DailyScore
                .Where(x => x.Year == year && x.TimeToAnswer > 0)
                .GroupBy(x => new { x.UserId, x.NameOfUser })
                .Select(x => new HighscoreViewModel
                {
                    NameOfUser = x.Key.NameOfUser,
                    PointsTotal = x.Sum(y => y.Points),
                    PointsLastDoor = x.Where(y => y.DoorNumber == doorNumberLastDoor).Sum(y => y.Points),
                    Rank = x.OrderByDescending(y => y.DoorNumber).First().Rank,
                    Bonus = x.Sum(y => y.Bonus),
                    TotalTimeToAnswer = (int)x.DefaultIfEmpty().Sum(y => y!.TimeToAnswer),
                    AverageSecondsSpentPerCorrectDoor = (int)x.DefaultIfEmpty().Average(y => y!.TimeToAnswer)
                })
                .Where(x => x.PointsTotal > doorNumberLastDoor)
                .OrderByDescending(x => x.PointsTotal)
                .ThenBy(x => x.TotalTimeToAnswer)
                .ThenBy(x => x.AverageSecondsSpentPerCorrectDoor)
                .ThenBy(x => x.NameOfUser)
                .ToListAsync();
        }

        public Task<Door?> GetTodaysDoor(DateTime today)
        {
            return _context.Doors.SingleOrDefaultAsync(x => x.ForDate == today);
        }

        public Task<Door?> GetNextDoor(DateTime today)
        {
            return _context.Doors
                .Where(x => x.ForDate > today)
                .Where(x => x.ForDate < new DateTime(today.Year + 1, 1, 1))
                .OrderBy(x => x.ForDate)
                .FirstOrDefaultAsync();
        }

        public Task<bool> HasOpenedDoor(string userId, int doorId)
        {
            return _context.FirstTimeOpeningDoor.AnyAsync(x => x.UserId == userId && x.DoorId == doorId);
        }

        public Task<FirstTimeOpeningDoor?> GetFirstTimeOpeningDoor(string userId, int doorId)
        {
            return _context.FirstTimeOpeningDoor.SingleOrDefaultAsync(x => x.UserId == userId && x.DoorId == doorId);
        }

        public Task<DateTime> GetWhenScoreWasLastUpdated()
        {
            return _context.DailyScoreLastUpdated.Select(x => x.LastUpdated).SingleOrDefaultAsync();
        }
    }
}
