using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace game_journal.Models.View_Models
{
    public class GameViewModel
    {
        public Game Game { get; set; }
        public IEnumerable<Genre> Genres { get; set; }
        public virtual IEnumerable<GameGenre> GameGenres { get; set; }
        public IEnumerable<Platform> Platforms { get; set; }
        public virtual IEnumerable<GamePlatform> GamePlatforms { get; set; }
    }
}
