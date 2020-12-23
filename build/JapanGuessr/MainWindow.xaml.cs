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

            //Bind the mode selection event to this class
            panelStartMenu.ModeSelectedEvent += HandleModeSelected;

            //Bind the return to start panel event to this class
            panelSelection.ReturnToStartEvent += ReturnToStart;
        }

        /*
        Mode selected event handler
        */
        private void HandleModeSelected(GameMode iMode)
        {
            //Hide the start menu and show the selection panel
            panelSelection.Visibility = Visibility.Visible;
            panelStartMenu.Visibility = Visibility.Hidden;

            //Initialize the selection panel
            panelSelection.SetGameMode(iMode);
        }


        /*
        Return to start panel event handler
        */
        private void ReturnToStart()
        {
            //Hide the selection panel and show the start menu
            panelStartMenu.Visibility = Visibility.Visible;
            panelSelection.Visibility = Visibility.Hidden;
        }
    }
}
