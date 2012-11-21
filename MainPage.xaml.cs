using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Windows.Foundation;
using System.Windows;
using System.Windows.Input;
using System.Threading;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;       // ellipse
using Windows.UI.Xaml.Media.Animation;  // storyboard
using Windows.UI.Xaml.Media;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Markup;
using Windows.UI;           // colors
using Windows.Globalization.DateTimeFormatting; // datetime formating
using Windows.UI.Notifications;
using Windows.Data.Xml.Dom;     /* tile API */
using ChristmasCountdown.Common;
using Windows.ApplicationModel.Activation;
using NotificationsExtensions.TileContent; /* live tile */
using Windows.ApplicationModel.Background; /* background tasks */
//using BackgroundTasks; 
/*
 * I'm basically porting this app over from a Windows Phone 7 tutorial 
 * and adding some new features, namely a date difference calculation, 
 * and calling it a Christmas Countdown. I've never ported from Windows Phone 7 
 * to Windows 8 before and am in the process of determining if I can go through 
 * and just add the missing libraries to make this thing work. 
 */

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace ChristmasCountdown
{

    
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : ChristmasCountdown.Common.LayoutAwarePage
    {
        private DispatcherTimer timer;
        private const string TASK_NAME = "TileUpdater";
        private const string TASK_ENTRY = "BackgroundTasks.TileUpdater";

        #region On Navigated To
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            var result = await BackgroundExecutionManager.RequestAccessAsync();
           
            if (result == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                result == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
            {
                foreach (var task in BackgroundTaskRegistration.AllTasks)
                {
                    if (task.Value.Name == TASK_NAME)
                        task.Value.Unregister(true);
                }

                BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
                builder.Name = TASK_NAME;
                builder.TaskEntryPoint = TASK_ENTRY;
                builder.SetTrigger(new TimeTrigger(15, false));
                var registration = builder.Register();
            }
            
            
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        { 
            base.OnNavigatedFrom(e);
        }
        #endregion 
        
        private static Random random;
        public static TileUpdater updater;
        public static int tileScheduler; 

        #region Constructor
        // Constructor
        public MainPage()
        {
            this.InitializeComponent();
           // RegisterMaintenanceBackgroundTask();

            Loaded += OnLoaded;

            random = new Random();
            tileScheduler = 0; 
            this.StartFallingSnowAnimation();

            // We keep results in this variable
            StringBuilder results = new StringBuilder();
            results.AppendLine();

            // Create basic date/time formatters.
            DateTimeFormatter[] basicFormatters = new[]
            {
                // Default date formatters
                new DateTimeFormatter("shortdate"),
                new DateTimeFormatter("longdate"),

                // Default time formatters
                new DateTimeFormatter("shorttime"),
                new DateTimeFormatter("longtime"),
             };

            // Create date/time to format, manipulate and display.
            DateTime dateTime = DateTime.Now;
            // Try to format and display date/time if calendar supports it.
            foreach (DateTimeFormatter formatter in basicFormatters)
            {
                try
                {
                    // Format and display date/time.
                    results.AppendLine(formatter.Template + ": " + formatter.Format(dateTime));
                }
                catch (ArgumentException)
                {
                    // Retrieve and display formatter properties. 
                    results.AppendLine(String.Format(
                        "Unable to format Gregorian DateTime {0} using formatter with template {1} for languages [{2}], region {3}, calendar {4} and clock {5}",
                        dateTime,
                        formatter.Template,
                        string.Join(",", formatter.Languages),
                        formatter.GeographicRegion,
                        formatter.Calendar,
                        formatter.Clock));
                }
            }

            DateTime Christmas = new DateTime(DateTime.Today.Year, 12, 25);
            TimeSpan ts = Christmas - DateTime.Now;
            int days = ts.Days;
            int hours = ts.Hours;

            var christmas = new DateTime(DateTime.Today.Year, 12, 25);
            var timeLeft = christmas - DateTime.Now;

            /*
            ITileWideText03 tileContent = TileContentFactory.CreateTileWideText03();
            tileContent.TextHeadingWrap.Text = string.Format("{0:D2} days until Christmas!", timeLeft.Days); 

            ITileSquareText04 squareContent = TileContentFactory.CreateTileSquareText04();
            squareContent.TextBodyWrap.Text = string.Format("{0:D2} days until Christmas!", timeLeft.Days);  

            tileContent.SquareContent = squareContent;
            */ 

            //TileNotification notification = tileContent.CreateNotification();
            updater = TileUpdateManager.CreateTileUpdaterForApplication();
            //updater.Update(notification); 
            

            /* Trying to figure out how to cycle through live tile while the app is completely closed */
           // updater.EnableNotificationQueue(true);

             
            
            

        }

        
        #endregion 

        #region On Loaded
        private void OnLoaded(object sender, RoutedEventArgs e)
        {

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            timer.Tick += new EventHandler<object>(OnTick);

            timer.Start(); 

            
        }
        #endregion 

        #region Set time textblock 
        private void SetTimeTextBlock(string str)
        {
            Countdown.Text = str;
        }
        #endregion 

        #region On Tick 
        private void OnTick(object sender, object e)
        {
            /* These should be global variables */ 
            var christmas = new DateTime(DateTime.Today.Year, 12, 25);
            var timeLeft = christmas - DateTime.Now;
            

            /*
             * tileSchedular is used to toggle the tile on/off while the app is suspended. 
             * That is, the app is running but not open 
             */  
            if (tileScheduler % 20 == 0)
            {
                updater.Clear();       
            }
            else if (tileScheduler % 10 == 0)
            {
                ITileWideText03 tileContent = TileContentFactory.CreateTileWideText03();
                tileContent.TextHeadingWrap.Text = string.Format("{0:D2} days until Christmas!", timeLeft.Days);

                ITileSquareText04 squareContent = TileContentFactory.CreateTileSquareText04();
                squareContent.TextBodyWrap.Text = string.Format("{0:D2} days until Christmas!", timeLeft.Days);

                tileContent.SquareContent = squareContent;
                TileNotification notification = tileContent.CreateNotification();
                updater.Update(notification); 
            }
            tileScheduler += 1; 
            

            Countdown.Text = string.Format("{0:D2} days\n{1:D2} hours\n{2:D2} minutes\n{3:D2} seconds", timeLeft.Days, timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);


        }
        #endregion 

        #region Start the snow fall
        private void StartFallingSnowAnimation()
        {
            for (int i = 0; i < 40; i++)
            {
                Ellipse localCopy = this.GenerateEllipse();
                localCopy.SetValue(Canvas.LeftProperty, i * 30 + 1.0);

                double y = Canvas.GetTop(localCopy);
                double x = Canvas.GetLeft(localCopy);

                double speed = 5 * random.NextDouble();
                double index = 0;
                double radius = 30 * speed * random.NextDouble();

                localCopy.Opacity = .3 + random.NextDouble();

                CompositionTarget.Rendering += delegate(object o, object arg)
                {
                    index += Math.PI / (180 * speed);

                    if (y < this.ContentPanel.DesiredSize.Height)
                    {
                        y += .3 + speed;
                    }
                    else
                    {
                        y = -localCopy.Height;
                    }

                    Canvas.SetTop(localCopy, y);
                    Canvas.SetLeft(localCopy, x + radius * Math.Cos(index));
                    Storyboard animation = this.CreateStoryboard(localCopy, y, x + radius * Math.Cos(index));
                    Storyboard.SetTarget(animation, localCopy);

                    animation.Begin();

                };
            }
        }
        #endregion 

        #region Generate an Ellipse
        private Ellipse GenerateEllipse()
        {
            Ellipse element = new Ellipse();
            element.Fill = new SolidColorBrush(Colors.White);
            element.Height = 10.0;
            element.Width = 10.0;
            this.ContentPanel.Children.Add(element);
            return element;
        }
        #endregion 

        #region Create storyboard
        private Storyboard CreateStoryboard(UIElement element, double to, double toLeft)
        {
            Storyboard result = new Storyboard();
            DoubleAnimation animation = new DoubleAnimation();
            animation.To = to;
            //Storyboard.SetTargetProperty(animation, new PropertyPath("(Canvas.Top)"));
            Storyboard.SetTargetProperty(animation, "(Canvas.Top)");
            Storyboard.SetTarget(animation, element);

            DoubleAnimation animationLeft = new DoubleAnimation();
            animationLeft.To = toLeft;
            //Storyboard.SetTargetProperty(animationLeft, new PropertyPath("(Canvas.Left)"));
            Storyboard.SetTargetProperty(animationLeft, "(Canvas.Left)"); 
            Storyboard.SetTarget(animationLeft, element);

            result.Children.Add(animation);
            result.Children.Add(animationLeft);

            return result;
        }
        #endregion 

        #region About textblock tapped 
        private void TextBlock_Tapped_1(object sender, TappedRoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AboutPage)); 
        }
        #endregion 

        #region Register Maintenance Trigger - not in use 
        //Registering the maintenance trigger background task   
        /* 
        private bool RegisterMaintenanceBackgroundTask()
        {

            
            BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
            updater = TileUpdateManager.CreateTileUpdaterForApplication();
            builder.Name = "Maintenance background task";
            builder.TaskEntryPoint = "MaintenanceTask.MaintenaceBackgroundTask";
            // Run every 8 hours if the device is on AC power 
            IBackgroundTrigger trigger = new MaintenanceTrigger(15, false);
            builder.SetTrigger(trigger);
            IBackgroundTaskRegistration task = builder.Register();

            var christmas = new DateTime(DateTime.Today.Year, 12, 25);
            var timeLeft = christmas - DateTime.Now;

            if (DateTime.Now.Minute % 30 < 15)
            {
                updater.Clear();
            }
            else
            {
                ITileWideText03 tileContent = TileContentFactory.CreateTileWideText03();
                tileContent.TextHeadingWrap.Text = string.Format("{0:D2} days until Christmas!", timeLeft.Days);

                ITileSquareText04 squareContent = TileContentFactory.CreateTileSquareText04();
                squareContent.TextBodyWrap.Text = string.Format("{0:D2} days until Christmas!", timeLeft.Days);

                tileContent.SquareContent = squareContent;
                TileNotification notification = tileContent.CreateNotification();
                updater.Update(notification);

            }

            return true;
        }*/
        #endregion 

    }

}
