using System.Collections.Generic;

namespace HiAnime_Desktop
{
    public class AnimeInfo
    {
        public string name { get; set; }
        public string posterLink { get; set; }
        public int subbedEpisodes { get; set; }
        public int dubbedEpisodes { get; set; }
        public string animeLink { get; set; }
        public List<Episode> Episodes { get; set; }
        public int EpsiodeCount { get; set; }

        /*public AnimeInfo(string name, string posterLink, int subbedEpisodes, int dubbedEpisodes, string animeLink, int epsiodeCount)
        {
            this.name = name;
            this.posterLink = posterLink;
            this.subbedEpisodes = subbedEpisodes;
            this.dubbedEpisodes = dubbedEpisodes;
            this.animeLink = animeLink;
            EpsiodeCount = epsiodeCount;
        }*/
    }
}