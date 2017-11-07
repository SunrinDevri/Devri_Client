using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.IO;

namespace Devri.Interact
{
    class VoicetoText
    {
        //private static readonly string SERVER_URL = "https://speech.googleapis.com";
        //public static string GET(String URL, String Content)
        //{
        //    var request = (HttpWebRequest)WebRequest.Create(URL + Content);
        //    try
        //    {
        //        var response = request.GetResponseAsync();
        //        var responseString = new StreamReader(response.Result.GetResponseStream()).ReadToEnd();
        //        return responseString;
        //    }
        //    catch (WebException we)
        //    {
        //        return "";
        //    }
        //}

        //public static async Task<string> POST_JSONAsync(string URL, string Content)
        //{
        //    var request = (HttpWebRequest)WebRequest.Create(URL);
        //    var postData = Content;
        //    request.ContentType = "application/json";
        //    //request.ContentLength = postData.Length;
        //    request.Method = "POST";
        //    var data = Encoding.ASCII.GetBytes(postData);
        //    try
        //    {
        //        using (var stream = await request.GetRequestStreamAsync())
        //        {
        //            stream.Write(data, 0, data.Length);
        //        }
        //        var response = request.GetResponseAsync();
        //        var responseString = new StreamReader(response.Result.GetResponseStream()).ReadToEnd();
        //        return responseString;
        //    }
        //    catch (WebException we)
        //    {
        //        //Console.WriteLine(((HttpWebResponse)we.Response).StatusCode);
        //        return "";
        //    }
        //}
    }


}
