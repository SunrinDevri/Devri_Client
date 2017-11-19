﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Windows.Storage.Streams;
using System.Net.Http;
using Windows.Storage;

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
        public static async void TTSPOSTAsync( string Content)
        {
            try
            {
                string RealContent = "speaker=mijin&speed=0&text="+Content;
                string url = "https://openapi.naver.com/v1/voice/tts.bin";
                Windows.Web.Http.HttpClient client = new Windows.Web.Http.HttpClient();
                var body = RealContent;
                Windows.Web.Http.HttpStringContent theContent = new Windows.Web.Http.HttpStringContent(body, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded");

                theContent.Headers["Content-Length"] = body.Length.ToString();
                theContent.Headers["X-Naver-Client-Id"] = "VLwfoTWl4e8GQ5_9JU35";
                theContent.Headers["X-Naver-Client-Secret"] = "fzoBsRF0_M";
                Windows.Web.Http.HttpResponseMessage aResponse = await client.PostAsync(new Uri(url), theContent);


                if (aResponse.StatusCode == Windows.Web.Http.HttpStatusCode.Ok && aResponse != null)
                {
                    using (InMemoryRandomAccessStream stream1 = new InMemoryRandomAccessStream())
                    {
                        await aResponse.Content.WriteToStreamAsync(stream1);
                        FileStream fs = new FileStream("voice.mp3", FileMode.Append);
                        StorageFile file1 = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("voice.mp3", CreationCollisionOption.ReplaceExisting);
                        //                      내가 수정해야 할 부분
                        using (var fileStream1 = await file1.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            await RandomAccessStream.CopyAndCloseAsync(stream1.GetInputStreamAt(0), fileStream1.GetOutputStreamAt(0));
                        }
                    }
                }

                await MainPage.Play_Voice();

            }
            catch (WebException we)
            {
            }
        }
        
    }
}
