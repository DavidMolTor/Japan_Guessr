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
using System.Configuration;
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

        /*
        Loaded event handler for the start menu panel
        */
        private void StartMenu_Loaded(object sender, RoutedEventArgs e)
        {
            //Get the application configuration file
            ExeConfigurationFileMap configMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = AppDomain.CurrentDomain.FriendlyName + ".config"
            };
            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

            //Check if a pictures path has been selected
            string sPicturesPath = config.AppSettings.Settings["PicturesPath"].Value;
            if (sPicturesPath == "N/D")
            {
                //Ask if the user wants to set a pictures directory
                MessageBoxResult iResult = MessageBox.Show(Properties.Resources.Main_textSetPicturesPath, "JapanGuessr", MessageBoxButton.YesNo);
                switch (iResult)
                {
                    case MessageBoxResult.Yes:
                        //Show a folder selection dialog
                        System.Windows.Forms.FolderBrowserDialog browserDialog = new System.Windows.Forms.FolderBrowserDialog();
                        if (browserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            //Set the pictures path
                            sPicturesPath = browserDialog.SelectedPath;
                        }
                        else
                        {
                            //Set the default pictures path
                            sPicturesPath = "./images";
                        }
                        break;
                    case MessageBoxResult.No:
                        //Set the default pictures path
                        sPicturesPath = "./images";
                        break;
                }

                //Save the pictures path
                config.AppSettings.Settings["PicturesPath"].Value = sPicturesPath;
                config.AppSettings.SectionInformation.ForceSave = true;
                config.Save(ConfigurationSaveMode.Full);
            }

            //Set the pictures
            IPictureManager.Instance.FindPictures(sPicturesPath);
        }

        //Selection event for main window
        public delegate void ModeSelected(GameMode iMode);
        public event ModeSelected ModeSelectedEvent;

        /*
        Selection button click event handler
        */
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Check if the pictures directory has been set
            if (IPictureManager.Instance.bDirectorySet)
            {
                //Send the selected game mode event
                GameMode iMode = (GameMode)Enum.Parse(typeof(GameMode), ((Button)sender).Name.Replace("button", ""));
                ModeSelectedEvent?.Invoke(iMode);
            }
            else
            {
                //Show the directory not found dialog
                MessageBox.Show(Properties.Resources.Main_textDirectoryError, "JapanGuessr", MessageBoxButton.OK);
            }
        }

        /*
        Select pictures directory button click event handler
        */
        private void ButtonSelectPath_Click(object sender, RoutedEventArgs e)
        {
            //Show a folder selection dialog
            System.Windows.Forms.FolderBrowserDialog browserDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (browserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //Get the application configuration file
                ExeConfigurationFileMap configMap = new ExeConfigurationFileMap
                {
                    ExeConfigFilename = AppDomain.CurrentDomain.FriendlyName + ".config"
                };
                Configuration config = ConfigurationManager.OpenMappedExeConfiguration(configMap, ConfigurationUserLevel.None);

                //Save the pictures path
                config.AppSettings.Settings["PicturesPath"].Value = browserDialog.SelectedPath;
                config.AppSettings.SectionInformation.ForceSave = true;
                config.Save(ConfigurationSaveMode.Full);
            }
        }
    }
}
