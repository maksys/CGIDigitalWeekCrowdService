<<<<<<< HEAD
﻿# Speech To Text and Text To Speech Web API


### Prerequisites

The minimum prerequisites to run this sample are:
* The latest update of Visual Studio 2015. You can download the community version [here](http://www.visualstudio.com) for free.
* The Bot Framework Emulator. To install the Bot Framework Emulator, download it from [here](https://emulator.botframework.com/). Please refer to [this documentation article](https://github.com/microsoft/botframework-emulator/wiki/Getting-Started) to know more about the Bot Framework Emulator.
* **[Recommended]** Visual Studio Code for IntelliSense and debugging, download it from [here](https://code.visualstudio.com/) for free.

````XML
  <appSettings>
    <add key="MicrosoftSpeechApiKey" value="PUT-YOUR-OWN-API-KEY-HERE" />
	<add key="AccessUriToken" value="https://api.cognitive.microsoft.com/sts/v1.0/issueToken" />
	<add key="VoiceName" value="Microsoft Server Speech Text to Speech Voice (fr-FR, HortenseRUS)" />
    <add key="VoiceType" value="Male" />
    <add key="Locale" value="fr-FR" />
	<add key="AudioOutputFormat" value="Audio16Khz128KBitRateMonoMp3" />
    <add key="STTRequestUri" value="https://speech.platform.bing.com/recognize/query" />
    <add key="TTSRequestUri" value="https://speech.platform.bing.com/synthesize" />
  </appSettings>
````

### Usage

Attach an audio file (wav format) or send a text to receive sound (wav format).

### Code Highlights

Microsoft Cognitive Services provides a Speech Recognition API to convert audio into text. Check out [Bing Speech API](https://www.microsoft.com/cognitive-services/en-us/speech-api) for a complete reference of Speech APIs available. In this sample we are using the Speech Recognition API using the [REST API](https://www.microsoft.com/cognitive-services/en-us/Speech-api/documentation/API-Reference-REST/BingVoiceRecognition).

In this sample we are using the API to get the text and send it back to the user.
````C#
using (var client = new HttpClient())
{
	//TTS
	client.BaseAddress = new Uri("http://localhost:3979");
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

	//STT
	using (var stream = File.OpenRead(filename))
	{
		stream.Seek(0, SeekOrigin.Begin);
		response = await client.PostAsync("/speech/recognize", new StreamContent(stream));
		response.EnsureSuccessStatusCode();
		var speechText = await response.Content.ReadAsStringAsync();
	}
}
````


