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
using TerrainGenerator.ViewModels;

namespace TerrainGenerator.Views
{
    /// <summary>
    /// Interaktionslogik für NoiseController.xaml
    /// </summary>
    public partial class NoiseController : UserControl
    {
        public NoiseController()
        {
            InitializeComponent();
            this.DataContext = Application.Current.MainWindow.DataContext;

        }
    }
}
