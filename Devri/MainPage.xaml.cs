using Devri.Common;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Devices.Gpio;
using Windows.Devices.Sensors.Temperature;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Threading;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Media.Imaging;
using Devri.Interact;
using Windows.Media.MediaProperties;
using Windows.UI.Core;


// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x412에 나와 있습니다.

namespace Devri
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : BindablePage
    {

        //temperature
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

        //humidity
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


        //geolocation point
        private Double latitude; //위도
        private Double longitude; //경도


        //button
        private GpioPinValue pushButtonValue;
        private GpioPin pin;
        private GpioPin pushButton;

        //recorder


        public MainPage()
        {
            this.InitializeComponent();
            Read();
            Send();
            Init_First_SettingAsync();
            
            
            
        }

        private async Task Init_First_SettingAsync()
        {
            await Feeling.InitializeFeelAsync();
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

            var pushButton  = gpio.OpenPin(17);//ButtonPin

            if (pushButton.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                pushButton.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                pushButton.SetDriveMode(GpioPinDriveMode.Input);
            pushButton.DebounceTimeout = TimeSpan.FromMilliseconds(50);



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
                

                await Task.Delay(2000);

            }

            


        }

       

        public async void PushButton_EventAsync()
        {
            var devices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(Windows.Devices.Enumeration.DeviceClass.AudioCapture);
            var captureInitSettings = new Windows.Media.Capture.MediaCaptureInitializationSettings();
            captureInitSettings.StreamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.AudioAndVideo;
            var storageFile = await Windows.Storage.KnownFolders.VideosLibrary.CreateFileAsync("Voice.mp3", Windows.Storage.CreationCollisionOption.GenerateUniqueName);

            //play
            var audioCapture = new Windows.Media.Capture.MediaCapture();

            await audioCapture.InitializeAsync(captureInitSettings);
            var profile = MediaEncodingProfile.CreateM4a(Windows.Media.MediaProperties.AudioEncodingQuality.Auto);
            await audioCapture.StartRecordToStorageFileAsync(profile, storageFile);


            //stop
            await audioCapture.StopRecordAsync();

            //녹음 정지 버튼을 버튼 컨트롤에 맞게 해야함



        }
        public static async Task Play_Voice()
        {
            Windows.Storage.StorageFile voiceFile = await Windows.Storage.KnownFolders.VideosLibrary.GetFileAsync("Voice.mp3");
            var stream = await voiceFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
            if (null != stream)
            {
                //MP3player.SetSource(stream, voiceFile.ContentType);
                //MP3player.Play();

                //여기다가 메인페이지 조정할 거 해야됨
            }
        }
        //voice timer
        private System.Threading.Timer timer;
        public object DisPatcher { get; private set; }

        public void Timer_Start(int j)
        {
            TimeSpan delay = TimeSpan.FromMinutes(j);
            //j must be a millisecond integer
            timer = new System.Threading.Timer(timerCallback, null, 0, j);

        }

        private async void timerCallback(object state)
        {

            timer.Dispose();
            

        }


        


        public async void Send()
        {
           string result = await ServerCommunication.POSTAsync(ServerCommunication.SERVER_URL+ "/auth/signup", "pin="+ Feeling.PIN +"&code="+ Feeling.CODE);
           //TTS.TTSPOSTAsync("김준수");
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
        
        

       
       




    }
}
