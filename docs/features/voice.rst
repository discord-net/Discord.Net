Voice
=====

Setup
-----

To use audio, you must install the AudioService to your DiscordClient.

.. code-block:: csharp6
	
	var _client = new DiscordClient();

	_client.UsingAudio(x => // Opens an AudioConfigBuilder so we can configure our AudioService
	{
		x.Mode == AudioMode.Outgoing; // Tells the AudioService that we will only be sending audio
	});

Joining a Channel
-----------------

Joining Voice Channels is pretty straight-forward, and is required to send Audio. This will also allow us to get an IAudioClient, which we will later use to send Audio.

.. code-block:: csharp6
	
	var voiceChannel = _client.FindServers("Music Bot Server").FirstOrDefault().VoiceChannels.FirstOrDefault(); // Finds the first VoiceChannel on the server 'Music Bot Server'

	var _vClient = await _client.GetService<AudioService>() // We use GetService to find the AudioService that we installed earlier. In previous versions, this was equivelent to _client.Audio()
		.Join(VoiceChannel); // Join the Voice Channel, and return the IAudioClient.

The client will sustain a connection to this channel until it is kicked, disconnected from Discord, or told to Disconnect.

The IAudioClient
----------------

The IAudioClient is used to connect/disconnect to/from a Voice Channel, and to send audio to that Voice Channel.

.. function:: IAudioClient.Disconnect();
	
	Disconnects the IAudioClient from the Voice Server.

..function:: IAudioClient.Join(Channel);
	
	Moves the IAudioClient to another channel on the Voice Server, or starts a connection if one has already been terminated.

.. note::

	Because versions previous to 0.9 do not discretely differentiate between Text and Voice Channels, you may want to ensure that users cannot request the audio client to join a text channel, as this will throw an exception, leading to potentially unexpected behavior

..function:: IAudioClient.Wait();
	
	Blocks the current thread until the sending audio buffer has cleared out. 

..function:: IAudioClient.Clear();
	
	Clears the sending audio buffer.

..function:: IAudioClient.Send(byte[] data, int offset, int count);
	
	Adds a stream of data to the Audio Client's internal buffer, to be sent to Discord. Follows the standard c# Stream.Send() format.

Broadcasting
------------

There are multiple approaches to broadcasting audio. Discord.Net will convert your audio packets into Opus format, so the only work you need to do is converting your audio into a format that Discord will accept. The format Discord takes is 16-bit 48000Hz PCM.

Broadcasting with NAudio
------------------------

`NAudio`_ is one of the easiest approaches to sending audio, although it is not multi-platform compatible. The following example will show you how to read an mp3 file, and send it to Discord.
You can `download NAudio from NuGet`_.

.. code-block:: csharp6

	using NAudio;
	using NAudio.Wave;
	using NAudio.CoreAudioApi;
	
	public void SendAudio(string filePath)
	{
		var channelCount = _client.GetService<AudioService>().Config.Channels; 		// Get the number of AudioChannels our AudioService has been configured to use.
		var OutFormat = new WaveFormat(48000, 16, channelCount); 					// Create a new Output Format, using the spec that Discord will accept, and with the number of channels that our client supports.
		using (var MP3Reader = new Mp3FileReader(filePath)) 						// Create a new Disposable MP3FileReader, to read audio from the filePath parameter
		using (var resampler = new MediaFoundationResampler(MP3Reader, OutFormat))  // Create a Disposable Resampler, which will convert the read MP3 data to PCM, using our Output Format
		{
			resampler.ResamplerQuality = 60; 										// Set the quality of the resampler to 60, the highest quality
			int blockSize = outFormat.AverageBytesPerSecond / 50;					// Establish the size of our AudioBuffer
			byte[] buffer = new byte[blockSize];
			int byteCount;

			while((byteCount = resampler.Read(buffer, 0, blockSize)) > 0) 			// Read audio into our buffer, and keep a loop open while data is present
			{
				if (byteCount < blockSize)
				{
					// Incomplete Frame
					for (int i = byteCount; i < blockSize; i++)
						buffer[i] = 0;
				}
				_vClient.Send(buffer, 0, blockSize)									// Send the buffer to Discord
			}
		}

	}

.. _NAudio: https://naudio.codeplex.com/
.. _download NAudio from NuGet: https://www.nuget.org/packages/NAudio/

Broadcasting with FFmpeg
------------------------

`FFmpeg`_ allows for a more advanced approach to sending audio, although it is multiplatform safe. The following example will show you how to stream a file to Discord.

.. code-block::csharp6

	public void SendAudio(string pathOrUrl)
	{
		var process = Process.Start(new ProcessStartInfo {							// FFmpeg requires us to spawn a process and hook into its stdout, so we will create a Process
			FileName = "ffmpeg",
			Arguments = $"-i {pathOrUrl}" +											// Here we provide a list of arguments to feed into FFmpeg. -i means the location of the file/URL it will read from
				"-f s16le -ar 48000 -ac 2 pipe:1",									// Next, we tell it to output 16-bit 48000Hz PCM, over 2 channels, to stdout. 
			UseShellExecute = false,
			RedirectStandardOutput = true											// Capture the stdout of the process
		});
		Thread.Sleep(2000);															// Sleep for a few seconds to FFmpeg can prebuffer.

		int blockSize = 3840;														// The size of bytes to read per frame; 1920 for mono
		byte[] buffer = new byte[blockSize];
		int byteCount;

		while (true)																// Loop forever, so data will always be read
		{
			byteCount = process.StandardOutput.BaseStream							// Access the underlying MemoryStream from the stdout of FFmpeg
				.Read(buffer, 0, blockSize)											// Read stdout into the buffer

			if (byteCount == 0)														// FFmpeg did not output anything
				break;																// Break out of the while(true) loop, since there was nothing to read.

			_vClient.Send(buffer, 0, byteCount)										// Send our data to Discord
		}
		_vClient.Wait();															// Wait for the Voice Client to finish sending data, as ffMPEG may have already finished buffering out a song, and it is unsafe to return now.
	}

.. _FFmpeg: https://ffmpeg.org/

.. note::
	
	The code-block above assumes that your client is configured to stream 2-channel audio. It also may prematurely end a song. FFmpeg can — especially when streaming from a URL — stop to buffer data from a source, and cause your output stream to read empty data. Because the snippet above does not safely track for failed attempts, or buffers, an empty buffer will cause playback to stop. This is also not 'memory-friendly'.

Multi-Server Broadcasting
-------------------------

.. warning:: Multi-Server broadcasting is not supported by Discord, will cause performance issues for you, and is not encouraged. Proceed with caution.

To prepare for Multi-Server Broadcasting, you must first enable it in your config.

.. code-block::csharp6
	
	_client.UsingAudio(x => 
	{
		x.Mode == AudioMode.Outgoing;
		x.EnableMultiserver = true;	// Enable Multiserver
	});

From here on, it is as easy as creating an IAudioClient for each server you want to join. See the sections on broadcasting to proceed.


Receiving
---------

**Receiving is not implemented in the latest version of Discord.Net**