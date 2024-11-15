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

namespace HiAnime_Desktop
{
    /// <summary>
    /// Interaction logic for AnimeTemplate.xaml
    /// </summary>  
    public partial class AnimeTemplate : UserControl
    {
        public AnimeTemplate(AnimeInfo animes)
        {
            InitializeComponent();

            DataContext = animes;
        }


        private void navigate_to_anime(object sender, RoutedEventArgs e)
        {
            
            new EpsiodeWindow((AnimeInfo)this.DataContext).Show();
        }
    }
}
