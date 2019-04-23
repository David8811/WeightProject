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
using Microsoft.Win32;

using BodyScanMulticamera;

namespace WeightTest
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

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "STL Mesh Files|*.stl";

            if (dialog.ShowDialog() == true)
            {
                try
                {

                    BodyScanMulticamera.ModelVisualizer mv = new ModelVisualizer(dialog.FileName, (int)ageSlider.Value);
                    if (MyViewer.Children.Count == 0)
                        MyViewer.Children.Add(mv.Model);
                    else
                        MyViewer.Children[0] = mv.Model;
                
                    MyViewer.Visibility = System.Windows.Visibility.Visible;

                    lWeight.Content = "Peso estimado: " + mv.Weight.ToString("00") + " Kg" + " ± " + mv.DeltaWeight.ToString("00");
                }
                catch (Exception ex)
                {
                    
                }
            }
            
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(ageLabel != null && e != null)
                ageLabel.Content = e.NewValue.ToString("0");
        }
    }
}
