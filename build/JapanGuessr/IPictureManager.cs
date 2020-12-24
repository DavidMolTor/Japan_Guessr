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
            //Remove the previous picture from the list
            listPictures.Remove(sCurrentFilePath);
            sCurrentFilePath = "";

            //Check if there are any pictures left
            if (listPictures.Count > 0)
            {
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
            //Initialize an image object from the current file path
            using (Image image = new Bitmap(sCurrentFilePath))
            {
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
                if (GetInfoGPS(out double dLatitude, out double dLongitude))
                {
                    //Set the new coordinates
                    coords = new GeoCoordinate(dLatitude, dLongitude);

                    return true;
                }
                else
                {
                    //Set the coordinates to null
                    coords = null;

                    return false;
                }
            }
        }

        /*
        Retrieves the GPS information from the given image
        */
        public bool GetInfoGPS(out double dLatitude, out double dLongitude)
        {
            //Initialize an image object from the current file path
            using (Image image = new Bitmap(sCurrentFilePath))
            {
                try
                {
                    //Get the GPS latitude reference and value items
                    PropertyItem itemLatitude = image.GetPropertyItem(2);
                    PropertyItem itemLatitudeRef = image.GetPropertyItem(1);
                    dLatitude = CoordinateToValue(itemLatitude, itemLatitudeRef);

                    //Get the GPS longitude reference and value items
                    PropertyItem itemLongitude = image.GetPropertyItem(4);
                    PropertyItem itemLongitudeRef = image.GetPropertyItem(3);
                    dLongitude = CoordinateToValue(itemLongitude, itemLongitudeRef);

                    return true;
                }
                catch (ArgumentException)
                {
                    dLatitude = double.NaN;
                    dLongitude = double.NaN;
                    return false;
                }
            }
        }

        /*
        Sets the selected GPS information for the current image
        */
        public bool SetInfoGPS(double dLatitude, double dLongitude)
        {
            try
            {
                //Copy the original image
                string sTmpFilePath = sCurrentFilePath.Substring(0, sCurrentFilePath.LastIndexOf('.')) + "_tmp" + sCurrentFilePath.Substring(sCurrentFilePath.LastIndexOf('.'));
                File.Copy(sCurrentFilePath, sTmpFilePath);

                //Initialize an image object with the current selected path
                using (Image image = new Bitmap(sTmpFilePath))
                {
                    //Set the GPS version identifier field
                    SetProperty(image, 0, 1, new byte[] { 2, 3, 0, 0 });

                    //Set the latitude property value
                    SetProperty(image, 2, 5, CoordinateToRational(Math.Abs(dLatitude)));

                    //Set the latitude reference property value
                    char cLatitudeRef = dLatitude < 0 ? 'S' : 'N';
                    SetProperty(image, 1, 2, new byte[] { (byte)cLatitudeRef, 0 });

                    //Set the longitude property value
                    SetProperty(image, 4, 5, CoordinateToRational(Math.Abs(dLongitude)));

                    //Set the longitude reference property value
                    char cLongitudeRef = dLongitude < 0 ? 'W' : 'E';
                    SetProperty(image, 3, 2, new byte[] { (byte)cLongitudeRef, 0 });

                    //Delete the local copy of the image
                    File.Delete(sCurrentFilePath);

                    //Save the image with the new information
                    image.Save(sCurrentFilePath);
                }

                //Delete the temporary image
                File.Delete(sTmpFilePath);

                return true;
            }
            catch (ArgumentException)
            {
                return false;
            }
        }

        /*
        Sets a property item from the given parameters
        */
        private void SetProperty(Image image, int iID, short shortType, byte[] byteValue)
        {
            //Initialize the property item with the given values
            PropertyItem item   = image.PropertyItems[0];
            item.Id             = iID;
            item.Type           = shortType;
            item.Len            = byteValue.Length;
            item.Value          = byteValue;

            //Set the image property
            image.SetPropertyItem(item);
        }

        /*
        Converts the given GPS coordinate to single values with sign
        */
        private double CoordinateToValue(PropertyItem itemValue, PropertyItem itemRef)
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

        /*
        Converts the given GPS coordinate to rational value without sign
        */
        private byte[] CoordinateToRational(double dCoordinate)
        {
            //Get the degrees value
            int iDegrees = (int)Math.Floor(dCoordinate);
            dCoordinate = (dCoordinate - iDegrees) * 60;

            //Get the minutes value
            int iMinutes = (int)Math.Floor(dCoordinate);
            dCoordinate = (dCoordinate - iMinutes) * 60 * 100;

            //Get the seconds value
            int iSeconds = (int)Math.Round(dCoordinate);

            //Initialize the rational array
            byte[] bytes = new byte[3 * 2 * 4];

            //Set the degrees array value
            Array.Copy(BitConverter.GetBytes(iDegrees), 0, bytes, 0, 4);
            Array.Copy(BitConverter.GetBytes(1), 0, bytes, 4, 4);

            //Set the minutes array value
            Array.Copy(BitConverter.GetBytes(iMinutes), 0, bytes, 8, 4);
            Array.Copy(BitConverter.GetBytes(1), 0, bytes, 12, 4);

            //Set the seconds array value
            Array.Copy(BitConverter.GetBytes(iSeconds), 0, bytes, 16, 4);
            Array.Copy(BitConverter.GetBytes(100), 0, bytes, 20, 4);

            return bytes;
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
