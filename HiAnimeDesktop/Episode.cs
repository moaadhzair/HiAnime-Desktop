using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiAnime_Desktop
{
    public class Episode
    {
        public List<(string ServerName, string ServerEmbedLink)> SubbedServers;
        public List<(string ServerName, string ServerEmbedLink)> DubbedServers;
        public bool isDubbed = false;
        public string Name {  get; set; }
        public string Link;
        public int ID;
        public int Number;

        public Episode()
        {
        }
    }
}
