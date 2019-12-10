using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace game_journal.Models
{
    public class Platform
    {
        [Key]
        public int PlatformId { get; set; }

        // Example: Sony
        [Required]
        public string Manufacturer { get; set; }
        
        // Example: PlayStation 4
        [Required]
        public string Model { get; set;  }
        
        public string Notes { get; set; }
        
        public string UserId { get; set; }
    }
}
