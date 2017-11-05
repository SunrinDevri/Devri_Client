using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.IO;

namespace Devri.Interact
{
    class ServerCommunication

    {
        private static readonly string SERVER_URL= "http://iwin247.kr";
        public static string GET(String URL, String Content)
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

        public static string POST_FILE(string URL, string Filename)
        {
            try
            {
                FileStream stream = File.Open(Filename, FileMode.Open);
                StreamContent scontent = new StreamContent(stream);
                scontent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
                {
                    FileName = Filename,
                    Name = "file"
                };
                scontent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/png");
                var client = new HttpClient();
                var multi = new MultipartFormDataContent();
                multi.Add(scontent);
                client.BaseAddress = new Uri("http://iwin247.kr");
                var result = client.PostAsync("upload/", multi).Result;
                return result.Content.ReadAsStringAsync().Result;
            }
            catch (Exception)
            {
                return "error";
            }

        }
        public static async Task<string> POSTAsync(string URL, string Content)
        {
            var request = (HttpWebRequest)WebRequest.Create(URL);
            var postData = Content;
            request.ContentType = "application/x-www-form-urlencoded";
            //request.ContentLength = postData.Length;
            request.Method = "POST";
            var data = Encoding.ASCII.GetBytes(postData);
            try
            {
                using (var stream = await request.GetRequestStreamAsync())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = request.GetResponseAsync();
                var responseString = new StreamReader(response.Result.GetResponseStream()).ReadToEnd();
                return responseString;
            }
            catch (WebException we)
            {
                //Console.WriteLine(((HttpWebResponse)we.Response).StatusCode);
                return "";
            }
        }

        public static async Task<string> POST_JSONAsync(string URL, string Content)
        {
            var request = (HttpWebRequest)WebRequest.Create(URL);
            var postData = Content;
            request.ContentType = "application/json";
            //request.ContentLength = postData.Length;
            request.Method = "POST";
            var data = Encoding.ASCII.GetBytes(postData);
            try
            {
                using (var stream = await request.GetRequestStreamAsync())
                {
                    stream.Write(data, 0, data.Length);
                }
                var response = request.GetResponseAsync();
                var responseString = new StreamReader(response.Result.GetResponseStream()).ReadToEnd();
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
