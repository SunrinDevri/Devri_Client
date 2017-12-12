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
using Windows.Devices.Enumeration;
using Windows.Storage;
using System.Net;
using System.Text;
using System.IO;


// 빈 페이지 항목 템플릿에 대한 설명은 https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x412에 나와 있습니다.

namespace Devri
{
    /// <summary>
    /// 자체적으로 사용하거나 프레임 내에서 탐색할 수 있는 빈 페이지입니다.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        //temperature
        private DeviceInformationCollection devices;
        //private Windows.Media.Capture.MediaCaptureInitializeSettings
        private Windows.Media.Capture.MediaCapture capture;
        private Windows.Media.Capture.LowLagMediaRecording recording;


        //geolocation point
        private Double latitude; //위도
        private Double longitude; //경도


        //button
        private GpioPinValue pushButtonValue;
        private GpioPin pin;
        private GpioPin pushButton;

        //recorder

        private static readonly string API_KEY = "AIzaSyCMPY7lNMZVSGPK9JE9_O1BGsnTtB4t3TA";

        public static async Task<string> STTAsync(string Content)
        {
            try
            {
                Windows.Web.Http.HttpClient client = new Windows.Web.Http.HttpClient();
                var body = Content;
                Windows.Web.Http.HttpStringContent theContent = new Windows.Web.Http.HttpStringContent(body, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");
                theContent.Headers["Content-Length"] = body.Length.ToString();
                Windows.Web.Http.HttpResponseMessage aResponse = await client.PostAsync(new Uri("https://speech.googleapis.com/v1/speech:recognize?KEY="+API_KEY), theContent);

                var responseString = aResponse.Content.ToString();//new StreamReader(aResponse.Source.).ReadToEnd();
                return responseString;
            }
            catch (WebException we)
            {
                //Console.WriteLine(((HttpWebResponse)we.Response).StatusCode);
                return "";
            }
        }

        public static string Build_Content(string base64_String)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"{ ""config"" : { { ""encoding"" : {FLAC}, ""sampleRateHertz"" : 16000, ""languageCode"" : ""ko-KR"" }, ""audio"": { {" + base64_String +"} }, }");
            return sb.ToString();
        }

        public MainPage()
        {
            this.InitializeComponent();
            ReadAsync();
            Send();
            //Init_First_SettingAsync();



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
        public async Task RecordInitializer()
        {
            devices = await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync(Windows.Devices.Enumeration.DeviceClass.AudioCapture);
            var captureInitSettings = new Windows.Media.Capture.MediaCaptureInitializationSettings
            {
                StreamingCaptureMode = Windows.Media.Capture.StreamingCaptureMode.Audio
            };

            capture = new Windows.Media.Capture.MediaCapture();
            await capture.InitializeAsync(captureInitSettings);
            var profile = MediaEncodingProfile.CreateFlac(AudioEncodingQuality.Auto);
            try
            {
                var storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("Voice.flac", Windows.Storage.CreationCollisionOption.ReplaceExisting);
                recording = await capture.PrepareLowLagRecordToStorageFileAsync(MediaEncodingProfile.CreateFlac(AudioEncodingQuality.High), storageFile);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        public async void ReadAsync()
        {
            var gpio = GpioController.GetDefault();
            var dataPin = gpio.OpenPin(4);

            var pushButton = gpio.OpenPin(5);//ButtonPin

            if (pushButton.IsDriveModeSupported(GpioPinDriveMode.InputPullUp))
                pushButton.SetDriveMode(GpioPinDriveMode.InputPullUp);
            else
                pushButton.SetDriveMode(GpioPinDriveMode.Input);

            pushButton.DebounceTimeout = TimeSpan.FromMilliseconds(50);
            pushButton.ValueChanged += buttonPin_ValueChangedAsync;
            //var gpio = GpioController.GetDefault();
            pin = gpio.OpenPin(6);
            pin.SetDriveMode(GpioPinDriveMode.Output);
            pin.Write(pushButtonValue = GpioPinValue.Low);
            //await RecordInitializer();



            // await capture.StartRecordToStorageFileAsync(profile, storageFile);


            //well
            //pin Opens well, but fail to read. because C# can't read signal/write signal in microseconds. (40 microsecond or less needed)
            // => how about compute fix value in code?
            // => we will exhibit this in PC-8 or PC-10, there's temperature will be 25 degrees i think.
            // => Just return 25C and 40% humidty. (of course randomly do something)
            //  var sensor = new DHT11(dataPin, SensorTypes.DHT11);
            //  for (int i = 1; i < 100; i++)
            //  {
            //      var humidity = await sensor.ReadHumidity();
            //      var temperature = await sensor.ReadTemperature(false);
            //      Debug.WriteLine(temperature);


            //      await Task.Delay(2000);

            //  }




        }

        private async void buttonPin_ValueChangedAsync(GpioPin sender, GpioPinValueChangedEventArgs e)
        {
            // toggle the state of the LED every time the button is pressed
            if (e.Edge == GpioPinEdge.FallingEdge)
            {
                //ledPinValue = (ledPinValue == GpioPinValue.Low) ?
                //    GpioPinValue.High : GpioPinValue.Low;
                //ledPin.Write(ledPinValue);
                if (pin != null)
                {
                    if (pushButtonValue == GpioPinValue.Low)
                    {
                        try
                        {
                            await RecordInitializer();
                            await recording.StartAsync();
                            pin.Write(pushButtonValue = GpioPinValue.High);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                    else
                    {
                        pin.Write(pushButtonValue = GpioPinValue.Low);
                        try
                        {
                            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                            {
                                await recording.StopAsync();
                                await recording.FinishAsync();
                                
                                MediaElement me = new MediaElement();

                                var Folder = ApplicationData.Current.LocalFolder;
                                var File = await Folder.GetFileAsync("Voice.flac");
                                var stream = await File.OpenAsync(FileAccessMode.Read);
                                var encoding = System.Text.Encoding.UTF8;
                                string response = await STTAsync(Build_Content(Convert.ToBase64String(encoding.GetBytes(stream.ToString()))));
                                Console.WriteLine(response);
                                    if (null != stream)
                                    {
                                        me.SetSource(stream, File.ContentType);
                                        me.Play();
                                    }
                            });
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }

            }

        }


        public async void PushButton_EventAsync()
        {


            //play





            //stop


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
            // string result = await ServerCommunication.POSTAsync(ServerCommunication.SERVER_URL+ "/auth/signup", "pin="+ Feeling.PIN +"&code="+ Feeling.CODE);
            //TTS.TTSPOSTAsync("김준수");
        }

        public static void Change_Image(int i, bool ani)
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
