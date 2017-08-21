using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace TestVoiceRecoService
{
    internal class Program
    {
        private const string filename = @"C:\temp\sound.wav";

        private static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                using (var httpClient = new HttpClient())
                {
                    using (var client = new HttpClient())
                    {
                        //client.BaseAddress = new Uri("http://localhost:3979");
                        client.BaseAddress = new Uri("http://cgidigitalweekvoiceservice.azurewebsites.net");
                        var content = new FormUrlEncodedContent(new[]
                        {
                            new KeyValuePair<string, string>("", "Bonjour mon nom est bond james bond. Je suis un agent secret mon nom de code est 007")
                        });
                        var response = await client.PostAsync("/speech/synthesize", content);
                        response.EnsureSuccessStatusCode();
                        using (var stream = await response.Content.ReadAsStreamAsync())
                        using (var fileStream = File.Create(filename))
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                            stream.CopyTo(fileStream);
                        }

                        using (var stream = File.OpenRead(filename))
                        {
                            stream.Seek(0, SeekOrigin.Begin);
                            response = await client.PostAsync("/speech/recognize", new StreamContent(stream));
                            response.EnsureSuccessStatusCode();
                            var speechText = await response.Content.ReadAsStringAsync();
                        }
                    }
                }
            }).GetAwaiter().GetResult();
        }
    }
}