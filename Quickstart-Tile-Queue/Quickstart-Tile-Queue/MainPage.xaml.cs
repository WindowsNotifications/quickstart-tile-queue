using Microsoft.Toolkit.Uwp.Notifications; // NuGet package "Microsoft.Toolkit.Uwp.Notifications"
using System;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

            // Clear the current tile
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();

            // Send five notifications for five different stocks
            SendStockTileNotification("MSFT", 54.40, -3.55, DateTime.Now.AddMinutes(-10));
            SendStockTileNotification("TSLA", 223.41, -6.92, DateTime.Now.AddMinutes(-9));
            SendStockTileNotification("AMZN", 636.99, -5.76, DateTime.Now.AddMinutes(-7));
            SendStockTileNotification("DOW", 17148.94, -1.58, DateTime.Now.AddMinutes(-5));
            SendStockTileNotification("NASDAQ", 4903.09, -2.08, DateTime.Now.AddMinutes(-3));
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
                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new AdaptiveText()
                                {
                                    Text = symbol
                                },

                                new AdaptiveText()
                                {
                                    Text = price.ToString("N")
                                },

                                new AdaptiveText()
                                {
                                    Text = percentString
                                },

                                new AdaptiveText()
                                {
                                    Text = dateUpdated.ToString("t"),
                                    HintStyle = AdaptiveTextStyle.CaptionSubtle
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
