using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace game_journal.Models
{
    public class Cover
    {
        [JsonProperty("id")]
        public int CoverId { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("height")]
        public int PxlHeight { get; set; }

        [JsonProperty("width")]
        public int PxlWidth { get; set; }

        [JsonProperty("image_id")]
        public string ImageId { get; set; }

        // Reference ID to Game Model
        [JsonProperty("game")]
        public int GameId { get; set; }
    }
}
