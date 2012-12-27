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

    public partial class MainPage : Page
    {
        private const string TASKNAMEUSERPRESENT = "TileSchedulerTaskUserPresent";
        private const string TASKNAMETIMER = "TileSchedulerTaskTimer";
        private const string TASKENTRYPOINT = "Clock.WinRT.TileSchedulerTask";
        public static DateTime Christmas = new DateTime(DateTime.Today.Year, 12, 25);

        private DispatcherTimer timer;

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
            
        }
        #endregion 
        
        private static Random random;

        #region Constructor
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            Loaded += OnLoaded;


            random = new Random();
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

            // Display the results
            //this.todaysDateTxtBlock.Text = results.ToString();

            
            TimeSpan ts = Christmas - DateTime.Now;
            int days = ts.Days;
            int hours = ts.Hours;
            //diffTxtBlock.Text = days + " days " + hours + "hours"; 
        }
        #endregion 

       

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
                //UpdateClockText();
                CreateClockTask();
            }
            catch (Exception ex)
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


        private static async void CreateClockTask()
        {
            var result = await BackgroundExecutionManager.RequestAccessAsync();
            if (result == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                result == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
            {
                ClockTileScheduler.CreateSchedule();

                EnsureUserPresentTask();
                EnsureTimerTask();
            }
        }

        private static void EnsureUserPresentTask()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
                if (task.Value.Name == TASKNAMEUSERPRESENT)
                    return;

            BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
            builder.Name = TASKNAMETIMER;
            builder.TaskEntryPoint = TASKENTRYPOINT;
            builder.SetTrigger(new SystemTrigger(SystemTriggerType.UserPresent, false));
            builder.Register();
        }

        private static void EnsureTimerTask()
        {
            foreach (var task in BackgroundTaskRegistration.AllTasks)
                if (task.Value.Name == TASKNAMETIMER)
                    return;

            BackgroundTaskBuilder builder = new BackgroundTaskBuilder();
            builder.Name = TASKNAMETIMER;
            builder.TaskEntryPoint = TASKENTRYPOINT;
            builder.SetTrigger(new TimeTrigger(180, false));
            builder.Register();
        }

        private void OnTick(object sender, object e)
        {
            var timeLeft = Christmas - DateTime.Now;

            Countdown.Text = string.Format("{0:D2} days\n{1:D2} hours\n{2:D2} minutes\n{3:D2} seconds", timeLeft.Days, timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds); 
        }

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

        #region About button clicked
        private void aboutBtn_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(AboutPage)); 
        }
        #endregion 

        #region Settings button clicked
        private void settingsBtn_Click_1(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(SettingsPage));
        }
        #endregion

        #region Change background color 
        public void ChangeColor(int color)
        {
            switch (color)
            {
                case 0: break;
                case 1: 
                    MainPageGrid.Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 166));
                    untilTxtBlock.Visibility = Visibility.Collapsed; 
                    break; 
                default: break; 
            }
        }
        #endregion 
    }
}
