namespace ChristmasCalendar.Domain
{
    public class FirstTimeOpeningDoor
    {
        public int Id { get; protected set; }

        public string UserId { get; protected set; } = null!;

        public int DoorId { get; protected set; }

        public DateTime When { get; protected set; }

        public static FirstTimeOpeningDoor Register(string userId, int doorId)
        {
            return new FirstTimeOpeningDoor
            {
                UserId = userId,
                DoorId = doorId,
                When = DateTime.Now
            };
        }
    }
}
