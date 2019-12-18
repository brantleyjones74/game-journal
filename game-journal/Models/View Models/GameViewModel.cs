using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace game_journal.Models.View_Models
{
    public class GameViewModel
    {
        public Game Game { get; set; }
        public List<Genre> Genres { get; set; } = new List<Genre>();
        public virtual List<GameGenre> GameGenres { get; set; }
        public List<Platform> Platforms { get; set; } = new List<Platform>();
        public virtual List<GamePlatform> GamePlatforms { get; set; }
    }
}
