using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace game_journal.Models
{
    public class Genre
    {
        [JsonProperty("id")]
        public int GenreId { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
