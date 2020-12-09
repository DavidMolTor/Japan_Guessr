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

        //Selected picture variable
        private string sFilePath = "";

        /*
        Returns a random picture file path
        */
        public string UpdatePicture()
        {
            //Get all the pictures from the images folder
            string[] sPictures = Directory.GetFiles("./images", "*.jpg");

            //Generate a random number from the image count
            Random rand = new Random();
            int iRandom = rand.Next(sPictures.Length);

            //Set the picture file path
            sFilePath = sPictures[iRandom];

            //Return the selected picture
            return sFilePath;
        }

        /*
        Returns the location information for the selected picture
        */
        public bool GetImageGPS(out double[] dCoordinates)
        {
            //Initialize the coordinates vector
            dCoordinates = new double[2];

            //Initialize the EXIF reader
            ExifReader exifReader = new ExifReader(sFilePath);

            //Try getting the GPS information
            bool bLatitudeOK    = exifReader.GetTagValue(ExifTags.GPSLatitude, out double[] dLatitude);
            bool bLongitudeOK   = exifReader.GetTagValue(ExifTags.GPSLongitude, out double[] dLongitude);

            //Check if there is any GPS information
            if (bLatitudeOK && bLongitudeOK)
            {

                //Set the coordinates as sinlge double format
                dCoordinates[0] = dLatitude[0] + dLatitude[1] / 60 + dLatitude[2] / 3600;
                dCoordinates[1] = dLongitude[0] + dLongitude[1] / 60 + dLongitude[2] / 3600;

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
