using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Notifications;         // Tiles 
using NotificationsExtensions;
using NotificationsExtensions.TileContent;
using Windows.Storage;          // ApplicationData

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ChristmasCountdown
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SettingsPage : UserControl
    {
        private Windows.Foundation.Collections.IPropertySet appSettings;
        public SettingsPage()
        {
            this.InitializeComponent();
            tile_styleComboBox.SelectedIndex = 0;
            appSettings = ApplicationData.Current.LocalSettings.Values;
        }


        #region tile style combo box selection changed
        private void tile_styleComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            appSettings = ApplicationData.Current.LocalSettings.Values;

            switch (tile_styleComboBox.SelectedIndex)
            {
                case 0:
                    App.Live_Tile_Style = 0;
                    appSettings["LiveTileStyle"] = 0; 
                    break;
                case 1:
                    App.Live_Tile_Style = 1;
                    appSettings["LiveTileStyle"] = 1; 
                    break;
                case 2:
                    App.Live_Tile_Style = 2;
                    appSettings["LiveTileStyle"] = 2; 
                    break;
                default: break;
            }

        }
        #endregion 
    }
}
