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
        public static readonly string SERVER_URL= "http://iwin247.kr:3080";
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
            try
            {
                Windows.Web.Http.HttpClient client = new Windows.Web.Http.HttpClient();
                var body = Content;
                Windows.Web.Http.HttpStringContent theContent = new Windows.Web.Http.HttpStringContent(body, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/x-www-form-urlencoded");
                theContent.Headers["Content-Length"] = body.Length.ToString();
                Windows.Web.Http.HttpResponseMessage aResponse = await client.PostAsync(new Uri(URL), theContent);

                var responseString = aResponse.Content.ToString();//new StreamReader(aResponse.Source.).ReadToEnd();
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
            try
            {
                Windows.Web.Http.HttpClient client = new Windows.Web.Http.HttpClient();
                var body = String.Format("body string");
                body = Content;

                Windows.Web.Http.HttpStringContent theContent = new Windows.Web.Http.HttpStringContent(body, Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json");
                theContent.Headers["Content-Length"] = body.Length.ToString();
                Windows.Web.Http.HttpResponseMessage aResponse = await client.PostAsync(new Uri(URL), theContent);
            

                var responseString = aResponse.Source.ToString();
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
