using ChristmasCalendar.Domain;

namespace ChristmasCalendar.Data
{
    public interface IDatabasePersister
    {
        Task RegisterFirstTimeOpeningDoor(string userId, Door doorForToday);
        Task RegisterAnswer(string userId, int doorId, string? location, string? country);
    }

    public class DatabasePersister : IDatabasePersister
    {
        private readonly ApplicationDbContext _context;

        public DatabasePersister(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task RegisterAnswer(string userId, int doorId, string? location, string? country)
        {
            _context.Answers.Add(Answer.Create(userId, doorId, location, country));

            await _context.SaveChangesAsync();
        }

        public async Task RegisterFirstTimeOpeningDoor(string userId, Door door)
        {
            _context.FirstTimeOpeningDoor.Add(FirstTimeOpeningDoor.Register(userId, door.Id));

            await _context.SaveChangesAsync();
        }
    }
}
