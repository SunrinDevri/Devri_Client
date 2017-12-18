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
using System.Net.Http;
using NAudio.Wave;
using Windows.Media.Audio;
using Windows.Media.Render;
using Windows.Media.Devices;
using Windows.Media.Capture;
using Windows.Storage.Pickers;
using System.Collections.Generic;


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
        // private static MediaElement PlayMusic = new MediaElement();
        static MediaElement player = new MediaElement();
        public static async Task<string> STTAsync(string Content)
        {
            try
            {
                Windows.Web.Http.HttpClient client = new Windows.Web.Http.HttpClient();
                var body = Content;
                var Folder = ApplicationData.Current.LocalFolder;
                var File = await Folder.GetFileAsync("Voice.wav");
                var stream = await File.OpenAsync(FileAccessMode.Read);
                //Windows.Web.Http.HttpStreamContent content = new Windows.Web.Http.HttpStreamContent(stream);
                Windows.Web.Http.HttpStringContent theContent = new Windows.Web.Http.HttpStringContent(body, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");
                theContent.Headers["Content-Length"] = Content.Length.ToString();
                //theContent.Headers.Append("Authorization","Bearer 60e04b73ba24193960baede7472e2ad2c99cd3c0");
                Windows.Web.Http.HttpResponseMessage aResponse = await client.GetAsync(new Uri("http://iwin247.kr/sounds"));

                var responseString = aResponse.Content.ToString();//new StreamReader(aResponse.Source.).ReadToEnd();
                return responseString;
            }
            catch (Exception e)
            {
                //Console.WriteLine(((HttpWebResponse)we.Response).StatusCode);
                Console.WriteLine(e.Message);
                return "";
            }
        }





        public static string Build_Content(string base64_String)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(base64_String);
            return sb.ToString();
        }

        public MainPage()
        {
            this.InitializeComponent();
            ReadAsync();
            Change_Image(3, false);
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
            var profile = MediaEncodingProfile.CreateWav(AudioEncodingQuality.Auto);
            try
            {
                var storageFile = await ApplicationData.Current.LocalFolder.CreateFileAsync("Voice.wav", Windows.Storage.CreationCollisionOption.ReplaceExisting);
                recording = await capture.PrepareLowLagRecordToStorageFileAsync(MediaEncodingProfile.CreateWav(AudioEncodingQuality.High), storageFile);
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
                                var File = await Folder.GetFileAsync("Voice.wav");



                                if (File != null)
                                {
                                    var result = await AudioGraph.CreateAsync(
                                           new AudioGraphSettings(AudioRenderCategory.Speech));

                                    if (result.Status == AudioGraphCreationStatus.Success)
                                    {
                                        this.graph = result.Graph;

                                        var microphone = await DeviceInformation.CreateFromIdAsync(
                                          MediaDevice.GetDefaultAudioCaptureId(AudioDeviceRole.Default));

                                        // In my scenario I want 16K sampled, mono, 16-bit output
                                        var outProfile = MediaEncodingProfile.CreateWav(AudioEncodingQuality.Low);
                                        outProfile.Audio = AudioEncodingProperties.CreatePcm(8000, 1, 16);

                                        var outputResult = await this.graph.CreateFileOutputNodeAsync(File,
                                          outProfile);

                                        if (outputResult.Status == AudioFileNodeCreationStatus.Success)
                                        {
                                            this.outputNode = outputResult.FileOutputNode;

                                            var inProfile = MediaEncodingProfile.CreateWav(AudioEncodingQuality.High);

                                            var inputResult = await this.graph.CreateDeviceInputNodeAsync(
                                              MediaCategory.Speech,
                                              inProfile.Audio,
                                              microphone);

                                            if (inputResult.Status == AudioDeviceNodeCreationStatus.Success)
                                            {
                                                inputResult.DeviceInputNode.AddOutgoingConnection(
                                                  this.outputNode);

                                                this.graph.Start();
                                            }
                                        }
                                    }
                                }





                                var stream = await File.OpenAsync(FileAccessMode.Read);
                                var encoding = System.Text.Encoding.UTF8;

                                //  string response = GET_String("http://iwin247.kr/sound/base64=",Convert.ToBase64String(encoding.GetBytes(stream.ToString())));


                                
                                string response = ServerCommunication.POST_FILE("http://iwin247.info:3080/sounds", File.Path);//Maybe It works ??

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


        public string GET_String(String URL, String Content)
        {
            var request = (HttpWebRequest)WebRequest.Create(URL + Content);
            try
            {
                var response = request.GetResponseAsync();
                var responseString = new StreamReader(response.Result.GetResponseStream()).ReadToEnd();
                return responseString;
            }
            catch (WebException we)
            {
                return "";
            }
        }


        public async Task Play_Voice()
        {
            Windows.Storage.StorageFile voiceFile = await ApplicationData.Current.LocalFolder.GetFileAsync("Voice.mp3");
            // var stream = await voiceFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
            if (null != voiceFile)
            {
                try
                {
                    player.SetSource(await voiceFile.OpenAsync(FileAccessMode.Read), voiceFile.ContentType);
                    player.Volume = 100.0f;


                    player.Play();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
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



        async void OnStart(object sender, RoutedEventArgs e)
        {
            var file = await this.PickFileAsync();

            if (file != null)
            {
                var result = await AudioGraph.CreateAsync(
                       new AudioGraphSettings(AudioRenderCategory.Speech));

                if (result.Status == AudioGraphCreationStatus.Success)
                {
                    this.graph = result.Graph;

                    var microphone = await DeviceInformation.CreateFromIdAsync(
                      MediaDevice.GetDefaultAudioCaptureId(AudioDeviceRole.Default));

                    // In my scenario I want 16K sampled, mono, 16-bit output
                    var outProfile = MediaEncodingProfile.CreateWav(AudioEncodingQuality.Low);
                    outProfile.Audio = AudioEncodingProperties.CreatePcm(8000, 1, 16);

                    var outputResult = await this.graph.CreateFileOutputNodeAsync(file,
                      outProfile);

                    if (outputResult.Status == AudioFileNodeCreationStatus.Success)
                    {
                        this.outputNode = outputResult.FileOutputNode;

                        var inProfile = MediaEncodingProfile.CreateWav(AudioEncodingQuality.High);

                        var inputResult = await this.graph.CreateDeviceInputNodeAsync(
                          MediaCategory.Speech,
                          inProfile.Audio,
                          microphone);

                        if (inputResult.Status == AudioDeviceNodeCreationStatus.Success)
                        {
                            inputResult.DeviceInputNode.AddOutgoingConnection(
                              this.outputNode);

                            this.graph.Start();
                        }
                    }
                }
            }
        }


        async void OnStop(object sender, RoutedEventArgs e)
        {
            if (this.graph != null)
            {
                this.graph?.Stop();

                await this.outputNode.FinalizeAsync();

                // assuming that disposing the graph gets rid of the input/output nodes?
                this.graph?.Dispose();

                this.graph = null;
            }
        }
        async Task<StorageFile> PickFileAsync()
        {
            FileSavePicker picker = new FileSavePicker();
            picker.FileTypeChoices.Add("Wave File (PCM)", new List<string> { ".wav" });
            picker.SuggestedStartLocation = PickerLocationId.Desktop;

            var file = await picker.PickSaveFileAsync();

            return (file);
        }
        AudioGraph graph;
        AudioFileOutputNode outputNode;






        public async void Send(string line)
        {

            string result = await TTS.TTSPOSTAsync(line);
            await Play_Voice();
        }

        public static void Change_Image(int i, bool ani)
        {
            Image img = new Image();
            BitmapImage Devri_Common_ani = new BitmapImage(new Uri("ms-appx:///Resources/devri_normal.gif", UriKind.Absolute));//여기 고쳐야함
            BitmapImage Devri_Positive_ani = new BitmapImage(new Uri("ms-appx:///Resources/devri_smile.gif", UriKind.Absolute));
            BitmapImage Devri_Angry_ani = new BitmapImage(new Uri("ms-appx:///Resources/devri_angry.gif", UriKind.Absolute));
            BitmapImage Devri_Sad_ani = new BitmapImage(new Uri("ms-appx:///Resources/devri_Confused.gif", UriKind.Absolute));
            BitmapImage Devri_Common = new BitmapImage(new Uri("ms-appx:///Resources/normal.png", UriKind.Absolute));
            BitmapImage Devri_Positive = new BitmapImage(new Uri("ms-appx:///Resources/smile.png", UriKind.Absolute));
            BitmapImage Devri_Angry = new BitmapImage(new Uri("ms-appx:///Resources/yan.png", UriKind.Absolute));
            BitmapImage Devri_Sad = new BitmapImage(new Uri("ms-appx:///Resources/confused.png", UriKind.Absolute));
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
