/*
JAPAN GUESSR 2020

Main.cs

- Description: Main application window logic
- Author: David Molina Toro
- Date: 08 - 12 - 2020
- Version: 1.0

Property of Skeptic Productions
*/

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JapanGuessr
{
    public partial class MainWindow : Window
    {
        /*
        Public constructor
        */
        public MainWindow()
        {
            InitializeComponent();
        }

        /*
        Updated the picture shown
        */
        private void UpdatePicture()
        {
            //Get a new image file path
            string sFilePath = IPictureManager.Instance.UpdatePicture();

            //Get the GPS information and check if it exists
            if (IPictureManager.Instance.GetImageGPS(out double[] dCoordinates))
            {
                //Set the new image
                SetImage(sFilePath);
            }
            else if (checkSkipGPS.IsChecked.Value)
            {
                UpdatePicture();
            }
            else
            {
                //Set the new image
                SetImage(sFilePath);

                //Ask if the user want to add the information
                MessageBoxResult iResult = MessageBox.Show(Properties.Resources.Main_textSetInfoGPS, "JapanGuessr", MessageBoxButton.YesNo);
                switch (iResult)
                {
                    case MessageBoxResult.Yes:
                        //TODO: Set selected GPS information
                        break;
                    case MessageBoxResult.No:
                        //Set a new picture
                        UpdatePicture();
                        break;
                }
            }
        }

        /*
        Sets the picture to show from a file path
        */
        private void SetImage(string sFilePath)
        {
            //Initialize the new image as a bitmap
            BitmapImage bitmap = new BitmapImage();

            //Set the bitmap source
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(sFilePath, UriKind.Relative);

            //Release the bitmap for it to be accessed
            bitmap.EndInit();
            bitmap.Freeze();

            //Set the grid background color
            gridPicture.Background = new SolidColorBrush(Colors.Black);

            //Set the map visibility
            mapPicture.Visibility = Visibility.Visible;

            //Set the new image
            imgPicture.Source = bitmap;
        }

        /*
        Button update click event handler
        */
        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            UpdatePicture();
        }
    }
}
