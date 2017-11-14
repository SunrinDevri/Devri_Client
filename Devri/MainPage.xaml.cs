using Devri.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using Windows.Devices.Gpio;
using Windows.Devices.Sensors.Temperature;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using Windows.Devices.Geolocation;

// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x412에 나와 있습니다.

namespace Devri
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : BindablePage
    {
        private float m_temperature;        
        public float Temperature
        {
            get { return m_temperature; }
            set
            {
                SetProperty(ref m_temperature,value);
                OnPropertyChanged(nameof(DisplayTemperature));
            }
        }

        public string DisplayTemperature
        {
            get
            {
                return string.Format("{0} C", Temperature);
            }
        }
        private float m_humidity;
        public float Humidity
        {
            get { return m_humidity; }
            set
            {
                SetProperty(ref m_temperature, value);
                OnPropertyChanged(nameof(DisplayHumidity));
            }
        }

        public string DisplayHumidity
        {
            get
            {
                return string.Format("{0} C", Humidity);
            }
        }

        public MainPage()
        {
            this.InitializeComponent();
            SetupGeo(); 
            Read();
        }

        public async void Read()
        {
            var gpio = GpioController.GetDefault();
            var dataPin = gpio.OpenPin(5);
            var sensor = new DHT11(dataPin, SensorTypes.DHT11);
            for (int i = 1; i < 100; i++)
            {
                var humidity = await sensor.ReadHumidity();
                var temperature = await sensor.ReadTemperature(false);
                Debug.WriteLine(temperature);
                await Task.Delay(2000);
            }
        }
        public static System.Threading.Timer timer;
        public object DisPatcher { get; private set; }

        public void Timer_Start()
        {
            
            timer = new System.Threading.Timer(timerCallback, null, 0,10);

        }

        private async void timerCallback(object state)
        {
            //Put Something you want to run async
            await Window.Current.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
            () => {
                // do some work on UI here;
            });
        }

        public void Stop_Image()
        {
            
        }



        public static Geolocator Geolocator { get; set; }
        public static bool RunningInBackground { get; set; }
        private void SetupGeo()
        {
            if (Geolocator == null)
            {
                Geolocator = new Geolocator();
                Geolocator.DesiredAccuracy = PositionAccuracy.High;
                Geolocator.MovementThreshold = 100;
                Geolocator.PositionChanged += Geolocator_PositionChanged;
            }
        }
        private void Geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            geolocator_PositionChanged(sender, args);
        }

        private async void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {   //Point point;
                //point.Position.Latitude
                String wido=args.Position.Coordinate.Latitude.ToString("0.00");
                String gyungdo=args.Position.Coordinate.Longitude.ToString("0.00");
            });

        }





    }
}
