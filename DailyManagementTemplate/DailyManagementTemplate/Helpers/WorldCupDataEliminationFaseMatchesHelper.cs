using DailyManagementTemplate.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DailyManagementTemplate.Helpers
{
    public class WorldCupDataEliminationFaseMatchesHelper
    {
        private WorldCupViewModel _worldCupModel;

        internal void GetEliminationFaseRounds(WorldCupViewModel worldCupModel)
        {
            var worldCupFileData = WorldCupFileIOHelper.ReadFile();

            _worldCupModel = worldCupModel;

            ParseWorldCupDataEliminationFaseMatches(worldCupFileData);
        }

        private void ParseWorldCupDataEliminationFaseMatches(string worldCupFileData)
        {
            using (var sr = new StringReader(worldCupFileData))
            {
                string line;

                // load header
                line = sr.ReadLine();

                while ((line = sr.ReadLine()) != null)
                {
                    var split       = line.Split(',');
                    var day         = split[0];
                    var date        = split[1] + split[2];
                    var match       = split[3];
                    var matchNumber = int.Parse(split[4]);

                    MatchParseForEliminationFaseMatches(match, matchNumber, day, date);
                }
            }
        }

        private void MatchParseForEliminationFaseMatches(string match, int matchNumber, string day, string date)
        {
            if (matchNumber > 48)
            {
                var eliminationMatch = new EliminationFaseViewModel();

                SetRound(match, eliminationMatch);

                var endIndex = match.IndexOf(']');
                match = match.Substring(endIndex + 1);
                var matchSplit = match.Split('-');

                if (eliminationMatch.EliminationFaaseRound == EliminationFaseRoundEnum.Round16)
                {
                    SetEliminationR16HomeTeam(matchSplit[0], eliminationMatch.Match);
                    SetEliminationR16AwayTeam(matchSplit[1], eliminationMatch.Match);
                    SetEliminationR16SetScore(matchSplit[1], eliminationMatch.Match);
                }

                if (eliminationMatch.EliminationFaaseRound == EliminationFaseRoundEnum.QuarterFinals
                    || eliminationMatch.EliminationFaaseRound == EliminationFaseRoundEnum.SemiFinals
                    || eliminationMatch.EliminationFaaseRound == EliminationFaseRoundEnum.Final)
                {
                    SetEliminationR8HomeTeam(matchSplit[0], eliminationMatch.Match);
                    SetEliminationR8AwayTeam(matchSplit[1], eliminationMatch.Match);
                    SetEliminationR16SetScore(matchSplit[1], eliminationMatch.Match);
                }

                if (eliminationMatch.EliminationFaaseRound == EliminationFaseRoundEnum.ThirdFourthPlace)
                {
                    SetElimination34PHomeTeam(matchSplit[0], eliminationMatch.Match);
                    SetElimination34PAwayTeam(matchSplit[1], eliminationMatch.Match);
                    SetEliminationR16SetScore(matchSplit[1], eliminationMatch.Match);
                }

                eliminationMatch.Match.Day = day;
                eliminationMatch.Match.Date = date;
                eliminationMatch.Match.Number = matchNumber;

                if(eliminationMatch.Match.HomeTeam != null) { 
                    _worldCupModel.EliminationFase.Add(eliminationMatch);
                }
            }
        }

        private void SetRound(string match, EliminationFaseViewModel eliminationMatch)
        {
            var startIndex = match.IndexOf('[');
            var endIndex = match.IndexOf(']');

            if (startIndex >= 0 && endIndex > 0 && endIndex > startIndex)
            {
                var round = match.Substring((startIndex + 1), (endIndex - startIndex - 1));
                switch (round)
                {
                    case "R16":
                        eliminationMatch.EliminationFaaseRound = EliminationFaseRoundEnum.Round16;

                        break;
                    case "R8":
                        eliminationMatch.EliminationFaaseRound = EliminationFaseRoundEnum.QuarterFinals;

                        break;
                    case "R4":
                        eliminationMatch.EliminationFaaseRound = EliminationFaseRoundEnum.SemiFinals;

                        break;
                    case "3rd":
                        eliminationMatch.EliminationFaaseRound = EliminationFaseRoundEnum.ThirdFourthPlace;

                        break;
                    case "final":
                        eliminationMatch.EliminationFaaseRound = EliminationFaseRoundEnum.Final;

                        break;
                    default:
                        break;
                }
            }
        }

        private void SetEliminationR16HomeTeam(string match, MatchViewModel eliminationMatch)
        {
            var groupLetter = match.Replace('1', ' ').Trim();
            var groupLetterEnum = Enum.Parse<GroupLetterEnum>(groupLetter);

            var team = _worldCupModel.Groups.Where(s => s.GroupLetter == groupLetterEnum).FirstOrDefault()?.GroupRows.OrderByDescending(s => s.Points).FirstOrDefault();
            eliminationMatch.HomeTeam = team.Team;
        }

        private void SetEliminationR16AwayTeam(string match, MatchViewModel eliminationMatch)
        {
            var endIndex = match.IndexOf('(');

            if (endIndex < 0)
            {
                var groupLetter = match.Replace('2', ' ').Trim();
                var groupLetterEnum = Enum.Parse<GroupLetterEnum>(groupLetter);

                var team = _worldCupModel.Groups.Where(s => s.GroupLetter == groupLetterEnum).FirstOrDefault()?.GroupRows.OrderByDescending(s => s.Points).Skip(1).Take(1).FirstOrDefault();
                eliminationMatch.AwayTeam = team.Team;
            }
            else
            {
                var groupLetter = match.Substring(0, endIndex).Replace('2', ' ').Trim();
                var groupLetterEnum = Enum.Parse<GroupLetterEnum>(groupLetter);

                var team = _worldCupModel.Groups.Where(s => s.GroupLetter == groupLetterEnum).FirstOrDefault()?.GroupRows.OrderByDescending(s => s.Points).Skip(1).Take(1).FirstOrDefault();
                eliminationMatch.AwayTeam = team.Team;
            }
        }

        private void SetEliminationR16SetScore(string match, MatchViewModel eliminationMatch)
        {
            var scoresCount = match.Count(s => s == '(');
            switch (scoresCount)
            {
                case 1:
                    var startIndex = match.IndexOf('(');

                    if (startIndex >= 0)
                    {
                        var score = match.Substring(startIndex + 1).Replace(')', ' ').Split('?');
                        eliminationMatch.HomeTeamScore = int.Parse(score[0]);
                        eliminationMatch.AwayTeamScore = int.Parse(score[1]);
                    }
                    break;
                case 2:
                    var startIndex2 = match.IndexOf('(');
                    var endIndex2 = match.IndexOf(')');

                    if (startIndex2 >= 0)
                    {
                        var score = match.Substring(startIndex2 + 1, (endIndex2 - startIndex2 - 1)).Replace(')', ' ').Split('?');
                        eliminationMatch.HomeTeamScore = int.Parse(score[0]);
                        eliminationMatch.AwayTeamScore = int.Parse(score[1]);
                    }

                    var startIndex2P = match.LastIndexOf('(');
                    var endIndex2P = match.LastIndexOf(')');

                    if (startIndex2P >= 0)
                    {
                        var score = match.Substring(startIndex2P + 1, (endIndex2P - startIndex2P - 1)).Replace(')', ' ').Split('?');
                        eliminationMatch.HomeTeamScorePenalties = int.Parse(score[0]);
                        eliminationMatch.AwayTeamScorePenalties = int.Parse(score[1]);
                    }

                    break;
                default:
                    break;
            }

            //var startIndex = match.IndexOf('(');
            //var endIndex = match.IndexOf(')');

            //if (startIndex >= 0)
            //{
            //    var score = match.Substring(startIndex + 1).Replace(')', ' ').Split('?');
            //    eliminationMatch.HomeTeamScore = int.Parse(score[0]);
            //    eliminationMatch.AwayTeamScore = int.Parse(score[1]);
            //}
                        
        }

        private void SetEliminationR8HomeTeam(string match, MatchViewModel eliminationMatch)
        {
            var matchNumber = int.Parse(match.Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault());

            var elimFaseMatch = _worldCupModel.EliminationFase.FirstOrDefault(m => m.Match.Number == matchNumber)?.Match;

            if (elimFaseMatch.HomeTeamScore.HasValue &&
                elimFaseMatch.HomeTeamScore == elimFaseMatch.AwayTeamScore)
            {
                if (elimFaseMatch.HomeTeamScorePenalties > elimFaseMatch.AwayTeamScorePenalties )
                {
                    eliminationMatch.HomeTeam = elimFaseMatch.HomeTeam;
                }
                else if (elimFaseMatch.HomeTeamScorePenalties < elimFaseMatch.AwayTeamScorePenalties)
                {
                    eliminationMatch.HomeTeam = elimFaseMatch.AwayTeam;
                }
            }
            else if (elimFaseMatch.HomeTeamScore > elimFaseMatch.AwayTeamScore)
            {
                eliminationMatch.HomeTeam = elimFaseMatch.HomeTeam;
            }
            else
            {
                eliminationMatch.HomeTeam = elimFaseMatch.AwayTeam;
            }            
        }

        private void SetEliminationR8AwayTeam(string match, MatchViewModel eliminationMatch)
        {
            var endIndex = match.IndexOf('(');

            if (endIndex < 0)
            {
                var matchNumber = int.Parse(match.Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault());

                var elimFaseMatch = _worldCupModel.EliminationFase.FirstOrDefault(m => m.Match.Number == matchNumber)?.Match;

                if (elimFaseMatch.HomeTeamScore.HasValue &&
                    elimFaseMatch.HomeTeamScore == elimFaseMatch.AwayTeamScore)
                {
                    if (elimFaseMatch.HomeTeamScorePenalties > elimFaseMatch.AwayTeamScorePenalties)
                    {
                        eliminationMatch.AwayTeam = elimFaseMatch.HomeTeam;
                    }
                    else if (elimFaseMatch.HomeTeamScorePenalties < elimFaseMatch.AwayTeamScorePenalties)
                    {
                        eliminationMatch.AwayTeam = elimFaseMatch.AwayTeam;
                    }
                }
                else if (elimFaseMatch.HomeTeamScore >= elimFaseMatch.AwayTeamScore)
                {
                    eliminationMatch.AwayTeam = elimFaseMatch.HomeTeam;
                }
                else
                {
                    eliminationMatch.AwayTeam = elimFaseMatch.AwayTeam;
                }
            }
            else
            {
                var matchNumber = int.Parse(match.Substring(0, endIndex).Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault());

                var elimFaseMatch = _worldCupModel.EliminationFase.FirstOrDefault(m => m.Match.Number == matchNumber)?.Match;

                if (elimFaseMatch.HomeTeamScore.HasValue &&
                    elimFaseMatch.HomeTeamScore == elimFaseMatch.AwayTeamScore)
                {
                    if (elimFaseMatch.HomeTeamScorePenalties > elimFaseMatch.AwayTeamScorePenalties)
                    {
                        eliminationMatch.AwayTeam = elimFaseMatch.HomeTeam;
                    }
                    else if (elimFaseMatch.HomeTeamScorePenalties < elimFaseMatch.AwayTeamScorePenalties)
                    {
                        eliminationMatch.AwayTeam = elimFaseMatch.AwayTeam;
                    }
                }
                else if (elimFaseMatch.HomeTeamScore > elimFaseMatch.AwayTeamScore)
                {
                    eliminationMatch.AwayTeam = elimFaseMatch.HomeTeam;
                }
                else
                {
                    eliminationMatch.AwayTeam = elimFaseMatch.AwayTeam;
                }
            }
        }

        private void SetElimination34PHomeTeam(string match, MatchViewModel eliminationMatch)
        {
            var matchNumber = int.Parse(match.Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault());

            var elimFaseMatch = _worldCupModel.EliminationFase.FirstOrDefault(m => m.Match.Number == matchNumber)?.Match;

            if (elimFaseMatch.HomeTeamScore.HasValue &&
                elimFaseMatch.HomeTeamScore == elimFaseMatch.AwayTeamScore)
            {
                if (elimFaseMatch.HomeTeamScorePenalties < elimFaseMatch.AwayTeamScorePenalties)
                {
                    eliminationMatch.AwayTeam = elimFaseMatch.HomeTeam;
                }
                else if (elimFaseMatch.HomeTeamScorePenalties > elimFaseMatch.AwayTeamScorePenalties)
                {
                    eliminationMatch.AwayTeam = elimFaseMatch.AwayTeam;
                }
            }
            else if (elimFaseMatch.HomeTeamScore < elimFaseMatch.AwayTeamScore)
            {
                eliminationMatch.HomeTeam = elimFaseMatch.HomeTeam;
            }
            else
            {
                eliminationMatch.HomeTeam = elimFaseMatch.AwayTeam;
            }
        }

        private void SetElimination34PAwayTeam(string match, MatchViewModel eliminationMatch)
        {

            var endIndex = match.IndexOf('(');
            if (endIndex < 0)
            {

                var matchNumber = int.Parse(match.Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault());

                var elimFaseMatch = _worldCupModel.EliminationFase.FirstOrDefault(m => m.Match.Number == matchNumber)?.Match;

                if (elimFaseMatch.HomeTeamScore.HasValue &&
                    elimFaseMatch.HomeTeamScore == elimFaseMatch.AwayTeamScore)
                {
                    if (elimFaseMatch.HomeTeamScorePenalties < elimFaseMatch.AwayTeamScorePenalties)
                    {
                        eliminationMatch.AwayTeam = elimFaseMatch.HomeTeam;
                    }
                    else if (elimFaseMatch.HomeTeamScorePenalties > elimFaseMatch.AwayTeamScorePenalties)
                    {
                        eliminationMatch.AwayTeam = elimFaseMatch.AwayTeam;
                    }
                }
                else if (elimFaseMatch.HomeTeamScore < elimFaseMatch.AwayTeamScore)
                {
                    eliminationMatch.AwayTeam = elimFaseMatch.HomeTeam;
                }
                else
                {
                    eliminationMatch.AwayTeam = elimFaseMatch.AwayTeam;
                }
            }
            else
            {
                var matchNumber = int.Parse(match.Substring(0, endIndex).Split(' ', StringSplitOptions.RemoveEmptyEntries).LastOrDefault());

                var elimFaseMatch = _worldCupModel.EliminationFase.FirstOrDefault(m => m.Match.Number == matchNumber)?.Match;

                if (elimFaseMatch.HomeTeamScore.HasValue &&
                    elimFaseMatch.HomeTeamScore == elimFaseMatch.AwayTeamScore)
                {
                    if (elimFaseMatch.HomeTeamScorePenalties < elimFaseMatch.AwayTeamScorePenalties)
                    {
                        eliminationMatch.AwayTeam = elimFaseMatch.HomeTeam;
                    }
                    else if (elimFaseMatch.HomeTeamScorePenalties > elimFaseMatch.AwayTeamScorePenalties)
                    {
                        eliminationMatch.AwayTeam = elimFaseMatch.AwayTeam;
                    }
                }
                else if (elimFaseMatch.HomeTeamScore < elimFaseMatch.AwayTeamScore)
                {
                    eliminationMatch.AwayTeam = elimFaseMatch.HomeTeam;
                }
                else
                {
                    eliminationMatch.AwayTeam = elimFaseMatch.AwayTeam;
                }
            }
        }
    }
}
