using ChristmasCalendar.Data.Dapper;
using ChristmasCalendar.Domain;
using ChristmasCalendar.Pages.Highscore;
using Dapper;
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
        private readonly DbConnectionFactory _dbConnectionFactory;

        public DatabaseQueries(ApplicationDbContext context, DbConnectionFactory dbConnectionFactory)
        {
            _context = context;
            _dbConnectionFactory = dbConnectionFactory;
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

        public async Task<List<HighscoreViewModel>> GetScores(int year)
        {
            using var connection = _dbConnectionFactory.Connection;

            var sql = @"
                DECLARE @doorNumberForLastDoor INT
                SELECT @doorNumberForLastDoor = ISNULL(MAX(y.[DoorNumber]), 0) FROM [dbo].[DailyScore] y (NOLOCK) WHERE y.[Year] = 2021

                SELECT 
	                [NameOfUser],
	                SUM((CASE WHEN ds.DoorId = @doorNumberForLastDoor THEN ds.[Rank] ELSE 0 END)) as [Rank],
	                SUM(ds.[Bonus]) as [Bonus],
	                0 as [NumberOfCorrectDoors],--NA
	                SUM(ds.[TimeToAnswer]) as [TotalTimeToAnswer],
	                AVG(ds.[TimeToAnswer]) as [AverageSecondsSpentPerCorrectDoor],
	                SUM((CASE WHEN ds.DoorId = @doorNumberForLastDoor THEN ds.[Points] ELSE 0 END)) as [PointsLastDoor],
	                SUM(ds.[Points]) as [PointsTotal]
                FROM 
	                [dbo].[DailyScore] ds (NOLOCK)
                WHERE
	                ds.[Year] = @year
                GROUP BY
	                ds.[UserId], ds.[NameOfUser]
                ORDER BY
	                [PointsTotal] DESC, 
	                [NameOfUser] ASC";

            return (await connection.QueryAsync<HighscoreViewModel>(sql, new { year })).ToList();
        }

        public async Task<List<HighscoreViewModel>> GetScoresSortedByTime(int year)
        {
            using var connection = _dbConnectionFactory.Connection;

            var sql = @"
                DECLARE @doorNumberForLastDoor INT
                SELECT @doorNumberForLastDoor = ISNULL(MAX(y.[DoorNumber]), 0) FROM [dbo].[DailyScore] y (NOLOCK) WHERE y.[Year] = @year

                SELECT 
	                [NameOfUser],
	                1 as [Rank],
	                SUM(ds.[Bonus]) as [Bonus],
	                0 as [NumberOfCorrectDoors],
	                SUM(ds.[TimeToAnswer]) as [TotalTimeToAnswer],
	                AVG(ds.[TimeToAnswer]) as [AverageSecondsSpentPerCorrectDoor],
	                SUM((CASE WHEN ds.DoorId = @doorNumberForLastDoor THEN ds.[Points] ELSE 0 END)) as [PointsLastDoor],
	                SUM(ds.[Points]) as [PointsTotal]
                FROM 
	                [dbo].[DailyScore] ds (NOLOCK)
                WHERE
	                ds.[Year] = @year
                GROUP BY
	                ds.[UserId], ds.[NameOfUser]
                ORDER BY
	                [PointsTotal] DESC, 
	                [TotalTimeToAnswer] ASC,
	                [AverageSecondsSpentPerCorrectDoor] ASC,
	                [NameOfUser] ASC";

            return (await connection.QueryAsync<HighscoreViewModel>(sql, new { year })).ToList();
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
