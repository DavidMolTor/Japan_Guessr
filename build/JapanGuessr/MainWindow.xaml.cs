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

            //Bind the mode selection to this class
            panelStartMenu.ModeSelectedEvent += HandleModeSelected;
        }

        /*
        Mode selected event handler
        */
        private void HandleModeSelected(GameMode iMode)
        {
            //Initialize the selection panel
            panelSelection.SetGameMode(iMode);

            //Hide the start menu and show the selection panel
            panelSelection.Visibility = Visibility.Visible;
            panelStartMenu.Visibility = Visibility.Hidden;
        }
    }
}
