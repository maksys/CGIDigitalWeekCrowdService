namespace CGIDigitalWeekVoiceReco.Services
{
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Web;
    using System.Web.Configuration;

    public sealed class Authentication
    {
        // The token has an expiry time of 10 minutes https://www.microsoft.com/cognitive-services/en-us/Speech-api/documentation/API-Reference-REST/BingVoiceRecognition
        private const int TokenExpiryInSeconds = 600;

        private static readonly object LockObject;
        private static readonly string AccessUriToken;
        private static readonly string ApiKey;
        private string token;
        private Timer timer;

        static Authentication()
        {
            LockObject = new object();
            ApiKey = WebConfigurationManager.AppSettings["MicrosoftSpeechApiKey"];
            AccessUriToken = WebConfigurationManager.AppSettings["AccessUriToken"]; 
        }

        private Authentication()
        {
        }

        public static Authentication Instance { get; } = new Authentication();

        /// <summary>
        /// Gets the current access token.
        /// </summary>
        /// <returns>Current access token</returns>
        public string GetAccessToken()
        {
            // Token will be null first time the function is called.
            if (token == null)
            {
                lock (LockObject)
                {
                    // This condition will be true only once in the lifetime of the application
                    if (token == null)
                    {
                        RefreshToken();
                    }
                }
            }

            return token;
        }

        /// <summary>
        /// Issues a new AccessToken from the Speech Api
        /// </summary>
        /// This method couldn't be async because we are calling it inside of a lock.
        /// <returns>AccessToken</returns>
        private static string GetNewToken()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", ApiKey);

                var response = client.PostAsync(AccessUriToken, null).Result;
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new HttpException((int)response.StatusCode, $"({response.StatusCode}) {response.ReasonPhrase}");
                }

                return response.Content.ReadAsStringAsync().Result;
            }
        }

        /// <summary>
        /// Refreshes the current token before it expires. This method will refresh the current access token.
        /// It will also schedule itself to run again before the newly acquired token's expiry by one minute.
        /// </summary>
        private void RefreshToken()
        {
            token = GetNewToken();
            timer?.Dispose();
            timer = new Timer(
                x => this.RefreshToken(), 
                null, 
                TimeSpan.FromSeconds(TokenExpiryInSeconds).Subtract(TimeSpan.FromMinutes(1)), // Specifies the delay before RefreshToken is invoked.
                TimeSpan.FromMilliseconds(-1)); // Indicates that this function will only run once
        }
    }
}