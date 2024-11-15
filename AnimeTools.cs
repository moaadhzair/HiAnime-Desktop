
using HiAnime_Desktop;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace HiAnime_Desktop
{
    internal class AnimeTools
    {
        public AnimeTools()
        {
        }
        public static async Task<List<AnimeInfo>> Search(string Search)
        {
            string html = await new HttpClient().GetStringAsync("https://hianime.to/search?keyword=" + Search);
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            var animes = new List<AnimeInfo>();
            htmlDoc.LoadHtml(html);

            var filmListWrapNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id='main-content']/section/div[contains(@class, 'tab-content')]/div/div[contains(@class, 'film_list-wrap')]");

            if (filmListWrapNode.OuterLength > 200)
            {
                var filmItems = filmListWrapNode.SelectNodes(".//div[contains(@class, 'flw-item')]");
                //this for loop iterates over each anime in filemItems variable and add them to List<AnimeInfo> animes
                foreach (var item in filmItems)
                {
                    var nameNode = item.SelectSingleNode(".//h3[contains(@class, 'film-name')]/a");                //Name
                    var matchh = Regex.Match(item.InnerHtml, "data-src=\"(.*?)\"");                                //PoserLink
                    var subbedNode = item.SelectSingleNode(".//div[contains(@class, 'tick-item tick-sub')]");      //SubbedEpisodes
                    var dubbedNode = item.SelectSingleNode(".//div[contains(@class, 'tick-item tick-dub')]");      //DubbedEpisodes
                    var match = Regex.Match(item.InnerHtml, "<a href=\"(.*?)\"");                                  //AnimeLink

                    var AnimeLink = "https://hianime.to" + (match.Success ? match.Groups[1].Value : "Anime name not found");
                    var DubbedEpisodes = int.TryParse(dubbedNode?.InnerText.Trim(), out int dubbed) ? dubbed : 0;
                    var SubbedEpisodes = int.TryParse(subbedNode?.InnerText.Trim(), out int subbed) ? subbed : 0;
                    var PosterLink = (matchh.Success ? matchh.Groups[1].Value : "Image URL not found");
                    var Name = nameNode?.InnerText.Trim();

                    var EpisodeCount = Math.Max(DubbedEpisodes, SubbedEpisodes);

                    var anime = new AnimeInfo()
                    {
                        name = Name,
                        posterLink = PosterLink,
                        subbedEpisodes = SubbedEpisodes,
                        dubbedEpisodes = DubbedEpisodes,
                        animeLink = AnimeLink,
                        EpsiodeCount = EpisodeCount
                    };
                    animes.Add(anime);
                }

                return animes;

            }

            return null;

        }
        /*public static async Task<int> processEpsiodes(AnimeInfo anime)
        {
            //this function processes each episode and retrieves it subbed and dubbed servers
            var id = anime.animeLink.Substring(anime.animeLink.LastIndexOf("-") + 1);
            var Episodes = await new HttpClient().GetStringAsync("https://hianime.to/ajax/v2/episode/list/" + id);


            var jsonDoc = JsonDocument.Parse(Episodes);
            string htmlContent = jsonDoc.RootElement.GetProperty("html").GetString();

            // Load HTML for parsing
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            // Extract episode names and links
            var episodes = new List<Episode>();
            var list = htmlDoc.DocumentNode.FirstChild.ChildNodes[1].ChildNodes[1].ChildNodes[3];

            cleanList(list);

            //Console.WriteLine("wdw");

            foreach (var node in list.ChildNodes)
            {
                string linkk = "https://hianime.to" + node.Attributes["href"].Value;
                string name = node.Attributes["title"].Value;
                int ID = int.Parse(node.Attributes["data-id"].Value);


                jsonDoc = JsonDocument.Parse(await new HttpClient().GetStringAsync("https://hianime.to/ajax/v2/episode/servers?episodeId=" + ID));
                string html = jsonDoc.RootElement.GetProperty("html").ToString();

                var htmlDoc2 = new HtmlAgilityPack.HtmlDocument();

                htmlDoc2.LoadHtml(html);
                var SubbedList = htmlDoc2.DocumentNode.ChildNodes[2].ChildNodes[3];
                cleanList(SubbedList);
                var SubbedServers = new List<(String ServerName, String ServerEmbedLink)>();
                foreach (var item in SubbedList.ChildNodes)
                {
                    jsonDoc = JsonDocument.Parse(await new HttpClient().GetStringAsync("https://hianime.to/ajax/v2/episode/sources?id=" + item.Attributes["data-id"].Value));
                    SubbedServers.Add((item.ChildNodes[1].InnerHtml, jsonDoc.RootElement.GetProperty("link").ToString()));
                }


                var DubbedList = htmlDoc2.DocumentNode.ChildNodes[4].ChildNodes[3];
                cleanList(DubbedList);
                var DubbedServers = new List<(String ServerName, String ServerEmbedLink)>();
                foreach (var item in DubbedList.ChildNodes)
                {
                    jsonDoc = JsonDocument.Parse(await new HttpClient().GetStringAsync("https://hianime.to/ajax/v2/episode/sources?id=" + item.Attributes["data-id"].Value));
                    DubbedServers.Add((item.ChildNodes[1].InnerHtml, jsonDoc.RootElement.GetProperty("link").ToString()));
                }

                episodes.Add(new Episode() { isDubbed = DubbedServers.Count > 0, Name = name, SubbedServers = SubbedServers, DubbedServers = DubbedServers, Link = linkk, ID = ID });

            }
            anime.Episodes = episodes;
            return 0;
        }

        */
        public static async Task<int> processEpsiodeServers(/*AnimeInfo anime, int ep*/ Episode episode)
        {
            //this function processes a certain episode @{ep} and retrieves it subbed and dubbed servers
            /*var id = anime.animeLink.Substring(anime.animeLink.LastIndexOf("-") + 1);
            var Episodes = await new HttpClient().GetStringAsync("https://hianime.to/ajax/v2/episode/list/" + id);


            var jsonDoc = JsonDocument.Parse(Episodes);
            string htmlContent = jsonDoc.RootElement.GetProperty("html").GetString();

            // Load HTML for parsing
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            // Extract episode names and links
            var episodes = new List<Episode>();

            int EpisodeCount = jsonDoc.RootElement.GetProperty("totalItems").GetInt32();

            for (int i = 0; i < EpisodeCount; i++)
            {
                episodes.Add(null);
            }



            if (EpisodeCount < ep) throw new Exception($"{anime.name} doesn't have the {ep} Episode");
            int pages = (EpisodeCount + 100 - 1) / 100;
            HtmlNode list = null;
            HtmlNode Animenode = null;
            if (pages > 1)
            {
                var Pagelist = htmlDoc.DocumentNode.FirstChild.ChildNodes[1].ChildNodes[1];
                cleanList(Pagelist);
                Pagelist.RemoveChild(Pagelist.FirstChild);
                var dontknow = Pagelist.ChildNodes[-1 + (ep + 100 - 1) / 100];
                cleanList(dontknow);
                int currentEp = 10 + ep - (1000 + (-1 + (ep + 100 - 1) / 100));
                Animenode = dontknow.ChildNodes[currentEp];

            }
            else
            {
                Animenode = list.FirstChild.ChildNodes[ep];
            }




            string linkk = "https://hianime.to" + Animenode.Attributes["href"].Value;
            string name = Animenode.Attributes["title"].Value;
            int ID = int.Parse(Animenode.Attributes["data-id"].Value);
            */

            var episodes = new List<Episode>();


            var jsonDoc = JsonDocument.Parse(await new HttpClient().GetStringAsync("https://hianime.to/ajax/v2/episode/servers?episodeId=" +/*anime.Episodes[ep]*/episode.ID));
            string html = jsonDoc.RootElement.GetProperty("html").ToString();
            var htmlDoc2 = new HtmlAgilityPack.HtmlDocument();
            htmlDoc2.LoadHtml(html);

            HtmlNode SubbedList = null;
            HtmlNode DubbedList = null;
            var SubbedServers = new List<(String ServerName, String ServerEmbedLink)>();
            var DubbedServers = new List<(String ServerName, String ServerEmbedLink)>();



            try
            {
                SubbedList = htmlDoc2.DocumentNode.ChildNodes[2].ChildNodes[3];
                cleanList(SubbedList);

                foreach (var item in SubbedList.ChildNodes)
                {
                    jsonDoc = JsonDocument.Parse(await new HttpClient().GetStringAsync("https://hianime.to/ajax/v2/episode/sources?id=" + item.Attributes["data-id"].Value));
                    SubbedServers.Add((item.ChildNodes[1].InnerHtml, jsonDoc.RootElement.GetProperty("link").ToString()));
                }
            }
            catch (Exception)
            {

            }


            try
            {
                DubbedList = htmlDoc2.DocumentNode.ChildNodes[4].ChildNodes[3];
                cleanList(DubbedList);

                foreach (var item in DubbedList.ChildNodes)
                {
                    jsonDoc = JsonDocument.Parse(await new HttpClient().GetStringAsync("https://hianime.to/ajax/v2/episode/sources?id=" + item.Attributes["data-id"].Value));
                    DubbedServers.Add((item.ChildNodes[1].InnerHtml, jsonDoc.RootElement.GetProperty("link").ToString()));
                }
            }
            catch (Exception)
            {

            }





            //episodes.Insert(ep, new Episode() { Number = ep, isDubbed = DubbedList != null, Name = name, SubbedServers = SubbedServers, DubbedServers = DubbedServers, Link = linkk, ID = ID });

            episode.isDubbed = DubbedList != null;
            episode.SubbedServers = SubbedServers;
            episode.DubbedServers = DubbedServers;



            //anime.Episodes = episodes;
            return 0;
        }


        public static async Task<int> processEpsiodes(AnimeInfo anime)
        {
            //this function processes a certain episode @{ep} and retrieves it subbed and dubbed servers
            var id = anime.animeLink.Substring(anime.animeLink.LastIndexOf("-") + 1);
            var Episodes = await new HttpClient().GetStringAsync("https://hianime.to/ajax/v2/episode/list/" + id);


            var jsonDoc = JsonDocument.Parse(Episodes);
            string htmlContent = jsonDoc.RootElement.GetProperty("html").GetString();

            // Load HTML for parsing
            var htmlDoc = new HtmlAgilityPack.HtmlDocument();
            htmlDoc.LoadHtml(htmlContent);

            // Extract episode names and links
            var episodes = new List<Episode>();

            int EpisodeCount = jsonDoc.RootElement.GetProperty("totalItems").GetInt32();

            for (int j = 0; j < EpisodeCount; j++)
            {
                episodes.Add(null);
            }



            //if (EpisodeCount < ep) throw new Exception($"{anime.name} doesn't have the {ep} Episode");
            int pages = (EpisodeCount + 100 - 1) / 100;
            HtmlNode list = null;
            //HtmlNode Animenode = null;

            var Pagelist = htmlDoc.DocumentNode.FirstChild.ChildNodes[1].ChildNodes[1];
            cleanList(Pagelist);

            Pagelist.RemoveChild(Pagelist.FirstChild);
            int i = 0;
            foreach (var page in Pagelist.ChildNodes)
            {
                cleanList(page);
                foreach (var Animenode in page.ChildNodes)
                {
                    string linkk = "https://hianime.to" + Animenode.Attributes["href"].Value;
                    string name = Animenode.Attributes["title"].Value;
                    int ID = int.Parse(Animenode.Attributes["data-id"].Value);


                    episodes.Insert(i, new Episode() { Number = i, Name = name, Link = linkk, ID = ID });
                    i++;
                }
                
            }
            
                
                
                /*var currentPage = Pagelist.ChildNodes[-1 + (ep + 100 - 1) / 100];
                cleanList(currentPage);
                int currentEp = 10 + ep - (1000 + (-1 + (ep + 100 - 1) / 100));
                Animenode = currentPage.ChildNodes[currentEp];

            
            */



            
            anime.Episodes = episodes;
            return 0;
        }

        static void cleanList(HtmlNode list)
        {
            for (int i = list.ChildNodes.Count - 1; i >= 0; i--)
            {
                if (i % 2 == 0)
                {
                    list.RemoveChild(list.ChildNodes[i]);
                }
            }
        }
    }
}
