using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace game_journal.Models
{
    public class testclass
    {

        public class Rootobject
        {
            public int id { get; set; }
            public int cover { get; set; }
            public int first_release_date { get; set; }
            public List<int> genres { get; set; }
            public string name { get; set; }
            public List<int> platforms { get; set; }
            public string summary { get; set; }
        }

    }
}
