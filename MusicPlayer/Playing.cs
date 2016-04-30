using System;
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
		public Playlist Playlist { get; set; }
		public string Song { get; set; }


		public Playing(Playlist playlist)
		{
			Playlist = playlist;
		}

		


		public void Play()
		{

			foreach (var song in Playlist.PlayList)
			{

				Console.WriteLine("playing "+song.SongName);
				using (IWaveSource soundSource = CodecFactory.Instance.GetCodec(song.SongPath))
				{
					using (ISoundOut soundOut = GetSoundOut())
					{
						//Tell the SoundOut which sound it has to play
						soundOut.Initialize(soundSource);

						soundOut.Play();

						Thread.Sleep((int)soundSource.Length + 1);


						//soundOut.Stop();
					}

				}
			}
			

		}
		private ISoundOut GetSoundOut()
		{
			if (WasapiOut.IsSupportedOnCurrentPlatform)
				return new WasapiOut();
			return new DirectSoundOut();
		}

		public void ChangeSong(int song)
		{
			
		}
	}
}
