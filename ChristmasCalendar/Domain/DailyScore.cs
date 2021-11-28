namespace ChristmasCalendar.Domain
{
    public class DailyScore
    {
        public int Id { get; set; }

        public int DoorId { get; set; }

        public int? DoorNumber { get; set; }

        public string UserId { get; set; } = null!;

        public int Points { get; set; }

        public int Bonus { get; set; }

        public int Rank { get; set; }

        public int RankMovement { get; set; }

        public int TimeToAnswer { get; set; }

        public int Year { get; set; }

        public string NameOfUser { get; set; } = null!;
    }
}
