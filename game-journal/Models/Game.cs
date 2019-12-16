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
        public ulong first_release_date { get; set; }

        public DateTime _releaseDate { get; set; }

        [Display(Name = "Release Date")]
        public DateTime ReleaseDate
        {
            get
            {
                System.DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                _releaseDate = dateTime.AddSeconds(first_release_date).ToLocalTime();
                return _releaseDate;
            }
            set 
            {
                System.DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
                _releaseDate = dateTime.AddSeconds(first_release_date).ToLocalTime();
            }
        }

        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        // References to other models
        [JsonPropertyName("genres")]
        [NotMapped]
        public List<Genre> Genres { get; set; }

        [JsonProperty("platforms")]
        [NotMapped]
        public List<Platform> Platforms { get; set; }

        [JsonPropertyName("cover")]
        [NotMapped]
        public int CoverId { get; set; }

        // User entered properties
        public string Notes { get; set; }

        public int HoursPlayed { get; set; }

        public int UserRating { get; set; }

        public string UserId { get; set; }
    }
}