using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Windows.Storage.Streams;

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
            try
            {
                string url = "https://openapi.naver.com/v1/voice/tts.bin";
                Windows.Web.Http.HttpClient client = new Windows.Web.Http.HttpClient();
                var body = String.Format("body string");
                
                Windows.Web.Http.HttpStringContent theContent = new Windows.Web.Http.HttpStringContent(body, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded");
                theContent.Headers["Content-Length"] = "length";
                theContent.Headers["X-Naver-Client-Id"] = "VLwfoTWl4e8GQ5_9JU35";
                theContent.Headers["X-Naver-Client-Secret"] = "fzoBsRF0_M";
                Windows.Web.Http.HttpResponseMessage aResponse = await client.PostAsync(new Uri(url), theContent);
                

                var responseString = aResponse.Source.ToString();
                
                FileOutputStream outputStream = new FileOutputStream();//File.OpenWrite("voice.mp3")

                await aResponse.Content.WriteToStreamAsync(outputStream);

                //new StreamReader(aResponse.Source.).ReadToEnd();
                return responseString;
            }
            catch (WebException we)
            {
                //Console.WriteLine(((HttpWebResponse)we.Response).StatusCode);
                return "";
            }
        }
    }
}
