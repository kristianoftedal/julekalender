﻿using System.ComponentModel.DataAnnotations;

namespace ChristmasCalendar.Domain
{
    public class Door
    {
        public int Id { get; protected set; }

        public int Number { get; protected set; }

        public DateTime ForDate { get; protected set; }

        [MaxLength(256)]
        public string Location { get; protected set; } = null!;

        [MaxLength(256)]
        public string Country { get; protected set; } = null!;

        public string ImagePath { get; protected set; } = null!;

        [MaxLength(512)]
        public string? Description { get; protected set; }

        public int? PointsForCountry { get; protected set; }

        public int? PointsForLocation { get; protected set; }
    }
}
