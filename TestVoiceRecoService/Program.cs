using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace TestVoiceRecoService
{
    internal class Program
    {
        private const string filename = @"C:\temp\sound{0}.wav";


        private static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                using (var httpClient = new HttpClient())
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("http://localhost:3979");
                        //client.BaseAddress = new Uri("http://cgidigitalweekvoiceservice.azurewebsites.net");

                        Console.Write("Command> ");
                        string workingtext;
                        while (true)
                        {
                            workingtext = Console.ReadLine().Trim();
                            var content = new FormUrlEncodedContent(new[]
                            {
                            new KeyValuePair<string, string>("", workingtext)
                        });
                            var response = await client.PostAsync("/speech/synthesize", content);
                            response.EnsureSuccessStatusCode();
                            using (var stream = await response.Content.ReadAsStreamAsync())
                            using (var fileStream = File.Create(FilaNameGeneration()))
                            {
                                stream.Seek(0, SeekOrigin.Begin);
                                stream.CopyTo(fileStream);
                            }

                            //Thread.Sleep(2000);
                            //SoundPlayer s = new SoundPlayer(filename);
                            //s.Play();
                        }
                        //using (var stream = File.OpenRead(filename))
                        //{
                        //    stream.Seek(0, SeekOrigin.Begin);
                        //    response = await client.PostAsync("/speech/recognize", new StreamContent(stream));
                        //    response.EnsureSuccessStatusCode();
                        //    var speechText = await response.Content.ReadAsStringAsync();
                        //}
                    }
                }
            }).GetAwaiter().GetResult();
        }

        private static string FilaNameGeneration()
        {

            return string.Format(filename, DateTime.Now.Day.ToString() + DateTime.Now.Hour.ToString() + DateTime.Now.Minute.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString());
        }
    }
}