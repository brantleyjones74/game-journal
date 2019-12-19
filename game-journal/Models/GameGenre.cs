using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace game_journal.Models
{
    public class GameGenre
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public int GameId { get; set; }
        public Game Game { get; set; }

        [Required]
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
    }
}
