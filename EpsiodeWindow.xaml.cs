using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace HiAnime_Desktop
{
    /// <summary>
    /// Interaction logic for EpsiodeWindow.xaml
    /// </summary>
    public partial class EpsiodeWindow : Window
    {
        AnimeInfo Anime;
        public EpsiodeWindow(AnimeInfo Anime)
        {
            this.Anime = Anime;
            InitializeComponent();
            initAnimes(); 
        }



        private async void initAnimes()
        {
            animeName.Text = Anime.name;
            AnimePoster.Source = new BitmapImage(new Uri(Anime.posterLink));
            await AnimeTools.processEpsiodes(Anime);
            Anime.Episodes.RemoveAll(e => e == null);
            EpisodeListView.ItemsSource = Anime.Episodes;
        }

        private async void EpisodeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            // Get the selected item
            Episode episode = EpisodeListView.SelectedItem as Episode;

            if (episode != null)
            {
                new PlayerView(episode).Show();
            }
            EpisodeListView.UnselectAll();

        }
    }
}
