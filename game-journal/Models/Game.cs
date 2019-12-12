using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace game_journal.Models
{
    public class Rootobject
    {
        public Game[] id { get; set; }
    }
    public class Game
    {
        [Required]
        [Key]
        [JsonPropertyName("id")]
        public int GameId { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("genres")]
        public int Genre { get; set; }

        //public string Genre { get; set; }

        //public DateTime ReleaseDate { get; set; }

        //public string ImgUrl { get; set; }

        //public string Notes { get; set; }

        //public int HoursPlayed { get; set; }

        //public int UserRating { get; set; }

        //public string UserId { get; set; }

        /* Stretch Goals
        public bool IsAvailable { get; set; }
        public bool IsOwned { get; set; } */
    }
}