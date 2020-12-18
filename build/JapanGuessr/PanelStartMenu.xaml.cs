/*
JAPAN GUESSR 2020

Main.cs

- Description: Start menu panel
- Author: David Molina Toro
- Date: 18 - 12 - 2020
- Version: 1.0

Property of Skeptic Productions
*/

using System;
using System.Windows;
using System.Windows.Controls;

namespace JapanGuessr
{
    public partial class PanelStartMenu : UserControl
    {
        /*
        Public constructor
        */
        public PanelStartMenu()
        {
            InitializeComponent();
        }

        //Selection event for main window
        public delegate void ModeSelected(GameMode iMode);
        public event ModeSelected ModeSelectedEvent;

        /*
        Selection button click event handler
        */
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Send the selected game mode event
            GameMode iMode = (GameMode)Enum.Parse(typeof(GameMode), ((Button)sender).Name.Replace("button", ""));
            ModeSelectedEvent?.Invoke(iMode);
        }
    }
}
