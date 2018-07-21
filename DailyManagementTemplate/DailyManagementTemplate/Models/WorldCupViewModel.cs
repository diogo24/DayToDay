using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyManagementTemplate.Models
{
    public class WorldCupViewModel
    {
        public WorldCupViewModel()
        {
            GroupMatches    = new HashSet<GroupMatchViewModel>();
            Groups          = new HashSet<GroupTableViewModel>();
            EliminationFase = new HashSet<EliminationFaseViewModel>();
        }

        public ICollection<GroupMatchViewModel> GroupMatches { get; set; }

        public ICollection<GroupTableViewModel> Groups { get; set; }

        public ICollection<EliminationFaseViewModel> EliminationFase { get; set; }

        public int NumbertOfRounds { get; set; }
    }

    public class GroupMatchViewModel {

        public GroupMatchViewModel()
        {
            Match = new MatchViewModel();
        }

        public MatchViewModel Match { get; set; }

        public GroupLetterEnum GroupLetter { get; set; }
    }

    public class EliminationFaseViewModel
    {
        public EliminationFaseViewModel()
        {
            Match = new MatchViewModel();
        }

        public MatchViewModel Match { get; set; }

        public EliminationFaseRoundEnum EliminationFaaseRound { get; set; }

        public int RoundIndex { get; set; }
    }

    public enum EliminationFaseRoundEnum {
        Round16          = 1,
        QuarterFinals    = 2,
        SemiFinals       = 3,
        ThirdFourthPlace = 4,
        Final            = 5
    }

    public class GroupTableViewModel {

        public GroupTableViewModel()
        {
            GroupRows = new HashSet<GroupRowViewModel>();
        }

        public ICollection<GroupRowViewModel> GroupRows { get; set; }

        public GroupLetterEnum GroupLetter { get; set; }
    }

    public enum GroupLetterEnum {
        A = 1,
        B = 2,
        C = 3,
        D = 4,
        E = 5,
        F = 6,
        G = 7,
        H = 8
    }

    public class GroupRowViewModel {
        public TeamViewModel Team { get; set; }
        public GroupPositionEnum GroupPosition { get; set; }

        public int GamesPlayed{ get; set; }
        public int Points { get; set; }

        public int Wins { get; set; }
        public int Draws { get; set; }
        public int Defeats { get; set; }

        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
    }

    public enum GroupPositionEnum {
        First  = 1,
        Second = 2,
        Third  = 3,
        Fourth = 4
    }

    public class MatchViewModel {
        public TeamViewModel HomeTeam { get; set; }
        public int? HomeTeamScore { get; set; }


        public TeamViewModel AwayTeam { get; set; }
        public int? AwayTeamScore { get; set; }
        public string Day { get; internal set; }
        public string Date { get; internal set; }
        public int Number { get; internal set; }
    }

    public class TeamViewModel {
        public int TeamID { get; set; }
        public string TeamName { get; set; }
    }
}
