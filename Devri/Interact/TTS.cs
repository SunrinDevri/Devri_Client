using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Windows.Storage.Streams;
using System.Net.Http;
using Windows.Storage;
using System.Net.Http.Headers;

namespace Devri.Interact
{
    class TTS
    {
        //public static void TextToSound(string line)
        //{
        //    string text = "좋은 하루 되세요."; // 음성합성할 문자값
        //    string url = "https://openapi.naver.com/v1/voice/tts.bin";
        //    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        //    request.Headers.Add("X-Naver-Client-Id", "YOUR-CLIENT-ID");
        //    request.Headers.Add("X-Naver-Client-Secret", "YOUR-CLIENT-SECRET");
        //    request.Method = "POST";
        //    byte[] byteDataParams = Encoding.UTF8.GetBytes("speaker=mijin&speed=0&text=" + text);
        //    request.ContentType = "application/x-www-form-urlencoded";
        //    request.ContentLength = byteDataParams.Length;
        //    Stream st = request.GetRequestStream();
        //    st.Write(byteDataParams, 0, byteDataParams.Length);
        //    st.Close();
        //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        //    string status = response.StatusCode.ToString();
        //    Console.WriteLine("status=" + status);
        //    using (Stream output = File.OpenWrite("c:/tts.mp3"))
        //    using (Stream input = response.GetResponseStream())
        //
        //    {
        //        input.CopyTo(output);
        //    }
        //    Console.WriteLine("c:/tts.mp3 was created");
        //}
        public static async Task<string> TTSPOSTAsync(string Content)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(@"https://openapi.naver.com/v1/voice/tts.bin");
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/x-www-form-urlencoded"));
                httpClient.DefaultRequestHeaders.AcceptEncoding.Add(new StringWithQualityHeaderValue("utf-8"));
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Naver-Client-Id", "VLwfoTWl4e8GQ5_9JU35");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Naver-Client-Secret", "fzoBsRF0_M");
                string endpoint = @"";

                try
                {
                    byte[] bytes = Encoding.UTF8.GetBytes("speaker=mijin&speed=0&text=" + Content);
                    HttpContent content = new StringContent("speaker=mijin&speed=0&text=" + Content, Encoding.UTF8, "application/x-www-form-urlencoded");
                    HttpContent byteContent = new ByteArrayContent(bytes);
                    HttpResponseMessage response = await httpClient.PostAsync(endpoint, content);

                    if (response.IsSuccessStatusCode)
                    {
                        string Response = await response.Content.ReadAsStringAsync();
                        //do something with json response here
                        using (InMemoryRandomAccessStream stream1 = new InMemoryRandomAccessStream())
                        {
                            var stream = await response.Content.ReadAsStreamAsync();
                          
                            StorageFile file1 = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("Voice.mp3", CreationCollisionOption.ReplaceExisting);
                            //                      내가 수정해야 할 부분
                            using (var fileStream1 = await file1.OpenAsync(FileAccessMode.ReadWrite))
                            {
                                await RandomAccessStream.CopyAndCloseAsync(stream.AsInputStream(), fileStream1.GetOutputStreamAt(0));
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    //Could not connect to server
                    //Use more specific exception handling, this is just an example
                    Console.WriteLine(e.Message);
                }
            }

            
            return "";


        }

       



    }

}

