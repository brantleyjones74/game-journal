using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace game_journal.Models
{
    public class Game
    {
        // API Properties
        [Key]
        [JsonProperty("id")]
        public int GameId { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("first_release_date")]
        [Display(Name = "Release Date")]
        public DateTime ReleaseDate { get; set; }

        // References to other models
        [JsonPropertyName("genres")]
        [NotMapped]
        public List<int> GenreIds { get; set; }

        [JsonPropertyName("cover")]
        [NotMapped]
        public int CoverId { get; set; }

        [JsonProperty("platforms")]
        [NotMapped]
        public List<int> PlatformIds { get; set; }

        // User entered properties
        public string Notes { get; set; }

        public int HoursPlayed { get; set; }

        public int UserRating { get; set; }

        public string UserId { get; set; }
    }
}