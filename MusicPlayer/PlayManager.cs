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
		private bool nextSong;
		private bool prevSong;


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
							Console.WriteLine(CurrPlaying);
							if (restartSong)
							{
								CurrPlaying--;
								restartSong = false;
								break;
								
							}
							if (nextSong )
							{
		
								nextSong = false;
								break;
							}
							if (prevSong)
							{
								
								prevSong = false;
								break;
							}
							Thread.Sleep(1);
						}


					}

				}
				CurrPlaying++;
			}


			Console.WriteLine("the end");
			
			

		}

		public void NextSong()
		{
			nextSong = true;
			if (CurrPlaying + 1 == Playlist.PlayList.Count)
			{
				CurrPlaying = -1;
			}

			
		}

		public void PreviousSong()
		{
			prevSong = true;
			if (CurrPlaying - 1 < 0)
			{
				CurrPlaying = Playlist.PlayList.Count - 2;
			}
			else
			{
				CurrPlaying -= 2;
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

		public void RestartSong()
		{
			restartSong = true;
		}

		public enum RepeatOptions
		{
			NoRepeat,RepeatThisPlaylist,GoToNextPlaylist
		}

		
	}
}
