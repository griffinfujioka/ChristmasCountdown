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
using Windows.UI.Xaml.Shapes;                       // ellipse
using Windows.UI.Xaml.Media.Animation;           // storyboard
using Windows.UI;           // colors
using Windows.Globalization.DateTimeFormatting; // datetime formatting
using Windows.ApplicationModel.Background;      // background tasks 
using Clock.WinRT;
using ChristmasCountdown.Common;
using Windows.UI.Notifications;         // Tiles 
using NotificationsExtensions;
using NotificationsExtensions.TileContent;
using Windows.Storage; 

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

    public partial class MainPage : Common.LayoutAwarePage
    {
        #region Variable declarations
        private const string TASKNAMEUSERPRESENT = "TileSchedulerTaskUserPresent";
        private const string TASKNAMETIMER = "TileSchedulerTaskTimer";
        private const string TASKENTRYPOINT = "Clock.WinRT.TileSchedulerTask";
        private const String LiveTileStyle = "LiveTileStyle"; 
        public static DateTime Christmas = new DateTime(DateTime.Today.Year, 12, 25);
        private Windows.Foundation.Collections.IPropertySet appSettings;
        private DispatcherTimer timer;
        private static Random random;
        #endregion

        #region On Navigated From
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            TimeSpan ts = Christmas - DateTime.Now;
            int days = ts.Days;
            int hours = ts.Hours;
            int minutes = ts.Minutes; 

            // Setup live tile 
            // Check to see if tile updating is enabled
            var tile = TileUpdateManager.CreateTileUpdaterForApplication();
            if (tile.Setting != NotificationSetting.Enabled)
                return;

            App.Live_Tile_Style = Convert.ToInt32(appSettings["LiveTileStyle"]); 

            switch (App.Live_Tile_Style)
            {

                case 0:
                    var tilecontent = TileContentFactory.CreateTileWideText01();
                    tilecontent.TextHeading.Text = days.ToString() + " days";
                    tilecontent.TextBody1.Text = "until Christmas!"; 
                    ITileSquareText01 squarecontent = TileContentFactory.CreateTileSquareText01(); 
                    squarecontent.TextBody1.Text = days + " days";
                    squarecontent.TextBody2.Text = "until Christmas!";
                    tilecontent.SquareContent = squarecontent; 
                    ScheduledTileNotification scheduledTile = new ScheduledTileNotification(tilecontent.GetXml(), DateTime.Now.AddSeconds(10));
                    var tileupdater = TileUpdateManager.CreateTileUpdaterForApplication();
                    tileupdater.EnableNotificationQueue(true);
                    tileupdater.AddToSchedule(scheduledTile); 
                    break;
                case 1:
                    var tilecontent2 = TileContentFactory.CreateTileWideText05();
                    tilecontent2.TextBody1.Text = days.ToString() + " days";
                    tilecontent2.TextBody2.Text = hours.ToString() + " hours"; 
                    tilecontent2.TextBody3.Text = "until Christmas!";
                    ITileSquareText01 squarecontent2 = TileContentFactory.CreateTileSquareText01(); 
                    squarecontent2.TextHeading.Text = days.ToString() + " days";
                    squarecontent2.TextBody1.Text = hours.ToString() + " hours"; 
                    squarecontent2.TextBody2.Text = "until Christmas!";
                    
                    tilecontent2.SquareContent = squarecontent2; 
                    ScheduledTileNotification scheduledTile2 = new ScheduledTileNotification(tilecontent2.GetXml(), DateTime.Now.AddSeconds(10));
                    var tileupdater2 = TileUpdateManager.CreateTileUpdaterForApplication();
                    tileupdater2.EnableNotificationQueue(true);
                    tileupdater2.AddToSchedule(scheduledTile2); 
                    break;
                case 2:
                    var tilecontent3 = TileContentFactory.CreateTileWideText05();
                    tilecontent3.TextBody1.Text = days.ToString() + " days";
                    tilecontent3.TextBody2.Text = hours.ToString() + " hours";
                    tilecontent3.TextBody3.Text = minutes.ToString() + " minutes";
                    tilecontent3.TextBody4.Text = "until Christmas!";
                    ITileSquareText01 squarecontent3 = TileContentFactory.CreateTileSquareText01();
                    squarecontent3.TextHeading.Text = days.ToString() + " days";
                    squarecontent3.TextBody1.Text = hours.ToString() + " hours";
                    squarecontent3.TextBody2.Text = minutes.ToString() + " minutes";
                    squarecontent3.TextBody3.Text = "until Christmas!";

                    tilecontent3.SquareContent = squarecontent3; 
                    ScheduledTileNotification scheduledTile3 = new ScheduledTileNotification(tilecontent3.GetXml(), DateTime.Now.AddSeconds(10));
                    var tileupdater3 = TileUpdateManager.CreateTileUpdaterForApplication();
                    tileupdater3.EnableNotificationQueue(true);
                    tileupdater3.AddToSchedule(scheduledTile3); 
                    break;
                default:
                    var tilecontent1 = TileContentFactory.CreateTileWideText01();
                    tilecontent1.TextHeading.Text = days.ToString() + " days";
                    tilecontent1.TextBody1.Text = "until Christmas!"; 
                    ITileSquareText01 squarecontent1 = TileContentFactory.CreateTileSquareText01(); 
                    squarecontent1.TextBody1.Text = days + " days";
                    squarecontent1.TextBody2.Text = "until Christmas!";
                    tilecontent1.SquareContent = squarecontent1; 
                    ScheduledTileNotification scheduledTile1 = new ScheduledTileNotification(tilecontent1.GetXml(), DateTime.Now.AddSeconds(10));
                    var tileupdater1 = TileUpdateManager.CreateTileUpdaterForApplication();
                    tileupdater1.EnableNotificationQueue(true);
                    tileupdater1.AddToSchedule(scheduledTile1); 
                    break;
            }

            base.OnNavigatedFrom(e);

            //CreateClockTask();
        }
        #endregion 

        #region On Navigated To
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            int CurrentYear = DateTime.Now.Year;                /* Get Current Year */
            DateTime NewYear = new DateTime(DateTime.Now.Year + 1, 1, 1);       /* January 1st of the next year */

            #region Set page background using App.Background_Color
            switch (App.Background_Color)
            {
                case 0: break;
                case 1:
                    MainPageGrid.Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));    // Black
                    break;
                case 2:
                    MainPageGrid.Background = new SolidColorBrush(Color.FromArgb(255, 220, 20, 60));    // Red 
                    break;
                case 3:
                    MainPageGrid.Background = new SolidColorBrush(Color.FromArgb(255, 245, 245, 245));    // White
                    untilTxtBlock.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));    // Black
                    Countdown.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));    // Black
                    break;
                case 4:
                    MainPageGrid.Background = new SolidColorBrush(Color.FromArgb(255, 34, 139, 34));    // White
                    break;
                case 5:
                    MainPageGrid.Background = new SolidColorBrush(Color.FromArgb(255, 96, 96, 96));
                    break;
                default: break;
            }
            #endregion


        }
        #endregion

        #region Constructor
        // Constructor
        public MainPage()
        {
            appSettings = ApplicationData.Current.LocalSettings.Values;

            InitializeComponent();

            #region  Setup the background task
            BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
            builder.Name = TASKNAMEUSERPRESENT;
            builder.TaskEntryPoint = TASKENTRYPOINT;
            builder.SetTrigger(new SystemTrigger(SystemTriggerType.UserPresent, false));
            builder.Register();
            var registration = builder.Register();
            #endregion 

            #region Setup the screen resolution
            var bounds = Window.Current.Bounds;
            double height = bounds.Height;
            double width = bounds.Width;

            ContentPanel.Height = height;
            ContentPanel.Width = width;
            MainPageGrid.Height = height;
            MainPageGrid.Width = width;
            #endregion 


            random = new Random();
            this.StartFallingSnowAnimation();

            // We keep results in this variable
            //StringBuilder results = new StringBuilder();
            //results.AppendLine();

            //// Create basic date/time formatters.
            //DateTimeFormatter[] basicFormatters = new[]
            //{
            //    // Default date formatters
            //    new DateTimeFormatter("shortdate"),
            //    new DateTimeFormatter("longdate"),

            //    // Default time formatters
            //    new DateTimeFormatter("shorttime"),
            //    new DateTimeFormatter("longtime"),
            // };

            //// Create date/time to format, manipulate and display.
            //DateTime dateTime = DateTime.Now;
            //// Try to format and display date/time if calendar supports it.
            //foreach (DateTimeFormatter formatter in basicFormatters)
            //{
            //    try
            //    {
            //        // Format and display date/time.
            //        results.AppendLine(formatter.Template + ": " + formatter.Format(dateTime));
            //    }
            //    catch (ArgumentException)
            //    {
            //        // Retrieve and display formatter properties. 
            //        results.AppendLine(String.Format(
            //            "Unable to format Gregorian DateTime {0} using formatter with template {1} for languages [{2}], region {3}, calendar {4} and clock {5}",
            //            dateTime,
            //            formatter.Template,
            //            string.Join(",", formatter.Languages),
            //            formatter.GeographicRegion,
            //            formatter.Calendar,
            //            formatter.Clock));
            //    }
            //}

            // Display the results
            //this.todaysDateTxtBlock.Text = results.ToString();


            TimeSpan ts = Christmas - DateTime.Now;
            int days = ts.Days;
            int hours = ts.Hours;
            //diffTxtBlock.Text = days + " days " + hours + "hours"; 

            Loaded += OnLoaded;
        }
        #endregion

        #region OnLoaded
        private async void OnLoaded(object sender, RoutedEventArgs e)
        {
            DateTime TestDate = new DateTime(DateTime.Now.Year, 11, 15);
            int CurrentYear = DateTime.Now.Year;                /* Get Current Year */
            DateTime NewYear = new DateTime(DateTime.Now.Year + 1, 1, 1);       /* January 1st of the next year */

            timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1)
            };

            timer.Tick += new EventHandler<object>(OnTick);

            try
            {
                timer.Start();
            }
            catch
            {
            }

            // If today is Christmas, behave accordingly!! 
            #region Display pop up a message if it's Christmas day
            if (DateTime.Now.Date == Christmas.Date)
            {
                App.isChristmas = true;
                #region Hide all countdown controls
                untilTxtBlock.Visibility = Visibility.Collapsed;
                Countdown.Visibility = Visibility.Collapsed;
                #endregion
                // Print out a Merry Christmas message instead
                merryChristmasTxtBlock.Visibility = Visibility.Visible;
                treeImg.Visibility = Visibility.Visible;
                CurrentYear = DateTime.Now.Year;
                Windows.UI.Popups.MessageDialog dialog = new Windows.UI.Popups.MessageDialog("Merry Christmas and Happy Holidays!\n\nThank you for your support!");
                await dialog.ShowAsync();
            }
            else if (DateTime.Now.Date > Christmas.Date && DateTime.Now.Date < NewYear.Date)
            {
                // Cover the cases Dec. 26th - 31st. Update Christmas to Christmas day of next year
                Christmas = new DateTime(NewYear.Year, 12, 25);
                App.isChristmas = false;
            }
            #endregion



        }
        #endregion

        //#region CreateClockTask
        //public static async void CreateClockTask()
        //{
        //    try
        //    {

        //        var result = await BackgroundExecutionManager.RequestAccessAsync();
        //        if (result == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
        //            result == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
        //        {
        //            foreach (var task in BackgroundTaskRegistration.AllTasks)
        //            {
        //                if (task.Value.Name == TASKNAMEUSERPRESENT)
        //                    task.Value.Unregister(true);
        //            }
        //            ClockTileScheduler.CreateSchedule();
        //            EnsureUserPresentTask();
        //            EnsureTimerTask();

        //            BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
        //            builder.Name = TASKNAMEUSERPRESENT;
        //            builder.TaskEntryPoint = TASKENTRYPOINT;
        //            builder.SetTrigger(new SystemTrigger(SystemTriggerType.UserPresent, false));
        //            builder.Register();
        //            var registration = builder.Register();
        //        }
        //    }
        //    catch
        //    {

        //    }


        //}
        //#endregion

        //#region EnsureUserPresentTask
        //private static void EnsureUserPresentTask()
        //{
        //    foreach (var task in BackgroundTaskRegistration.AllTasks)
        //        if (task.Value.Name == TASKNAMEUSERPRESENT)
        //            return;

        //    BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
        //    builder.Name = TASKNAMETIMER;
        //    builder.TaskEntryPoint = TASKENTRYPOINT;
        //    builder.SetTrigger(new SystemTrigger(SystemTriggerType.UserPresent, false));
        //    builder.Register();
        //}
        //#endregion

        //#region EnsureTimerTask
        //private static void EnsureTimerTask()
        //{
        //    foreach (var task in BackgroundTaskRegistration.AllTasks)
        //        if (task.Value.Name == TASKNAMETIMER)
        //            return;

        //    BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
        //    builder.Name = TASKNAMETIMER;
        //    builder.TaskEntryPoint = TASKENTRYPOINT;
        //    builder.SetTrigger(new TimeTrigger(180, false));
        //    builder.Register();
        //}
        //#endregion

        #region OnTick
        private void OnTick(object sender, object e)
        {
            var timeLeft = Christmas - DateTime.Now;

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
    }
}
