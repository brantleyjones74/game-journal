using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace game_journal.Models
{
    public class MyGamesViewModel
    {
        public string Title { get; set; }
        public IEnumerable<Game> Games { get; set; }
    }
}
