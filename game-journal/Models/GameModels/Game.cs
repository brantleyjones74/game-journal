using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        [JsonPropertyName("url")]
        public string GameUrl { get; set; }

        // References to other models
        [JsonPropertyName("genres")]
        public List<Genre> Genres { get; set; }

        [JsonPropertyName("cover")]
        public string ImgUrl { get; set; }

        [JsonProperty("platforms")]
        public List<Platform> Platforms { get; set; }

        // User entered properties
        public string Notes { get; set; }

        public int HoursPlayed { get; set; }

        public int UserRating { get; set; }

        public string UserId { get; set; }
    }
}