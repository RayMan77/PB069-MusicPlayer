using System.Threading;
using CSCore;
using CSCore.Codecs;
using CSCore.Codecs.WAV;
using CSCore.SoundIn;
using CSCore.SoundOut;

namespace PB_069_MusicPlayer.MusicPlayer
{
	public class Playing 
	{
		

		public Playing(string song)
		{
			Song = song;
		}

		public string Song { get; set; }


		public void Play()
		{

			using (IWaveSource soundSource = CodecFactory.Instance.GetCodec(Song))
			{
				//SoundOut implementation which plays the sound
				using (ISoundOut soundOut = GetSoundOut())
				{
					//Tell the SoundOut which sound it has to play
					soundOut.Initialize(soundSource);
					//Play the sound
					soundOut.Play();

					Thread.Sleep(20000);

					//Stop the playback
					//soundOut.Stop();
				}
			}

		}
		private ISoundOut GetSoundOut()
		{
			if (WasapiOut.IsSupportedOnCurrentPlatform)
				return new WasapiOut();
			else
				return new DirectSoundOut();
		}

		
	}
}
