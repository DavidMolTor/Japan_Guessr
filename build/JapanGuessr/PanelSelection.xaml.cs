/*
JAPAN GUESSR 2020

Main.cs

- Description: Selection game panel
- Author: David Molina Toro
- Date: 18 - 12 - 2020
- Version: 1.0

Property of Skeptic Productions
*/

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Device.Location;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

//Map libraries
using Microsoft.Maps.MapControl.WPF;

namespace JapanGuessr
{
    public partial class PanelSelection : UserControl
    {
        /*
        Public constructor
        */
        public PanelSelection()
        {
            InitializeComponent();
        }

        //Currently selected game mode
        private GameMode iMode;

        /*
        Game mode selection setter
        */
        public void SetGameMode (GameMode iSelectedMode)
        {
            //Set the game mode
            iMode = iSelectedMode;

            //Update the first picture
            UpdatePicture();
        }

        //Coordinate selection objects
        private GeoCoordinate coordsPicture;
        private GeoCoordinate coordsSelected;
        private bool bSaveCoords = false;

        /*
        Updated the picture shown
        */
        private void UpdatePicture()
        {
            //Get a new image file path
            string sFilePath = IPictureManager.Instance.UpdatePicture();

            //Get the GPS information and check if it exists
            bool bInfo = IPictureManager.Instance.GetImageGPS(out coordsPicture);
            switch (iMode)
            {
                case GameMode.Normal:
                    //Set the new image
                    SetImage(sFilePath);

                    //Check if the GPS information has been found
                    if (!bInfo)
                    {
                        //Ask if the user want to add the information
                        MessageBoxResult iResult = MessageBox.Show(Properties.Resources.Main_textSetInfoGPS, "JapanGuessr", MessageBoxButton.YesNo);
                        switch (iResult)
                        {
                            case MessageBoxResult.Yes:
                                //Set the coordinates selection flag
                                bSaveCoords = true;
                                break;
                            case MessageBoxResult.No:
                                //Set a new picture
                                UpdatePicture();
                                break;
                        }
                    }
                    break;
                case GameMode.OnlyGPS:
                    //Check if the GPS information has been found
                    if (bInfo)
                    {
                        //Set the new image
                        SetImage(sFilePath);
                    }
                    else
                    {
                        //Try with a new picture
                        UpdatePicture();
                    }
                    break;
                case GameMode.AddGPS:
                    //Check if the GPS information has been found
                    if (bInfo)
                    {
                        //Try with a new picture
                        UpdatePicture();
                    }
                    else
                    {
                        //Set the new image
                        SetImage(sFilePath);
                    }
                    break;
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
        private void ButtonSelect_Click(object sender, RoutedEventArgs e)
        {
            //Check the selected game mode
            double dDistance;
            switch (iMode)
            {
                case GameMode.Normal:
                    //Check if the coordinates must be saved
                    if (bSaveCoords)
                    {
                        //TODO: Save the coordinates

                        //Reset the save coordinates flag
                        bSaveCoords = false;
                    }
                    else
                    {
                        //Get the distance between the picture location and the selected point
                        dDistance = coordsPicture.GetDistanceTo(coordsSelected);

                        //Show the distance to the target
                        MessageBox.Show(Properties.Resources.Main_textDistanceToTarget.Replace("[DIST]", dDistance.ToString("0")), "JapanGuessr", MessageBoxButton.OK);
                    }
                    break;
                case GameMode.OnlyGPS:
                    //Get the distance between the picture location and the selected point
                    dDistance = coordsPicture.GetDistanceTo(coordsSelected);

                    //Show the distance to the target
                    MessageBox.Show(Properties.Resources.Main_textDistanceToTarget.Replace("[DIST]", dDistance.ToString("0")), "JapanGuessr", MessageBoxButton.OK);
                    break;
                case GameMode.AddGPS:
                    //TODO: Save the coordinates
                    break;
            }

            //Clear all map childrens
            mapPicture.Children.Clear();

            //Update the picture
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
                    //Check if the mouse has not moved too much
                    if (iMoveCount < 3)
                    {
                        //Get the current location
                        Location location = mapPicture.ViewportPointToLocation(e.GetPosition(mapPicture));

                        //Add a push pin to the selected location
                        Pushpin pushPin = new Pushpin()
                        {
                            Location = location
                        };
                        mapPicture.Children.Add(pushPin);

                        //Save the selected coordinates
                        coordsSelected = new GeoCoordinate(location.Latitude, location.Longitude);
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

    /*
    Game mode enumeration
    */
    public enum GameMode
    {
        Normal  = 0,
        OnlyGPS = 1,
        AddGPS  = 2
    }
}
