using System;

namespace DailyManagementTemplate.Models
{
    public class FantasyPremierLeagueModel
    {
        public List<PhaseModel> Phases { get; set; }

        public List<ElementsModel> Elements { get; set; }

        public StatsModel Stats { get; set; }

        public GameSettingsModel GameSettings { get; set; }

        public string CurrentEvent { get; set; }

        public string TotalPlayers { get; set; }

        public List<TeamsModel> Temas { get; set; }

        public List<ElementTypesModel> ElementTypes { get; set; }

        public string LastEntryEvent { get; set; }

        public List<StatsOptionsModel> StatsOptions { get; set; }

        public List<NextEventFixturesModel> NextEventFixtures { get; set; }

        public List<EventsModel> Events { get; set; }

        public string NextEvent { get; set; }
    }

    public class PhaseModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string NumWinners { get; set; }
        public string StartEvent { get; set; }
        public string StopEvent { get; set; }
    }

    public class ElementsModel
    {

    }

    public class StatsModel
    {
        public List<HeadingsModel> Events { get; set; }

        public string Categories { get; set; }
    }

    public class HeadingsModel 
    {
        public string Category { get; set; }
        public string Field { get; set; }
        public string Abbr { get; set; }
        public string Label { get; set; }
    }

    public class GameSettingsModel
    {

    }


    public class TeamsModel
    {

    }


    public class ElementTypesModel
    {
        public string Id { get; set; }
        public string SingularName { get; set; }
        public string SingularNameShot { get; set; }
        public string PluralName { get; set; }
        public string PluralNameShort { get; set; }
    }

    public class StatsOptionsModel
    {
        public string Name { get; set; }
        public string Key { get; set; }
    }

    public class NextEventFixturesModel
    {

    }

    public class EventsModel
    {

    }
}
