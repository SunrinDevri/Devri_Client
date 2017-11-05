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
        private static readonly string SERVER_URL = "https://speech.googleapis.com";
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


    }

    static async Task<object> StreamingRecognizeAsync(string filePath)
    {
        var speech = SpeechClient.Create();
        var streamingCall = speech.StreamingRecognize();
        // Write the initial request with the config.
        await streamingCall.WriteAsync(
            new StreamingRecognizeRequest()
            {
                StreamingConfig = new StreamingRecognitionConfig()
                {
                    Config = new RecognitionConfig()
                    {
                        Encoding =
                        RecognitionConfig.Types.AudioEncoding.Linear16,
                        SampleRateHertz = 16000,
                        LanguageCode = "en",
                    },
                    InterimResults = true,
                }
            });
        // Print responses as they arrive.
        Task printResponses = Task.Run(async () =>
        {
            while (await streamingCall.ResponseStream.MoveNext(
                default(CancellationToken)))
            {
                foreach (var result in streamingCall.ResponseStream
                    .Current.Results)
                {
                    foreach (var alternative in result.Alternatives)
                    {
                        Console.WriteLine(alternative.Transcript);
                    }
                }
            }
        });
        // Stream the file content to the API.  Write 2 32kb chunks per 
        // second.
        using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
        {
            var buffer = new byte[32 * 1024];
            int bytesRead;
            while ((bytesRead = await fileStream.ReadAsync(
                buffer, 0, buffer.Length)) > 0)
            {
                await streamingCall.WriteAsync(
                    new StreamingRecognizeRequest()
                    {
                        AudioContent = Google.Protobuf.ByteString
                        .CopyFrom(buffer, 0, bytesRead),
                    });
                await Task.Delay(500);
            };
        }
        await streamingCall.WriteCompleteAsync();
        await printResponses;
        return 0;
    }
}
