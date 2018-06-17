using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyManagementTemplate.Models;
using System.IO;
using System.Text;

namespace DailyManagementTemplate.Controllers
{
    public class WorldCupController : Controller
    {

        public IActionResult Index() {
            var worldCupModel = new WorldCupViewModel();

            GetWorldCupMatches(worldCupModel);
            worldCupModel.Groups          = GetWorldCupGroups();
            worldCupModel.EliminationFase = GetEliminationFaseRounds();
            worldCupModel.NumbertOfRounds = Enum.GetValues(typeof(EliminationFaseRoundEnum)).Length;

            return View(worldCupModel);
        }

        private void GetWorldCupMatches(WorldCupViewModel worldCupViewModel)
        {
            var worldCupFileData = ReadFile();

            ParseWorldCupData(worldCupFileData, worldCupViewModel);

            //throw new NotImplementedException();
        }

        private void ParseWorldCupData(string worldCupFileData, WorldCupViewModel worldCupViewModel)
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
                    var date        = split[1];
                    var match       = split[2];
                    var matchNumber = int.Parse(split[3]);

                    MatchParse(match, matchNumber, day, date);
                }
            }
        }

        private void MatchParse(string match, int matchNumber, string day, string date)
        {
            if (matchNumber <= 48)
            {
                var groupMatchViewModel = new GroupMatchViewModel();
                
                SetGroup(match, groupMatchViewModel);
                SetHomeTeam(match, groupMatchViewModel.Match);
                SetAwayTeam(match, groupMatchViewModel.Match);
                SetScore(match, groupMatchViewModel.Match);

                groupMatchViewModel.Match.Day  = day;
                groupMatchViewModel.Match.Date = date;
            }
            else {

            }
        }

        private void SetScore(string match, MatchViewModel matchViewModel)
        {
            var startIndex = match.IndexOf('(');

            if (startIndex >= 0)
            {
                var score = match.Substring(startIndex + 1).Remove(')').Split('?');
                matchViewModel.HomeTeamScore = int.Parse(score[0]);
                matchViewModel.AwayTeamScore = int.Parse(score[1]);
            }
        }

        private void SetAwayTeam(string match, MatchViewModel matchViewModel)
        {

            var startIndex = match.IndexOf('-');
            var endIndex = match.IndexOf('(');

            if (endIndex < 0)
            {
                var awayTeam = match.Substring((startIndex + 1));
                matchViewModel.AwayTeam = new TeamViewModel { TeamName = awayTeam.Trim() };
            }
            else {
                var awayTeam = match.Substring((startIndex + 1), (endIndex - startIndex - 1));
                matchViewModel.AwayTeam = new TeamViewModel { TeamName = awayTeam.Trim() };
            }
            
        }

        private void SetHomeTeam(string match, MatchViewModel matchViewModel)
        {
            var startIndex = match.IndexOf(']');
            var endIndex = match.IndexOf('-');
            var homeTeam = match.Substring((startIndex + 1), (endIndex - startIndex - 1));
            matchViewModel.HomeTeam = new TeamViewModel { TeamName = homeTeam.Trim() };
        }

        private void SetGroup(string match, GroupMatchViewModel groupMatchViewModel)
        {
            var startIndex = match.IndexOf('[');
            var groupLetter = match.Substring(startIndex + 1, 1);
            groupMatchViewModel.GroupLetter = Enum.Parse<GroupLetterEnum>(groupLetter);            
        }

        private static string ReadFile()
        {
            string result = string.Empty;

            using (var streamReader = new StreamReader(@"wwwroot\docs\World Cup 2018 Games List.csv"))
            {
                result = streamReader.ReadToEnd();
            }

            return result;
        }

        private ICollection<GroupTableViewModel> GetWorldCupGroups()
        {
            throw new NotImplementedException();
        }

        private ICollection<EliminationFaseViewModel> GetEliminationFaseRounds()
        {
            throw new NotImplementedException();
        }
    }
}
