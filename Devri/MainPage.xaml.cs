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
using Windows.UI.Xaml.Media.Imaging;
using Windows.Media;
using Devri.Interact;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

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
            Send();
            Init_First_Setting();
            Speech_Interaction();
            
            
        }

        private void Init_First_Setting()
        {
            Feeling.InitializeFeel();
        }

        private void Speech_Interaction()
        {

            VoiceAct va = new VoiceAct();
            //va.LineSelector(JArray)
            //TTS.TTSPOSTAsync();
        }

        public async void Read()
        {
            var gpio = GpioController.GetDefault();
            var dataPin = gpio.OpenPin(4);

            var buttonPin = gpio.OpenPin(9999);//임의의 값

            //well
            //pin Opens well, but fail to read. because C# can't read signal/write signal in microseconds. (40 microsecond or less needed)
            // => how about compute fix value in code?
            // => we will exhibit this in PC-8 or PC-10, there's temperature will be 25 degrees i think.
            // => Just return 25C and 40% humidty. (of course randomly do something)
            var sensor = new DHT11(dataPin, SensorTypes.DHT11);
            for (int i = 1; i < 100; i++)
            {
                var humidity = await sensor.ReadHumidity();
                var temperature = await sensor.ReadTemperature(false);
                Debug.WriteLine(temperature);
                if (buttonPin.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                    buttonPin.SetDriveMode(GpioPinDriveMode.InputPullUp);
                else
                    buttonPin.SetDriveMode(GpioPinDriveMode.Input);

                await Task.Delay(2000);

            }
        }


        public async void Send()
        {
           string result = await ServerCommunication.POSTAsync(ServerCommunication.SERVER_URL+ "/auth/signup", "pin="+ Feeling.PIN +"&code="+ Feeling.CODE);
           //TTS.TTSPOSTAsync("김준수");
        }
        public static System.Threading.Timer timer;
        public object DisPatcher { get; private set; }

        public void Timer_Start()
        {
            
            timer = new Timer(TimerCallback, null, 0,1000);

        }

        private async void TimerCallback(object state)
        {
            //Put Something you want to run async
            await Window.Current.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal,
            () => {
                // do some work on UI here;
            });
        }

        public static void Change_Image(int i,bool ani)
        {
            Image img = new Image();
            BitmapImage Devri_Common_ani = new BitmapImage(new Uri(@"Resources/devri_normal.gif"));
            BitmapImage Devri_Positive_ani = new BitmapImage(new Uri(@"Resources/devri_smile.gif"));
            BitmapImage Devri_Angry_ani = new BitmapImage(new Uri(@"Resources/devri_angry.gif"));
            BitmapImage Devri_Sad_ani = new BitmapImage(new Uri(@"Resources/devri_Confused.gif"));
            BitmapImage Devri_Common = new BitmapImage(new Uri(@"Resources/normal.png"));
            BitmapImage Devri_Positive = new BitmapImage(new Uri(@"Resources/smile.png"));
            BitmapImage Devri_Angry = new BitmapImage(new Uri(@"Resources/yan.png"));
            BitmapImage Devri_Sad = new BitmapImage(new Uri(@"Resources/confused.png"));
            switch (i)
            {
                // 0 = Normal 1=Positive 2=Angry 3=Sad 
                case 0:
                    if (ani == true)
                        img.Source = Devri_Common_ani;
                    else
                        img.Source = Devri_Common;
                    break;
                case 1:
                    if (ani == true)
                        img.Source = Devri_Positive_ani;
                    else
                        img.Source = Devri_Positive;
                    break;
                case 2:
                    if (ani == true)
                        img.Source = Devri_Angry_ani;
                    else
                        img.Source = Devri_Angry;
                    break;
                case 3:
                    if (ani == true)
                        img.Source = Devri_Sad_ani;
                    else
                        img.Source = Devri_Sad;
                    break;


            }

        }





        public static Geolocator Geolocator { get; set; }
        public static bool RunningInBackground { get; set; }
        private void SetupGeo()
        {
            if (Geolocator == null)
            {
                Geolocator = new Geolocator
                {
                    DesiredAccuracy = PositionAccuracy.High,
                    MovementThreshold = 100
                };
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
        public static async Task PlayVoice()
        {
            MediaElement media = new MediaElement();
        }





    }
}
