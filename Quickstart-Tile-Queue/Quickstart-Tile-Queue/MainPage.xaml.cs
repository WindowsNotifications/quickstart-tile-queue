using NotificationsExtensions.Tiles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Xml.Dom;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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

            var notification = new TileNotification(content.GetXml())
            {
                // Specify the tag so we can replace this stock later
                Tag = symbol
            };

            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
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
