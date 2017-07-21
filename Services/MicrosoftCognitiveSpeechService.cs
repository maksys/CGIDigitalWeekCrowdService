using CGIDigitalWeekVoiceReco.Extensions;
using CGIDigitalWeekVoiceReco.Model;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Xml.Linq;

namespace CGIDigitalWeekVoiceReco.Services
{
    public class MicrosoftCognitiveSpeechService
    {
        /// <summary>
        /// Gets text from an audio stream.
        /// </summary>
        /// <param name="audiostream">Stream to transcribe into text</param>
        /// <returns>Transcribed text. </returns>
        public async Task<string> GetTextFromAudioAsync(Stream audiostream)
        {
            var requestUri = WebConfigurationManager.AppSettings["STTRequestUri"];
            requestUri += @"?scenarios=ulm";
            requestUri += @"&appid=D4D52672-91D7-4C74-8AD8-42B1D98141A5";
            requestUri += @"&locale=" + WebConfigurationManager.AppSettings["Locale"];
            requestUri += @"&device.os=wp7";
            requestUri += @"&version=3.0";
            requestUri += @"&format=json";
            requestUri += @"&instanceid=565D69FF-E928-4B7E-87DA-9A750B96D9E3";
            requestUri += @"&requestid=" + Guid.NewGuid().ToString();

            using (var client = new HttpClient())
            {
                var token = Authentication.Instance.GetAccessToken();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                using (var binaryContent = new ByteArrayContent(StreamToBytes(audiostream)))
                {
                    binaryContent.Headers.TryAddWithoutValidation("content-type", "audio/wav; codec=\"audio/pcm\"; samplerate=16000");

                    var response = await client.PostAsync(requestUri, binaryContent);
                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        throw new HttpException((int)response.StatusCode, $"({response.StatusCode}) {response.ReasonPhrase}");
                    }

                    var responseString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    try
                    {
                        dynamic data = JsonConvert.DeserializeObject(responseString);
                        return data.header.name;
                    }
                    catch (JsonReaderException ex)
                    {
                        throw new Exception(responseString, ex);
                    }
                }
            }
        }

        /// <summary>
        /// Gets audio from text.
        /// </summary>
        /// <param name="text">Text to synthesize</param>
        /// <param name="inputOptions">Sound parameters</param>
        /// <returns>Sound synthesize. </returns>
        public async Task<Stream> GetTextToSpeechAsync(string text, InputOptions inputOptions)
        {
            string requestUri = WebConfigurationManager.AppSettings["TTSRequestUri"];
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                foreach (var header in inputOptions.Headers)
                {
                    client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
                }
                var token = Authentication.Instance.GetAccessToken();
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);

                var request = new HttpRequestMessage(HttpMethod.Post, requestUri)
                {
                    Content = new StringContent(GenerateSsml(inputOptions.Locale, inputOptions.VoiceType.GetDescription(), inputOptions.VoiceName, inputOptions.Text))
                };

                var response = await client.SendAsync(request);
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new HttpException((int)response.StatusCode, $"({response.StatusCode}) {response.ReasonPhrase}");
                }
                return await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Generates SSML.
        /// </summary>
        /// <param name="locale">The locale.</param>
        /// <param name="gender">The gender.</param>
        /// <param name="name">The voice name.</param>
        /// <param name="text">The text input.</param>
        private static string GenerateSsml(string locale, string gender, string name, string text)
        {
            var ssmlDoc = new XDocument(
                              new XElement("speak",
                                  new XAttribute("version", "1.0"),
                                  new XAttribute(XNamespace.Xml + "lang", "en-US"),
                                  new XElement("voice",
                                      new XAttribute(XNamespace.Xml + "lang", locale),
                                      new XAttribute(XNamespace.Xml + "gender", gender),
                                      new XAttribute("name", name),
                                      text)));
            return ssmlDoc.ToString();
        }

        /// <summary>
        /// Converts Stream into byte[].
        /// </summary>
        /// <param name="input">Input stream</param>
        /// <returns>Output byte[]</returns>
        private static byte[] StreamToBytes(Stream input)
        {
            using (var ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}