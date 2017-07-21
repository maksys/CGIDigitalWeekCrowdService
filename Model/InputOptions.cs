using CGIDigitalWeekVoiceReco.Enums;
using System;
using System.Collections.Generic;

namespace CGIDigitalWeekVoiceReco.Model
{
    /// <summary>
    /// Inputs Options for the TTS Service.
    /// </summary>
    public class InputOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Input"/> class.
        /// </summary>
        public InputOptions()
        {
            Locale = "fr-FR";
            VoiceName = "Microsoft Server Speech Text to Speech Voice (fr-FR, Julie, Apollo)";
            // Default to Riff16Khz16BitMonoPcm output format.
            OutputFormat = AudioOutputFormat.Riff16Khz16BitMonoPcm;
        }

        /// <summary>
        /// Gets or sets the audio output format.
        /// </summary>
        public AudioOutputFormat OutputFormat { get; set; }

        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> Headers
        {
            get
            {
                List<KeyValuePair<string, string>> toReturn = new List<KeyValuePair<string, string>>();
                toReturn.Add(new KeyValuePair<string, string>("Content-Type", "application/ssml+xml"));

                string outputFormat;

                switch (this.OutputFormat)
                {
                    case AudioOutputFormat.Raw16Khz16BitMonoPcm:
                        outputFormat = "raw-16khz-16bit-mono-pcm";
                        break;

                    case AudioOutputFormat.Raw8Khz8BitMonoMULaw:
                        outputFormat = "raw-8khz-8bit-mono-mulaw";
                        break;

                    case AudioOutputFormat.Riff16Khz16BitMonoPcm:
                        outputFormat = "riff-16khz-16bit-mono-pcm";
                        break;

                    case AudioOutputFormat.Riff8Khz8BitMonoMULaw:
                        outputFormat = "riff-8khz-8bit-mono-mulaw";
                        break;

                    case AudioOutputFormat.Ssml16Khz16BitMonoSilk:
                        outputFormat = "ssml-16khz-16bit-mono-silk";
                        break;

                    case AudioOutputFormat.Raw16Khz16BitMonoTrueSilk:
                        outputFormat = "raw-16khz-16bit-mono-truesilk";
                        break;

                    case AudioOutputFormat.Ssml16Khz16BitMonoTts:
                        outputFormat = "ssml-16khz-16bit-mono-tts";
                        break;

                    case AudioOutputFormat.Audio16Khz128KBitRateMonoMp3:
                        outputFormat = "audio-16khz-128kbitrate-mono-mp3";
                        break;

                    case AudioOutputFormat.Audio16Khz64KBitRateMonoMp3:
                        outputFormat = "audio-16khz-64kbitrate-mono-mp3";
                        break;

                    case AudioOutputFormat.Audio16Khz32KBitRateMonoMp3:
                        outputFormat = "audio-16khz-32kbitrate-mono-mp3";
                        break;

                    case AudioOutputFormat.Audio16Khz16KbpsMonoSiren:
                        outputFormat = "audio-16khz-16kbps-mono-siren";
                        break;

                    case AudioOutputFormat.Riff16Khz16KbpsMonoSiren:
                        outputFormat = "riff-16khz-16kbps-mono-siren";
                        break;

                    default:
                        outputFormat = "riff-16khz-16bit-mono-pcm";
                        break;
                }

                toReturn.Add(new KeyValuePair<string, string>("X-Microsoft-OutputFormat", outputFormat));
                // Refer to the doc
                toReturn.Add(new KeyValuePair<string, string>("X-Search-AppId", "07D3234E49CE426DAA29772419F436CA"));
                // Refer to the doc
                toReturn.Add(new KeyValuePair<string, string>("X-Search-ClientID", "1ECFAE91408841A480F00935DC390960"));
                // The software originating the request
                toReturn.Add(new KeyValuePair<string, string>("User-Agent", "CGIDigitalWeekVoiceReco"));

                return toReturn;
            }
            set
            {
                Headers = value;
            }
        }

        /// <summary>
        /// Gets or sets the locale.
        /// </summary>
        public String Locale { get; set; }

        /// <summary>
        /// Gets or sets the type of the voice; male/female.
        /// </summary>
        public Gender VoiceType { get; set; }

        /// <summary>
        /// Gets or sets the name of the voice.
        /// </summary>
        public string VoiceName { get; set; }

        /// <summary>
        /// Gets or sets the text.
        /// </summary>
        public string Text { get; set; }
    }
}