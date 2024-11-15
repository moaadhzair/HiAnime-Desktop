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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace HiAnime_Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }



        private async void Button_Click(object sender, RoutedEventArgs e)
        {

            List<AnimeInfo> animes = await AnimeTools.Search(searchbox.Text);
            FilmWrapPanel.Children.Clear();

            if (animes != null)
            {
                foreach (var anime in animes)
                   {
                        var itemTemplate = new AnimeTemplate(anime);
                        FilmWrapPanel.Children.Add(itemTemplate);
                    }
            }
            

            

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void searchbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click(null, null);
            }
        }

        private void searchbox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}
