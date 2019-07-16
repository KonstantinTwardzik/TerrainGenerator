using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Topographer3D.ViewModels;

namespace Topographer3D.Views
{
    /// <summary>
    /// Interaktionslogik für Viewport.xaml
    /// </summary>
    public partial class Viewport : UserControl
    {
        public Viewport()
        {
            InitializeComponent();
            this.DataContext = Application.Current.MainWindow.DataContext;

        }
    }
}
