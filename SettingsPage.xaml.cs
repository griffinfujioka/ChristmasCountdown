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

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace ChristmasCountdown
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SettingsPage : UserControl
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            tile_styleComboBox.SelectedIndex = 0; 
        }


        #region tile style combo box selection changed
        private void tile_styleComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            switch (tile_styleComboBox.SelectedIndex)
            {
                case 1:
                    App.Live_Tile_Style = 1;  
                    break;
                case 2:
                    App.Live_Tile_Style = 2;   
                    break;
                case 3:
                    App.Live_Tile_Style = 3;  
                    break;
                default: break;
            }

        }
        #endregion 
    }
}
