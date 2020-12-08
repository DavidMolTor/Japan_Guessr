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

            //Set an initial picture
            UpdatePicture();
        }

        /*
        Updated the picture shown
        */
        private void UpdatePicture()
        {
            imgPicture.Source = IPictureManager.Instance.GetRandomPicture();
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
