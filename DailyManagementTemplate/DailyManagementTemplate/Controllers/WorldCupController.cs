using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DailyManagementTemplate.Models;
using System.IO;
using System.Text;
using DailyManagementTemplate.Helpers;

namespace DailyManagementTemplate.Controllers
{
    public class WorldCupController : Controller
    {
        private WorldCupViewModel _worldCupModel;
        private Dictionary<GroupLetterEnum, IList<TeamViewModel>> _dicGroupTeams;
        private WorldCupDataEliminationFaseMatchesHelper _worldCupDataEliminationFaseMatchesHelper;

        public WorldCupController()
        {
            _worldCupModel = new WorldCupViewModel();
            _dicGroupTeams = new Dictionary<GroupLetterEnum, IList<TeamViewModel>>();
            foreach (GroupLetterEnum groupLetter in Enum.GetValues(typeof(GroupLetterEnum)))
            {
                _dicGroupTeams.Add(groupLetter, new List<TeamViewModel>());
            }

            _worldCupDataEliminationFaseMatchesHelper = new WorldCupDataEliminationFaseMatchesHelper();
        }

        public IActionResult Index() {
            _worldCupModel = new WorldCupViewModel();

            GetWorldCupMatches();
            GetWorldCupGroups();
            _worldCupDataEliminationFaseMatchesHelper.GetEliminationFaseRounds(_worldCupModel);
            _worldCupModel.NumbertOfRounds = Enum.GetValues(typeof(EliminationFaseRoundEnum)).Length;

            return View(_worldCupModel);
        }

        private void GetWorldCupMatches()
        {
            var worldCupFileData = WorldCupFileIOHelper.ReadFile();

            ParseWorldCupDataGroupMatches(worldCupFileData);

            //throw new NotImplementedException();
        }

        private void ParseWorldCupDataGroupMatches(string worldCupFileData)
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

