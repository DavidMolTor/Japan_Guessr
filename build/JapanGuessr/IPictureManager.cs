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
using System.Windows.Media;
using System.Windows.Media.Imaging;

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

        /*
        Returns a random picture as a bitmap
        */
        public ImageSource GetRandomPicture()
        {
            //Get all the pictures from the images folder
            string[] sPictures = Directory.GetFiles("./images", "*.jpg");

            //Generate a random number from the image count
            Random rand = new Random();
            int iRandom = rand.Next(sPictures.Length);

            //Return the selected image
            return new BitmapImage(new Uri(sPictures[iRandom], UriKind.Relative));
        }
    }
}
