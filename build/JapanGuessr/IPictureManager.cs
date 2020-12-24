/*
JAPAN GUESSR 2020

Main.cs

- Description: Contains the application entry point
- Author: David Molina Toro
- Date: 08 - 12 - 2020
- Version: 1.0

Property of Skeptic Productions
*/

using System;
using System.IO;
using System.Text;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.Device.Location;
using System.Collections.Generic;

namespace JapanGuessr
{
    public sealed class IPictureManager
    {
        private static volatile IPictureManager instance = null;
        private static readonly object padlock = new object();

        /*
        Public static constructor
        */
        static IPictureManager()
        {

        }

        /*
        Instance object for the singleton instantiation
        */
        public static IPictureManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new IPictureManager();
                        }
                    }
                }
                return instance;
            }
        }

        //Pictures list object
        private List<string> listPictures  = null;

        //Selected picture variable
        private string sCurrentFilePath = "";

        /*
        Sets the pictures found in the selected directory
        */
        public bool FindPictures(string sSearchPath)
        {
            //Check if the selected folder exists
            if (Directory.Exists(sSearchPath))
            {
                //Set a temporary files vector
                List<string> sFilesFound = new List<string>();

                //Set the image filters to look for
                string[] sFilters = new string[] { "jpg", "jpeg", "png", "gif", "tiff", "bmp", "svg" };

                //Search all image files
                for (int i = 0; i < sFilters.Length; i++)
                {
                    sFilesFound.AddRange(Directory.GetFiles(sSearchPath, string.Format("*.{0}", sFilters[i]), SearchOption.AllDirectories));
                }

                //Set the pictures array
                listPictures    = sFilesFound;
                return true;
            }
            else
            {
                //Show the directory not found dialog
                MessageBox.Show(Properties.Resources.Main_textDirectoryError, "JapanGuessr", MessageBoxButton.OK);
                return false;
            }
        }

        /*
        Returns a random picture file path
        */
        public string GetRandomPicture()
        {
            //Check if there are any pictures left
            if (listPictures.Count > 0)
            {
                //Remove the previous picture from the list
                listPictures.Remove(sCurrentFilePath);

                //Generate a random number from the image count
                Random rand = new Random();
                int iRandom = rand.Next(listPictures.Count);

                //Set the picture file path
                sCurrentFilePath = listPictures[iRandom];

                //Return the selected picture
                return sCurrentFilePath;
            }
            else
            {
                return "";
            }
        }

        /*
        Returns the location and orientation information for the selected picture
        */
        public bool GetImageInfo(out GeoCoordinate coords, out Rotation iRotation)
        {
            //Initialize a image object from the current file path
            Image image = new Bitmap(sCurrentFilePath);

            //Try getting the orientation information
            try
            {
                //Get the orientation information
                PropertyItem itemOrientation = image.GetPropertyItem(274);
                uint iOrientation = BitConverter.ToUInt16(itemOrientation.Value, 0);
                switch (iOrientation)
                {
                    case 5:
                    case 6:
                        iRotation = Rotation.Right;
                        break;
                    case 3:
                    case 4:
                        iRotation = Rotation.Full;
                        break;
                    case 7:
                    case 8:
                        iRotation = Rotation.Left;
                        break;
                    default:
                        iRotation = Rotation.None;
                        break;
                }
            }
            catch (ArgumentException)
            {
                iRotation = Rotation.None;
            }

            //Check if there is any GPS information
            if (GetInfoGPS(image, out double dLatitude, out double dLongitude))
            {
                coords = new GeoCoordinate(dLatitude, dLongitude);
                return true;
            }
            else
            {
                coords = null;
                return false;
            }
        }

        /*
        Retrieves the GPS information from the given image
        */
        public bool GetInfoGPS(Image image, out double dLatitude, out double dLongitude)
        {
            try
            {
                //Get the GPS latitude reference and value
                PropertyItem itemLatitude       = image.GetPropertyItem(2);
                PropertyItem itemLatitudeRef    = image.GetPropertyItem(1);
                dLatitude = ConvertCoordinate(itemLatitude, itemLatitudeRef);

                //Get the GPS longitude reference and value
                PropertyItem itemLongitude      = image.GetPropertyItem(4);
                PropertyItem itemLongitudeRef   = image.GetPropertyItem(3);
                dLongitude = ConvertCoordinate(itemLongitude, itemLongitudeRef);

                return true;
            }
            catch (ArgumentException)
            {
                dLatitude   = double.NaN;
                dLongitude  = double.NaN;
                return false;
            }
        }

        /*
        Converts the GPS information to single values with sign
        */
        private double ConvertCoordinate(PropertyItem itemValue, PropertyItem itemRef)
        {
            //Initialize the numerator and denominator variables
            uint iNumerator;
            uint iDenominator;

            //Get the degrees value from the numerator and denominator
            iNumerator      = BitConverter.ToUInt32(itemValue.Value, 0);
            iDenominator    = BitConverter.ToUInt32(itemValue.Value, 4);
            double dDegrees = iNumerator / (double)iDenominator;

            //Get the minutes value from the numerator and denominator
            iNumerator      = BitConverter.ToUInt32(itemValue.Value, 8);
            iDenominator    = BitConverter.ToUInt32(itemValue.Value, 12);
            double dMinutes = iNumerator / (double)iDenominator;

            //Get the seconds value from the numerator and denominator
            iNumerator      = BitConverter.ToUInt32(itemValue.Value, 16);
            iDenominator    = BitConverter.ToUInt32(itemValue.Value, 20);
            double dSeconds = iNumerator / (double)iDenominator;

            //Get the cardinal reference value
            string sReference = Encoding.ASCII.GetString(new byte[1] { itemRef.Value[0] });

            //Returnt he coordinate value
            return (dDegrees + (dMinutes / 60) + (dSeconds / 3600)) * ((sReference == "W" || sReference == "S") ? -1 : 1);
        }
    }

    /*
    Rotation enumeration
    */
    public enum Rotation
    {
        None    = 0,
        Left    = 1,
        Right   = 2,
        Full    = 3
    }
}
