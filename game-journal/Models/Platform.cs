using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace game_journal.Models
{
    public class Platform
    {
        [Key]
        public int PlatformId { get; set; }

        [JsonProperty("id")]
        public int ApiPlatformId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
