using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace DailyManagementTemplate.Models
{
    public class UpdateScoreViewModel
    {
        [Required]
        public string HomeTeam { get; set; }
        [Required]
        public int? HomeTeamScore { get; set; }
        [Required]
        public int? HomeTeamScorePenalties { get; set; }

        [Required]
        public string AwayTeam { get; set; }
        [Required]
        public int? AwayTeamScore { get; set; }
        [Required]
        public int? AwayTeamScorePenalties { get; set; }
        

        [Required]
        public int MatchNumber { get; set; }
    }
}
