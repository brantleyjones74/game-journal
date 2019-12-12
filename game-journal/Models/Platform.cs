﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace game_journal.Models
{
    public class Platform
    {
        [JsonProperty("id")]
        public int PlatformId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
