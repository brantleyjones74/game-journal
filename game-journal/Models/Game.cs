using System;
using System.ComponentModel.DataAnnotations;

namespace game_journal.Models
{
    public class Game
    {
        [Required]
        public string Name { get; set; }

        public string Genre { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string ImgUrl { get; set; }

        public string Notes { get; set; }

        public int HoursPlayed { get; set; }

        public int UserRating { get; set; }

        public string UserId { get; set; }

        /* Stretch Goals */
        public bool IsAvailable { get; set; }
        public bool IsOwned { get; set; }
    }
}