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
using System.Windows.Media.Animation;

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

            //Initialize all animation objects
            InitializeAnimations();
        }

        /*
        Initializes all object animations
        */
        private void InitializeAnimations()
        {
            //Initialize the width upscaling animation
            animationWidthUp = new DoubleAnimation()
            {
                From        = 180,
                To          = 450,
                Duration    = new Duration(TimeSpan.FromMilliseconds(200))
            };
            Storyboard.SetTargetName(animationWidthUp, mapPicture.Name);
            Storyboard.SetTargetProperty(animationWidthUp, new PropertyPath(WidthProperty));

            //Initialize the width upscaling animation
            animationHeightUp  = new DoubleAnimation()
            {
                From        = 120,
                To          = 300,
                Duration    = new Duration(TimeSpan.FromMilliseconds(200))
            };
            Storyboard.SetTargetName(animationHeightUp, mapPicture.Name);
            Storyboard.SetTargetProperty(animationHeightUp, new PropertyPath(HeightProperty));

            //Initialize the downscaling storyboard
            storyboardMapUp = new Storyboard();
            storyboardMapUp.Children.Add(animationWidthUp);
            storyboardMapUp.Children.Add(animationHeightUp);

            //Initialize the width downscaling animation
            animationWidthDown = new DoubleAnimation()
            {
                From        = 450,
                To          = 180,
                Duration    = new Duration(TimeSpan.FromMilliseconds(200))
            };
            Storyboard.SetTargetName(animationWidthDown, mapPicture.Name);
            Storyboard.SetTargetProperty(animationWidthDown, new PropertyPath(WidthProperty));

            //Initialize the width downscaling animation
            animationHeightDown = new DoubleAnimation()
            {
                From        = 300,
                To          = 120,
                Duration    = new Duration(TimeSpan.FromMilliseconds(200))
            };
            Storyboard.SetTargetName(animationHeightDown, mapPicture.Name);
            Storyboard.SetTargetProperty(animationHeightDown, new PropertyPath(HeightProperty));

            //Initialize the downscaling storyboard
            storyboardMapDown = new Storyboard();
            storyboardMapDown.Children.Add(animationWidthDown);
            storyboardMapDown.Children.Add(animationHeightDown);
        }

        //Panel back event for main window
        public delegate void ReturnToStart();
        public event ReturnToStart ReturnToStartEvent;

        //Currently selected game mode
        private GameMode iMode;

        /*
        Game mode selection setter
        */
        public void SetGameMode(GameMode iSelectedMode)
        {
            //Set the game mode
            iMode = iSelectedMode;

            //Set the default picture
            imgPicture.Source = new BitmapImage(new Uri("Resources/default.png", UriKind.Relative));

            //Set the first picture
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
            //Disable the selection button
            buttonSelect.IsEnabled = false;

            //Get a new image file path
            string sFilePath = IPictureManager.Instance.GetRandomPicture();

            //Check if there are any pictures left
            if (!string.IsNullOrEmpty(sFilePath))
            {
                //Get the GPS information and check if it exists
                bool bInfo = IPictureManager.Instance.GetImageInfo(out coordsPicture, out Rotation iRotation);
                switch (iMode)
                {
                    case GameMode.Normal:
                        //Set the new image
                        SetImage(sFilePath, iRotation);

                        //Check if the GPS information has been found
                        if (!bInfo)
                        {
                            //Ask if the user wants to add the information
                            MessageBoxResult iResult = MessageBox.Show(Properties.Resources.Main_textSetInfoGPS, "JapanGuessr", MessageBoxButton.YesNo);
                            switch (iResult)
                            {
                                case MessageBoxResult.Yes:
                                    //Set the coordinates selection flag
                                    bSaveCoords = true;

                                    //Enable the selection button
                                    buttonSelect.IsEnabled = true;
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
                            SetImage(sFilePath, iRotation);
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
                            SetImage(sFilePath, iRotation);

                            //Enable the selection button
                            buttonSelect.IsEnabled = true;
                        }
                        break;
                }
            }
            else
            {
                //Show the no pictures left dialog
                MessageBox.Show(Properties.Resources.Main_textNoPicturesLeft, "JapanGuessr", MessageBoxButton.OK);

                //Return to the start panel
                ReturnToStartEvent?.Invoke();
            }
        }

        /*
        Sets the picture to show from a file path
        */
        private void SetImage(string sFilePath, Rotation iRotation)
        {
            //Initialize the new image as a bitmap
            BitmapImage bitmap = new BitmapImage();

            //Set the bitmap source
            bitmap.BeginInit();
            bitmap.UriSource    = new Uri(sFilePath, UriKind.Relative);
            bitmap.CacheOption  = BitmapCacheOption.OnLoad;

            //Release the bitmap for it to be accessed
            bitmap.EndInit();
            bitmap.Freeze();

            //Initialize a transformed bitmap for rotation
            TransformedBitmap transBitmap = new TransformedBitmap();
            transBitmap.BeginInit();
            transBitmap.Source = bitmap.Clone();

            //Set the orientation of the transformed bitmap
            switch (iRotation)
            {
                case Rotation.Left:
                    transBitmap.Transform = new RotateTransform(270);
                    break;
                case Rotation.Right:
                    transBitmap.Transform = new RotateTransform(90);
                    break;
                case Rotation.Full:
                    transBitmap.Transform = new RotateTransform(180);
                    break;
                case Rotation.None:
                    transBitmap.Transform = new RotateTransform(0);
                    break;
            }

            //Release the transformed bitmap for it to be accessed
            transBitmap.EndInit();
            transBitmap.Freeze();

            //Set the grid background color
            gridPicture.Background = new SolidColorBrush(Colors.Black);

            //Set the map visibility
            mapPicture.Visibility = Visibility.Visible;

            //Set the new image
            imgPicture.Source = transBitmap.Clone();
        }

        /*
        Button select click event handler
        */
        private void ButtonSelect_Click(object sender, RoutedEventArgs e)
        {
            //Check if a location has been selected
            if (mapPicture.Children.Count == 1)
            {
                //Check the selected game mode
                double dDistance;
                switch (iMode)
                {
                    case GameMode.Normal:
                        //Check if the coordinates must be saved
                        if (bSaveCoords)
                        {
                            //Save the coordinates for the current image
                            IPictureManager.Instance.SetInfoGPS(coordsSelected.Latitude, coordsSelected.Longitude);

                            //Reset the save coordinates flag
                            bSaveCoords = false;
                        }
                        else
                        {
                            //Get the distance between the picture location and the selected point
                            dDistance = coordsPicture.GetDistanceTo(coordsSelected);

                            //Show the distance to the target
                            ShowDistance(dDistance);
                        }
                        break;
                    case GameMode.OnlyGPS:
                        //Get the distance between the picture location and the selected point
                        dDistance = coordsPicture.GetDistanceTo(coordsSelected);

                        //Show the distance to the target
                        ShowDistance(dDistance);
                        break;
                    case GameMode.AddGPS:
                        //Save the coordinates for the current image
                        IPictureManager.Instance.SetInfoGPS(coordsSelected.Latitude, coordsSelected.Longitude);
                        break;
                }

                //Clear all map childrens
                mapPicture.Children.Clear();

                //Update the picture
                UpdatePicture();
            }
            else
            {
                //Check the current game mode
                if (iMode == GameMode.AddGPS || (iMode == GameMode.Normal && bSaveCoords))
                {
                    //Show the next picture dialog
                    if (MessageBox.Show(Properties.Resources.Main_textNextPicture, "JapanGuessr", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        //Show the next picture
                        UpdatePicture();
                    }
                }
                else
                {
                    //Show the select location first dialog
                    MessageBox.Show(Properties.Resources.Main_textSelectLocation, "JapanGuessr", MessageBoxButton.OK);
                }
            }
        }


        /*
        Button start click event handler
        */
        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            //Return to the start panel
            ReturnToStartEvent?.Invoke();
        }

        /*
        Shows the distance to target dialog
        */
        private void ShowDistance(double dDistance)
        {
            //Check if the distance format to be shown
            string sDistance;
            if (dDistance > 1000)
            {
                sDistance = (dDistance / 1000).ToString("0.0") + " kilómetros";
            }
            else
            {
                sDistance = dDistance.ToString("0") + " metros";
            }

            //Show the distance to the target dialog
            MessageBox.Show(Properties.Resources.Main_textDistanceToTarget.Replace("[DIST]", sDistance), "JapanGuessr", MessageBoxButton.OK);
        }

        //Mouse event objects
        private int iMoveCount = 0;

        /*
        Left mouse button event handler for map object
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
                        //Clear the map children
                        mapPicture.Children.Clear();

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

                        //Enable the selection button
                        buttonSelect.IsEnabled = true;
                    }
                    break;
            }
        }

        /*
        Mouse moved event handler for map object
        */
        private void MapPicture_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                iMoveCount++;
            }
        }

        //Map animation objects for upscaling
        DoubleAnimation animationWidthUp;
        DoubleAnimation animationHeightUp;
        Storyboard storyboardMapUp;

        //Map animation objects for downscaling
        DoubleAnimation animationWidthDown;
        DoubleAnimation animationHeightDown;
        Storyboard storyboardMapDown;

        /*
        Mouse enter event handler for map object
        */
        private void MapPicture_MouseEnter(object sender, MouseEventArgs e)
        {
            //Start the upscaling animation
            storyboardMapUp.Begin(mapPicture);
        }

        /*
        Mouse leave event handler for map object
        */
        private void MapPicture_MouseLeave(object sender, MouseEventArgs e)
        {
            //Start the downscaling animation
            storyboardMapDown.Begin(mapPicture);
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
