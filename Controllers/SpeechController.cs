﻿using CGIDigitalWeekVoiceReco.Enums;
using CGIDigitalWeekVoiceReco.Model;
using CGIDigitalWeekVoiceReco.Services;
using Microsoft.Bot.Connector.DirectLine;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Http;

namespace CGIDigitalWeekVoiceReco.Controllers
{
    public class SpeechController : ApiController
    {
        private readonly MicrosoftCognitiveSpeechService speechService = new MicrosoftCognitiveSpeechService();
        private static string directLineSecret = ConfigurationManager.AppSettings["DirectLineSecret"];
        private static string botId = ConfigurationManager.AppSettings["BotId"];
        private static string fromUser = "DirectLineSampleClientUser";
        /// <summary>
        /// POST: speech/synthesize
        /// Receive a message from a user and synthetize
        /// </summary>
        [HttpPost]
        [Route("speech/synthesize")]
        public async Task<HttpResponseMessage> GetTextToSpeech([FromBody]string text)
        {

            try
            {
                DirectLineClient client = new DirectLineClient(directLineSecret);

                var conversation = await client.Conversations.StartConversationAsync();
                Activity userMessage = new Activity
                {
                    From = new ChannelAccount(fromUser),
                    Text = text,
                    Type = ActivityTypes.Message
                };

                await client.Conversations.PostActivityAsync(conversation.ConversationId, userMessage);
                var v =  await CGIBotService.ReadBotMessagesAsync(client, conversation.ConversationId);

                var inputOptions = new InputOptions()
                {
                    // Text to be spoken
                    Text = v,
                    VoiceType = (Gender)Enum.Parse(typeof(Gender), WebConfigurationManager.AppSettings["VoiceType"]),
                    // Refer to the documentation for complete list of supported locales
                    Locale = WebConfigurationManager.AppSettings["Locale"],
                    // You can also customize the output voice. Refer to the documentation to view the different
                    // voices that the TTS service can output
                    VoiceName = WebConfigurationManager.AppSettings["VoiceName"],
                    // Service can return audio in different output format
                    OutputFormat = (AudioOutputFormat)Enum.Parse(typeof(AudioOutputFormat), WebConfigurationManager.AppSettings["AudioOutputFormat"])
                };
                var speechStream = await speechService.GetTextToSpeechAsync(v, inputOptions);
                var response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(speechStream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                return response;
            }
            catch(Exception ex)
            {
                if (ex is HttpException)
                {
                    throw ex;
                }
                else
                {
                    throw new HttpException(500, "Erreur dans le processus de conversion TTS", ex);
                }
            }
        }

        [HttpPost]
        [Route("speech/recognize")]
        public async Task<HttpResponseMessage> GetSpeechToText()
        {
            try
            {
                using (var speechStream = await Request.Content.ReadAsStreamAsync())
                {
                    var speechString = await speechService.GetTextFromAudioAsync(speechStream);
                    var response = new HttpResponseMessage(HttpStatusCode.OK);
                    response.Content = new StringContent(speechString ?? string.Empty, Encoding.UTF8, "application/json");
                    return response;
                }
            }
            catch(Exception ex)
            {
                if(ex is HttpException)
                {
                    throw ex;
                }
                else
                {
                    throw new HttpException(500, "Erreur dans le processus de conversion STT", ex);
                }
            }
            
        }
    }
}