                    MatchParseForGroupMatches(match, matchNumber, day, date);
                }
            }
        }

        private void MatchParseForGroupMatches(string match, int matchNumber, string day, string date)
        {
            if (matchNumber <= 48)
            {
                var groupMatchViewModel = new GroupMatchViewModel();
                
                SetGroup(match, groupMatchViewModel);
                SetHomeTeam(match, groupMatchViewModel.Match);
                SetAwayTeam(match, groupMatchViewModel.Match);
                SetScore(match, groupMatchViewModel.Match);

                groupMatchViewModel.Match.Day    = day;
                groupMatchViewModel.Match.Date   = date;
                groupMatchViewModel.Match.Number = matchNumber;

                _worldCupModel.GroupMatches.Add(groupMatchViewModel);
                var teams = _dicGroupTeams[groupMatchViewModel.GroupLetter];
                if(!teams.Any(t => t.TeamName == groupMatchViewModel.Match.HomeTeam.TeamName))
                {
                    teams.Add(groupMatchViewModel.Match.HomeTeam);
                }
                if (!teams.Any(t => t.TeamName == groupMatchViewModel.Match.AwayTeam.TeamName))
                {
                    teams.Add(groupMatchViewModel.Match.AwayTeam);
                }
            }
        }        

        private void SetScore(string match, MatchViewModel matchViewModel)
        {
            var startIndex = match.IndexOf('(');

            if (startIndex >= 0)
            {
                var score = match.Substring(startIndex + 1).Replace(')', ' ').Split('?');
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

        private void GetWorldCupGroups()
        {
            _worldCupModel.Groups = new List<GroupTableViewModel>();


            foreach (GroupLetterEnum groupLetter in Enum.GetValues(typeof(GroupLetterEnum)))
            {
                var teams = _dicGroupTeams[groupLetter];

                var groupMatchs = _worldCupModel.GroupMatches.Where(s => s.GroupLetter == groupLetter);

                var dicTeamsPoints = teams.ToDictionary(k => k.TeamName, v => 0);

                foreach (var groupMatch in groupMatchs)
                {
                    if(groupMatch.Match.HomeTeamScore.HasValue && groupMatch.Match.AwayTeamScore.HasValue) { 
                        if(groupMatch.Match.HomeTeamScore == groupMatch.Match.AwayTeamScore)
                        {
                            dicTeamsPoints[groupMatch.Match.HomeTeam.TeamName] += 1;
                            dicTeamsPoints[groupMatch.Match.AwayTeam.TeamName] += 1;
                        }
                        else if(groupMatch.Match.HomeTeamScore > groupMatch.Match.AwayTeamScore)
                        {
                            dicTeamsPoints[groupMatch.Match.HomeTeam.TeamName] += 3;
                        }
                        else if (groupMatch.Match.HomeTeamScore < groupMatch.Match.AwayTeamScore)
                        {
                            dicTeamsPoints[groupMatch.Match.AwayTeam.TeamName] += 3;
                        }
                    }
                }

                var model = new GroupTableViewModel();
                model.GroupLetter = groupLetter;

                foreach (var team in dicTeamsPoints.OrderByDescending(s => s.Value))
                {
                    model.GroupRows.Add(new GroupRowViewModel { Team = new TeamViewModel { TeamName = team.Key }, Points = team.Value });
                }                

                _worldCupModel.Groups.Add(model);
            }
        }
        

        [HttpGet]
        public IActionResult UpdateScore(string homeTeam, string awayTeam)
        {
            var viewModel = new UpdateScoreViewModel();
            viewModel.HomeTeam = homeTeam;
            viewModel.AwayTeam = awayTeam;
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateScore(UpdateScoreViewModel viewModel)
        {
            if (ModelState.IsValid
                && !string.IsNullOrEmpty(viewModel.HomeTeam)
                && viewModel.HomeTeamScore.HasValue
                && !string.IsNullOrEmpty(viewModel.AwayTeam)
                && viewModel.AwayTeamScore.HasValue
                )
            {
                if (UpdateMatchScore(viewModel))
                {
                    return RedirectToAction(nameof(Index));
                }

                ModelState.AddModelError(string.Empty, "Error updating match score");
            }
            
            return View(viewModel);
        }

        private bool UpdateMatchScore(UpdateScoreViewModel viewModel)
        {
            var data = WorldCupFileIOHelper.ReadFile();
            var updatedFile = ParseFileForMatchScore(data, viewModel);

            return WorldCupFileIOHelper.UpdateFile(updatedFile);
        }



        private StringBuilder ParseFileForMatchScore(string worldCupFileData, UpdateScoreViewModel viewModel)
        {
            var sb = new StringBuilder();

            using (var sr = new StringReader(worldCupFileData))
            {
                string line;

                // load header
                line = sr.ReadLine();
                sb.AppendLine(line);
                var matchFound = false;

                while ((line = sr.ReadLine()) != null && !matchFound)
                {
                    if(line.IndexOf('(') < 0) { 
                        var split = line.Split(',');
                        var match = split[3];
                    
                        var homeTeamIndex = match.IndexOf(viewModel.HomeTeam);
                        var awayTeamIndex = match.IndexOf(viewModel.AwayTeam);

                        if (homeTeamIndex > 0 
                            && awayTeamIndex > 0
                            && homeTeamIndex < awayTeamIndex)
                        {
                            line = line.Replace(viewModel.AwayTeam, $"{viewModel.AwayTeam} ({viewModel.HomeTeamScore}?{viewModel.AwayTeamScore})");
                            matchFound = true;
                        }
                    }

                    sb.AppendLine(line);
                }

                if (line != null) {
                    sb.AppendLine(line);

                    while ((line = sr.ReadLine()) != null)
                    {
                        sb.AppendLine(line);
                    }
                }                
            }

            return sb;
        }

        private string GetHomeTeam(string match)
        {
            var startIndex = match.IndexOf(']');
            var endIndex = match.IndexOf('-');
            var homeTeam = match.Substring((startIndex + 1), (endIndex - startIndex - 1));
            return homeTeam.Trim();
        }

        private string GetAwayTeam(string match)
        {

            var startIndex = match.IndexOf('-');
            var endIndex = match.IndexOf('(');

            if (endIndex < 0)
            {
                var awayTeam = match.Substring((startIndex + 1));
                return awayTeam.Trim();
            }
            else
            {
                var awayTeam = match.Substring((startIndex + 1), (endIndex - startIndex - 1));
                return awayTeam.Trim();
            }

        }
    }
}
