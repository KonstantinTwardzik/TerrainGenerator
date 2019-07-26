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

namespace Topographer3D.Views
{
    /// <summary>
    /// Interaktionslogik für ViewportDirectX11.xaml
    /// </summary>
    public partial class LayerDetails : UserControl
    {
        public LayerDetails()
        {
            InitializeComponent();
            this.DataContext = Application.Current.MainWindow.DataContext;
        }
    }
}

