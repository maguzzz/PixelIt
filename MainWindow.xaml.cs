using Haley.WPF.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.Win32;

namespace PixelIt
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public string Color;
        public string lastColor = "#ffffff";
        public bool CanChagneColor = false;
        int rows;


        private void ClearButton(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < PixelGrid.Children.Count; i++)
            {
                if (PixelGrid.Children[i] is Button)
                {
                    Button button = (Button)PixelGrid.Children[i];
                    button.Background = Brushes.White;
                }
            }
        }


        private void Stift_Checked(object sender, RoutedEventArgs e)
        {
            if (StiftRadio.IsChecked == true)
            {
                CanChagneColor = true;
                Color = lastColor;
            }
        }

        private void Radier_Checked(object sender, RoutedEventArgs e)
        {
            if (RadierRadio.IsChecked == true)
            {
                CanChagneColor = false;
                Color = "#ffffff";
            }
        }

        private void SinglePixel(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (sender is Button clickedButton)
                {
                    if (Pipette.IsChecked == true)
                    {
                        CanChagneColor = false;
                        Color = Convert.ToString(clickedButton.Background);
                        lastColor = Color;
                    }
                    else
                    {
                        if (CanChagneColor == false)
                        {
                            Color = "#ffffff";

                        }
                        var bc = new BrushConverter();
                        clickedButton.Background = (Brush)bc.ConvertFrom(Color);
                        e.Handled = true;
                    }
                }
            }
        }


        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            PixelGrid.Children.Clear();

            rows = Convert.ToInt32(ResolutionSlider.Value);

            int calc = rows * rows;

            for (int i = 1; i <= calc; i++)
            {
                Button button = new Button();
                button.BorderBrush = Brushes.Gray;
                button.Background = Brushes.White;
                button.BorderThickness = new Thickness(0.5);
                button.MouseEnter += SinglePixel;
                button.PreviewMouseDown += SinglePixel;

                PixelGrid.Children.Add(button);
            }
        }

        private void PixelGrid_Loaded(object sender, EventArgs e)
        {
            PixelGrid.Rows = rows;
            PixelGrid.Columns = rows;
        }

        private void ColorPkrChange(object sender, MouseEventArgs e)
        {
            Color = Convert.ToString(ColorPicker.SelectedBrush);
            lastColor = Color;
        }

        private void ExportImg(object sender, RoutedEventArgs e)
        {
            string[] hexCodes = new String[PixelGrid.Children.Count];

            for (int i = 0; i < PixelGrid.Children.Count; i++)
            {
                if (PixelGrid.Children[i] is Button)
                {
                    Button button = (Button)PixelGrid.Children[i];
                    hexCodes[i] = button.Background.ToString();
                }
            }

            int width = rows;
            int height = rows;

            using (var image = new Image<Rgba32>(width, height))
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int index = y * width + x;
                        string hexCode = hexCodes[index];

                        var color = HexToRgba32(hexCode);

                        image[x, y] = color;
                    } 
                }

                MessageBox.Show("Exporting IMG to Public Downloads ");
   
                image.Save(@"C:\Users\Public\Downloads\output.png");
            }
        }

        private Rgba32 HexToRgba32(string hexCode)
        {
            hexCode = hexCode.TrimStart('#');

            if (hexCode.Length == 6)
            {
                hexCode = "FF" + hexCode;
            }

            uint hexValue = uint.Parse(hexCode, System.Globalization.NumberStyles.HexNumber);

            return new Rgba32(
                (byte)((hexValue >> 16) & 255),
                (byte)((hexValue >> 8) & 255),
                (byte)(hexValue & 255),
                (byte)((hexValue >> 24) & 255)
            );
        }
    }
}