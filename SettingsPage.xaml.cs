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
    public sealed partial class SettingsPage : ChristmasCountdown.Common.LayoutAwarePage
    {
        public SettingsPage()
        {
            this.InitializeComponent();
            backgroundColorComboBox.SelectedIndex = 0;
            tile_styleComboBox.SelectedIndex = 0; 
        }


        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        #region OnNavigatedTo
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }
        #endregion 

        #region OnLoaded
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            backgroundColorComboBox.SelectedIndex = 0;
            
        }
        #endregion 

        #region backgroundColorComboBox Selection Changed 
        private void backgroundColorComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            
            switch (backgroundColorComboBox.SelectedIndex)
            {
                case 0: break;
                case 1:
                    App.Background_Color = 1;   // 1 = black
                    break; 
                case 2:
                    App.Background_Color = 2;   // 2 = red
                    break; 
                case 3:
                    App.Background_Color = 3;   // 3 = white
                    break; 
                case 4:
                    App.Background_Color = 4;   // 4 = green
                    break; 
                case 5:
                    App.Background_Color = 5;   // Restore default 
                    break; 
                default: break; 
            }
        }
        #endregion 

        #region tile style combo box selection changed
        private void tile_styleComboBox_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            switch (tile_styleComboBox.SelectedIndex)
            {
                case 0: break;
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

            Clock.WinRT.ClockTileScheduler.SetLiveTileStyle(App.Live_Tile_Style);
            MainPage.CreateClockTask(); 
        }
        #endregion 
    }
}
