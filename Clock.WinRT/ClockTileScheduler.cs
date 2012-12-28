﻿

namespace Clock.WinRT
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using Windows.Data.Xml.Dom;
    using Windows.System.UserProfile;
    using Windows.UI.Notifications;

    
    
    public static class ClockTileScheduler
    {
        static int Live_Tile_Style = 1;  // Used to change which live tile style is shown 

        public static void SetLiveTileStyle(int style)
        {
            Live_Tile_Style = style; 
        }
        public static void CreateSchedule()
        {
            int CurrentYear = DateTime.Now.Year;                // Current year
            DateTime NewYear = new DateTime(DateTime.Now.Year + 1, 1, 1);       // January 1st of the next year

            var tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            var plannedUpdated = tileUpdater.GetScheduledTileNotifications();

            string language = GlobalizationPreferences.Languages.First();
            CultureInfo cultureInfo = new CultureInfo(language);

            DateTime now = DateTime.Now;
            DateTime planTill = now.AddHours(4);

        

            DateTime updateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0).AddMinutes(1);
           if (plannedUpdated.Count > 0)
                updateTime = plannedUpdated.Select(x => x.DeliveryTime.DateTime).Union(new [] { updateTime }).Max(); 

            // Here is where I define a number of different live tiles 
            // This is the special tile that will display on Christmas day 
            const string Christmas_xml = @"<tile><visual>
                                        <binding template=""TileSquareText04""><text id=""1"">Merry Christmas!</text></binding>
                                        <binding template=""TileWideText03""><text id=""1"">Merry Christmas!</text></binding>
                                </visual></tile>";

            // Tile 1: XX days until Christmas!
            const string xml = @"<tile><visual>
                                        <binding template=""TileSquareBlock""><text id=""1"">{0}</text><text id=""2"">days left!</text></binding>
                                        <binding template=""TileWideText01""><text id=""1"">{0}</text><text id=""2"">days until Christmas!</text></binding>
                                </visual></tile>";


            // Tile 2: XX days, YY hours until Christmas! 
            const string xml_2 = @"<tile><visual>
                                        <binding template=""TileSquareText01"">
                                            <text id=""1"">{0}</text>
                                            <text id=""2"">{1}</text>
                                            <text id=""3"">{2}</text>
                                        </binding>
                                        <binding template=""TileWideText05"">
                                            <text id=""1"">{0}</text>
                                            <text id=""2"">{1}</text>
                                            <text id=""3"">{2}</text>
                                        </binding>
                                </visual></tile>"; 

            // Tile 3: XX days, YY hours, ZZ minutes until Christmas!
            const string xml_3 = @"<tile><visual>
                                        <binding template=""TileSquareText01"">
                                            <text id=""1"">{0}</text>
                                            <text id=""2"">{1}</text>
                                            <text id=""3"">{2}</text>
                                            <text id=""4"">until Christmas!</text>
                                        </binding>
                                        <binding template=""TileWideText05"">
                                            <text id=""1"">{0}</text>
                                            <text id=""2"">{1}</text>
                                            <text id=""3"">{2}</text>
                                            <text id=""4"">until Christmas!</text>
                                        </binding>
                                </visual></tile>"; 
           
            DateTime christmas = new DateTime(DateTime.Today.Year, 12, 25);
            if (DateTime.Now.Date > christmas.Date && DateTime.Now.Date < NewYear.Date)
            {
                christmas = new DateTime(NewYear.Year, 12, 25);
            }
            var timeLeft = christmas - DateTime.Now;
            var tileXmlCountdown = ""; 
            if (DateTime.Now.Date == christmas.Date)
            {
                tileXmlCountdown = string.Format(Christmas_xml);
            }
            else if (Live_Tile_Style == 1)
            {
                tileXmlCountdown = string.Format(xml, timeLeft.Days.ToString());
            }
            else if (Live_Tile_Style == 2)
            {
                tileXmlCountdown = string.Format(xml_2, timeLeft.Days.ToString() + " days,", timeLeft.Hours == 1 ? timeLeft.Hours.ToString() + " hour" : timeLeft.Hours.ToString() + " hours", "until Christmas!");
            }
            else if(Live_Tile_Style == 3)
            {
                tileXmlCountdown = string.Format(xml_3, timeLeft.Days.ToString() + " days,", timeLeft.Hours == 1 ? timeLeft.Hours.ToString() + " hour," : timeLeft.Hours.ToString() + " hours,", timeLeft.Minutes == 1 ? timeLeft.Minutes.ToString() + " minute" : timeLeft.Minutes.ToString() + " minutes");
            }
            else    // default to style 1 
            {
                tileXmlCountdown = string.Format(xml, timeLeft.Days.ToString());
            }

                
            
            XmlDocument documentNow = new XmlDocument();
            documentNow.LoadXml(tileXmlCountdown);

            tileUpdater.Update(new TileNotification(documentNow) { ExpirationTime = now.AddMinutes(1) });

            for (var startPlanning = updateTime; startPlanning < planTill; startPlanning = startPlanning.AddMinutes(1))
            {
                Debug.WriteLine(startPlanning);
                Debug.WriteLine(planTill);

                try
                {

                    var tileXml = "";
                    if (DateTime.Now.Date == christmas.Date)
                    {
                        tileXml = string.Format(Christmas_xml); 
                    }
                    else
                    {
                        if (Live_Tile_Style == 1)
                        {
                            tileXml = string.Format(xml, timeLeft.Days.ToString());
                        }
                        else if (Live_Tile_Style == 2)
                        {
                            tileXml = string.Format(xml_2, timeLeft.Days.ToString());
                        }
                        else if (Live_Tile_Style == 3)
                        {
                            tileXml = string.Format(xml_3, timeLeft.Days.ToString());
                        }
                        else    // default to style 1 
                        {
                            tileXml = string.Format(xml, timeLeft.Days.ToString());
                        }
                    }
                    XmlDocument document = new XmlDocument();
                    document.LoadXml(tileXml);

                    ScheduledTileNotification scheduledNotification = new ScheduledTileNotification(document, new DateTimeOffset(startPlanning)) { ExpirationTime = startPlanning.AddMinutes(1) };
                    tileUpdater.AddToSchedule(scheduledNotification);

                    Debug.WriteLine("schedule for: " + startPlanning);
                }
                catch (Exception e)
                {
                    Debug.WriteLine("exception: " + e.Message);
                }
            }
        }
    }
}
