using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace game_journal.Models
{
    public class Genre
    {
        [Key]
        public int GenreId { get; set; }

        [JsonProperty("id")]
        public int ApiGenreId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
