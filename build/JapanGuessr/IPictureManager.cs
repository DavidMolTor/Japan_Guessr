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
using System.Windows;
using System.Device.Location;
using System.Collections.Generic;

//Exif libraries
using ExifLib;

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

        //Pictures found variable
        private string[] sPictures  = null;
        public bool bDirectorySet   = false;

        //Selected picture variable
        private string sCurrentFilePath = "";

        /*
        Sets the pictures found in the selected directory
        */
        public void FindPictures(string sSearchPath)
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
                sPictures = sFilesFound.ToArray();
                bDirectorySet = true;
            }
            else
            {
                //Show the directory not found dialog
                MessageBox.Show(Properties.Resources.Main_textDirectoryError, "JapanGuessr", MessageBoxButton.OK);
            }
        }

        /*
        Returns a random picture file path
        */
        public string GetRandomPicture()
        {
            //Generate a random number from the image count
            Random rand = new Random();
            int iRandom = rand.Next(sPictures.Length);

            //Set the picture file path
            sCurrentFilePath = sPictures[iRandom];

            //Return the selected picture
            return sCurrentFilePath;
        }

        /*
        Returns the location information for the selected picture
        */
        public bool GetImageGPS(out GeoCoordinate coords)
        {
            //Initialize the EXIF reader
            ExifReader exifReader = new ExifReader(sCurrentFilePath);

            //Try getting the GPS information
            bool bLatitudeOK        = exifReader.GetTagValue(ExifTags.GPSLatitude, out double[] dLatitudeRaw);
            bool bLatitudeRefOK     = exifReader.GetTagValue(ExifTags.GPSLatitudeRef, out string sLatitudeRef);
            bool bLongitudeOK       = exifReader.GetTagValue(ExifTags.GPSLongitude, out double[] dLongitudeRaw);
            bool bLongitudeRefOK    = exifReader.GetTagValue(ExifTags.GPSLongitudeRef, out string sLongitudeRef);

            //Check if there is any GPS information
            if (bLatitudeOK && bLatitudeRefOK && bLongitudeOK && bLongitudeRefOK)
            {

                //Set the coordinates as sinlge double format
                double dLatitude    = (dLatitudeRaw[0] + dLatitudeRaw[1] / 60 + dLatitudeRaw[2] / 3600) * (sLatitudeRef == "S" ? -1 : 1);
                double dLongitude   = (dLongitudeRaw[0] + dLongitudeRaw[1] / 60 + dLongitudeRaw[2] / 3600) * (sLongitudeRef == "W" ? -1 : 1);

                coords = new GeoCoordinate(dLatitude, dLongitude);
                return true;
            }
            else
            {
                coords = null;
                return false;
            }
        }
    }
}
