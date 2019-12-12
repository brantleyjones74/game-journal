using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace game_journal.Models
{
    public class Game
    {
        [Key]
        [JsonProperty("id")]
        public int GameId { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("genres")]
        public string Genre { get; set; }

        [JsonPropertyName("first_release_date")]
        [Display(Name = "Original Release Date")]
        public DateTime ReleaseDate { get; set; }

        [JsonPropertyName("cover")]
        public string ImgUrl { get; set; }

        public string Notes { get; set; }

        public int HoursPlayed { get; set; }

        public int UserRating { get; set; }

        public string UserId { get; set; }

        /* Stretch Goals
        public bool IsAvailable { get; set; }
        public bool IsOwned { get; set; } */
    }
}