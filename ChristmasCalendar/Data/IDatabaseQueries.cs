﻿using ChristmasCalendar.Pages.Highscore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChristmasCalendar.Data
{
    public interface IDatabaseQueries
    {
        Task<List<Door>> GetHistoricDoors(DateTime now);

        Task<List<HighscoreViewModel>> GetScores();

        Task<Door> GetTodaysDoor(DateTime today);

        Task<List<Answer>> GetRegisteredAnswersForDoor(string userId, int doorId);

        Task<bool> HasOpenedDoor(string userId, int doorId);

        Task<FirstTimeOpeningDoor> GetFirstTimeOpeningDoor(string userId, int doorId);

        Task<DateTime> GetWhenScoreWasLastUpdated();
    }
}
