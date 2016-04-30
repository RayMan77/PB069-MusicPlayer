using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using CSCore;
using CSCore.Codecs;
using CSCore.Codecs.WAV;
using CSCore.SoundIn;
using CSCore.SoundOut;

namespace PB_069_MusicPlayer.MusicPlayer
{
	public class PlayManager 
	{

		
		private IWaveSource soundSource;
		private ISoundOut soundOut;
		private bool restartSong;


		public Playlist Playlist { get; set; }
		public int CurrPlaying;
		
		public bool Shuffle { get; set; }
		public RepeatOptions Repeat { get; set; }


#region constructors
		public PlayManager(Playlist playlist, bool shuffle, RepeatOptions repeat)
		{
			Playlist = playlist;
			Shuffle = shuffle;
			Repeat = repeat;
		}


		public PlayManager(Playlist playlist) : this(playlist, false, RepeatOptions.NoRepeat)
		{}
#endregion

		private ISoundOut GetSoundOut()
		{
			if (WasapiOut.IsSupportedOnCurrentPlatform)
				return new WasapiOut();
			return new DirectSoundOut();
		}


		public void Play()
		{
			CurrPlaying = 0;

			while (CurrPlaying<Playlist.PlayList.Count)
			{
				var song = Playlist.PlayList[CurrPlaying];
				Console.WriteLine("playing " + song.SongName);

				using (soundSource = CodecFactory.Instance.GetCodec(song.SongPath))
				{
					using (soundOut = GetSoundOut())
					{

						soundOut.Initialize(soundSource);

						soundOut.Play();





						while (soundOut.PlaybackState == PlaybackState.Playing || soundOut.PlaybackState == PlaybackState.Paused)
						{
							Thread.Sleep(1);
							if (!restartSong) continue;
							CurrPlaying--;
							restartSong = false;
							break;
						}


					}

				}
				CurrPlaying++;
			}

				
			
			

		}


		public void ChangeSong(int song)
		{
			
		}

		public void Pause()
		{
			soundOut.Pause();
		}

		public void UnPause()
		{
			soundOut.Resume();
		}

		public void Dispose()
		{
			soundSource.Dispose();
			soundOut.Dispose();
		}

		public bool IsPlaying()
		{
			return soundOut.PlaybackState == PlaybackState.Playing;
		}

		public enum RepeatOptions
		{
			NoRepeat,RepeatThisPlaylist,GoToNextPlaylist
		}



		public void RestartSong()
		{
			restartSong = true;
		}
	}
}
