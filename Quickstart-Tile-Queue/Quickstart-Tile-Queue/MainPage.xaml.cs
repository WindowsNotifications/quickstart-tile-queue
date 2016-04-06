using NotificationsExtensions.Tiles; // NuGet package "NotificationsExtensions.Win10"
using System;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Linq;
using Windows.Foundation.Metadata;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Quickstart_Tile_Queue
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();

            UpdateTileActivatedInfo();

            // Clear the current tile
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();

            // Send five notifications for five different stocks
            SendStockTileNotification("MSFT", 54.40, -3.55, DateTime.Now.AddMinutes(-10));
            SendStockTileNotification("TSLA", 223.41, -6.92, DateTime.Now.AddMinutes(-9));
            SendStockTileNotification("AMZN", 636.99, -5.76, DateTime.Now.AddMinutes(-7));
            SendStockTileNotification("DOW", 17148.94, -1.58, DateTime.Now.AddMinutes(-5));
            SendStockTileNotification("NASDAQ", 4903.09, -2.08, DateTime.Now.AddMinutes(-3));
        }

        public void UpdateTileActivatedInfo()
        {
            // TileActivatedInfo is set upon app launch in App.xaml.cs.
            if (App.TileActivatedInfo == null)
            {
                TextBlockLaunchedFrom.Text = "Not launched from Live Tile (or chaseability isn't supported on this system).";
                TextBlockTileInfo.Visibility = Visibility.Collapsed;
            }

            else
            {
                TextBlockLaunchedFrom.Text = "Launched from Live Tile.";

                if (App.TileActivatedInfo.RecentlyShownNotifications.Count == 0)
                    TextBlockTileInfo.Text = "Live Tile was blank (no notifications).";

                else
                {
                    // The first ShownTileNotification is the one that the user tapped
                    TextBlockTileInfo.Text = "Tapped tile notification: " + App.TileActivatedInfo.RecentlyShownNotifications[0].Arguments;

                    // Eliminate duplicates (like updated notifications for the same stock)
                    string[] distinctArguments = App.TileActivatedInfo.RecentlyShownNotifications.Select(i => i.Arguments).Distinct().ToArray();

                    if (distinctArguments.Length > 1)
                    {
                        TextBlockTileInfo.Text += "\nOther recently seen notifications...";

                        foreach (var shownNotifArgs in distinctArguments.Skip(1))
                        {
                            TextBlockTileInfo.Text += "\n - " + shownNotifArgs;
                        }
                    }
                }

                TextBlockTileInfo.Visibility = Visibility.Visible;
            }
        }

        private void SendStockTileNotification(string symbol, double price, double percentChange, DateTime dateUpdated)
        {
            // Generate the notification content
            XmlDocument content = GenerateNotificationContent(symbol, price, percentChange, dateUpdated);

            // Create the tile notification
            TileNotification notification = new TileNotification(content);

            // Set the tag so that we can update (replace) this notification later
            notification.Tag = symbol;

            // Send the notification
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }

        private XmlDocument GenerateNotificationContent(string symbol, double price, double percentChange, DateTime dateUpdated)
        {
            string percentString = (percentChange < 0 ? "▼" : "▲") + " " + percentChange + "%";

            var content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    // Include arguments so that we know which tile notification was clicked
                    Arguments = symbol,

                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new TileText()
                                {
                                    Text = symbol
                                },

                                new TileText()
                                {
                                    Text = price.ToString("N")
                                },

                                new TileText()
                                {
                                    Text = percentString
                                },

                                new TileText()
                                {
                                    Text = dateUpdated.ToString("t"),
                                    Style = TileTextStyle.CaptionSubtle
                                }
                            }
                        }
                    }
                }
            };

            return content.GetXml();
        }

        private void ButtonSendNotification_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SendStockTileNotification(TextBoxSymbol.Text, double.Parse(TextBoxPrice.Text), double.Parse(TextBoxPercentChange.Text), DateTime.Today.Add(TimePickerTimeUpdated.Time));
            }

            catch (Exception ex)
            {
                var dontWait = new MessageDialog(ex.ToString()).ShowAsync();
            }
        }
    }
}
