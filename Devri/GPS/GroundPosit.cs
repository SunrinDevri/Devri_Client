using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Foundation;
using Windows.Storage.Streams;

namespace Devri.GPS
{
    class GroundPosit
    {
        private UBXSerialGPS gps;
        private MapIcon myLocation;
        private RandomAccessStreamReference mapIconStreamReference;


        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            // Get serial device
            string aqs = SerialDevice.GetDeviceSelector();
            var dis = await DeviceInformation.FindAllAsync(aqs);

            // Create configuration object
            Configuration.Port cfg_prt = new Configuration.Port()
            {
                PortID = 1,
                StopBit = Configuration.Port.StopBitType.OneStop,
                Parity = Configuration.Port.ParityType.NoParity,
                CharacterLength = Configuration.Port.CharacterLengthType.Bit8,
                BaudRate = 115200,
                InputProtocol = Configuration.Port.Protocol.UBX,
                OutputProtocol = Configuration.Port.Protocol.UBX
            };

            // Instantiate UBXSerialGPS
            gps = new UBXSerialGPS(dis[0], cfg_prt);

            // Register event handler for periodic data
            gps.MessageReceived += Gps_MessageReceived;
            statusTextBox.Text = "GPS Started";

            // Start the GPS
            await gps.Start();

            // When the code reach here, the GPS is up and running
            statusTextBox.Text = "GPS init completed";

            // Configure GPS to send geoposition data periodically
            Configuration.Message cfg_msg = Configuration.Message.GetConfigurationForType<Navigation.GeodeticPosition>();

            // Write the config to GPS
            bool res = await gps.WriteConfigAsync(cfg_msg);

            if (res)
            {
                statusTextBox.Text = "Success configuring message";
                await Task.Delay(5000);
            }
            else
            {
                statusTextBox.Text = "Failed configuring message";
                await Task.Delay(5000);
            }

            statusTextBox.Text = "Polling message Monitor Receiver Status";

            // Poll a data of specified type from the GPS module
            Navigation.Clock resp = await gps.PollMessageAsync<Navigation.Clock>();

            if (resp != null)
            {
                statusTextBox.Text = "Poll message success: " + resp.TimeMillisOfWeek;
            }
            else
            {
                statusTextBox.Text = "Poll message failed";
            }
        }

        private void Gps_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            // Message is received, try to cast to our desired data
            Navigation.GeodeticPosition pos = e.ReceivedMessage as Navigation.GeodeticPosition;

            // If it is correct casting, process data
            if (pos != null)
            {
                statusTextBox.Text = "GPS Data Received at " + DateTime.Now.ToString();

                StringBuilder bldr = new StringBuilder();
                bldr.AppendLine("GPS Information");
                bldr.AppendLine("Latitude: " + pos.Latitude);
                bldr.AppendLine("Longitude: " + pos.Longitude);
                bldr.AppendLine("Time: " + pos.TimeMillisOfWeek);
                bldr.AppendLine("MSL: " + pos.HeightAboveSeaLevel);

                contentTextBox.Text = bldr.ToString();

                if (myLocation == null)
                {

                    myLocation = new MapIcon();
                    myLocation.Location = mapView.Center;
                    myLocation.NormalizedAnchorPoint = new Point(0.5, 1.0);
                    myLocation.Image = mapIconStreamReference;
                    myLocation.ZIndex = 0;
                    mapView.MapElements.Add(myLocation);
                }
            }
        }
    }
}
