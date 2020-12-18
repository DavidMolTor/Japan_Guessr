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
using System.Windows.Input;
using System.Windows.Media;
using System.Device.Location;
using System.Windows.Media.Imaging;

//Map libraries
using Microsoft.Maps.MapControl.WPF;

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

        //Currently selected coordinates
        private GeoCoordinate currentCoords;

        //Current selection mode
        private bool bSetCoords = false;

        /*
        Updated the picture shown
        */
        private void UpdatePicture()
        {
            //Get a new image file path
            string sFilePath = IPictureManager.Instance.UpdatePicture();

            //Get the GPS information and check if it exists
            if (IPictureManager.Instance.GetImageGPS(out currentCoords))
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
                        //Set the coordinates selection flag
                        bSetCoords = true;
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

        //Mouse event objects
        private int iMoveCount = 0;

        /*
        Left mouse button event handler
        */
        private void MapPicture_MouseLeftEvent(object sender, MouseButtonEventArgs e)
        {
            switch (e.ButtonState)
            {
                case MouseButtonState.Pressed:
                    iMoveCount = 0;
                    break;
                case MouseButtonState.Released:
                    if (iMoveCount < 3)
                    {
                        //Check the coordinates setting flag
                        if (bSetCoords)
                        {
                            //TODO: Save new coordinates information for the current picture
                        }
                        else
                        {
                            //Get the current location
                            Location location = mapPicture.ViewportPointToLocation(e.GetPosition(mapPicture));

                            //Clear all map childrens
                            mapPicture.Children.Clear();

                            //Add a push pin to the selected location
                            Pushpin pushPin = new Pushpin()
                            {
                                Location = location
                            };
                            mapPicture.Children.Add(pushPin);

                            //Get the distance between the picture location and the selected point
                            double dDistance = currentCoords.GetDistanceTo(new GeoCoordinate(location.Latitude, location.Longitude));

                            //Show the distance to the target
                            MessageBox.Show(Properties.Resources.Main_textDistanceToTarget.Replace("[DIST]", dDistance.ToString("0")), "JapanGuessr", MessageBoxButton.OK);
                        }
                    }
                    break;
            }
        }

        /*
        Mouse moved event handler
        */
        private void MapPicture_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                iMoveCount++;
            }
        }
    }
}
