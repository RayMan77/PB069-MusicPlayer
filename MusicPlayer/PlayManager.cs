using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using CSCore;
using CSCore.Codecs;
using CSCore.Codecs.WAV;
using CSCore.SoundIn;
using CSCore.SoundOut;
using CSCore.Streams.Effects;
using PlaylistParsers;

namespace PB_069_MusicPlayer.MusicPlayer
{
	public class PlayManager
	{
		#region library staff

		private IWaveSource soundSource;

		private ISoundOut soundOut;

		private static ISoundOut GetSoundOut()
		{
			if (WasapiOut.IsSupportedOnCurrentPlatform)
				return new WasapiOut();
			return new DirectSoundOut();
		}

		#endregion

		#region properties/variables

		

		private enum RepeatOptions
		{
			NoRepeat, RepeatThisPlaylist, GoToNextPlaylist
		}

		private bool Shuffle { get; set; }

		private RepeatOptions Repeat { get; set; }


		
		



		private List<Playlist> ListOfPlaylists;

		private int CurrPlaying { get; set; }

		private Song CurrSong { get; set; }

		private Playlist CurrPlaylist { get; set; }


		private bool initialized;

		private bool songChange;
		private bool plChanged;
		
		private bool paused;


		
		
		

		public event OnSongChangedHandler OnSongChanged;

		#endregion

		#region constructors


		public PlayManager()
		{
			initialized = false;
			CurrPlaylist = new Playlist();
			ListOfPlaylists = new List<Playlist> {CurrPlaylist};
			
		}
		#endregion

		#region PlayCore
		


		public void Play()
		{
			

			while (CurrPlaying < CurrPlaylist.SongList.Count && Repeat==RepeatOptions.NoRepeat)
			{
				CurrSong = CurrPlaylist.SongList[CurrPlaying];
				
				
				OnSongChanged?.Invoke(this, new OnSongChanged(CurrSong.SongName));

				using (soundSource = CodecFactory.Instance.GetCodec(CurrSong.SongPath))
				{
					using (soundOut = GetSoundOut())
					{
						soundOut.Initialize(soundSource);
						soundOut.Play();

						if (paused)
						{
							paused = false;
							soundOut.Pause();
						}
						
						while (soundOut.PlaybackState == PlaybackState.Playing || soundOut.PlaybackState == PlaybackState.Paused)
						{
							if (songChange )
							{
								songChange = false;
								break;
							}
							if (plChanged)
							{
								plChanged = false;
								
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

		#endregion

		#region songChange

		public void NextSong()
		{
			if (soundOut == null) return;
			if (soundOut.PlaybackState == PlaybackState.Paused)
			{
				paused = true;
			}
			songChange = true;
			if (CurrPlaying + 1 == CurrPlaylist.SongList.Count)
			{
				CurrPlaying = -1;
			}


		}

		public void PreviousSong()
		{
			if (soundOut == null) return;
			if (soundOut.PlaybackState == PlaybackState.Paused)
			{
				paused = true;
			}
			songChange = true;
			if (CurrPlaying - 1 < 0)
			{
				CurrPlaying = CurrPlaylist.SongList.Count - 2;
			}
			else
			{
				CurrPlaying -= 2;
			}

		}

		public void ChangeSong(int song)
		{
			if(song<0)return;;
			CurrPlaying = song;
			songChange = true;
		}

		public void RestartAndPause()
		{
			CurrPlaying--;
			songChange = true;
			paused = true;
		}

		public void RestartSong()
		{
			CurrPlaying--;
			songChange = true;
		}

		#endregion

		#region songPausePlay

		public void Pause()
		{
			soundOut.Pause();
		}

		public void UnPause()
		{
			soundOut.Resume();
		}

		public bool IsPlaying()
		{
			return soundOut.PlaybackState == PlaybackState.Playing;
		}

		#endregion

		#region PlaylistManagement

		public List<string> AddToPlaylist(string[] songs)
		{
			var list = songs.Select(song => new Song(Path.GetFileNameWithoutExtension(song), song)).ToList();
			
			CurrPlaylist.SongList.AddRange(list);
			
			
			if (!initialized)
			{
				CurrPlaying = 0;
				CurrSong = CurrPlaylist.SongList[CurrPlaying];
			}


			initialized = true;
			return ParseForListView(CurrPlaylist.SongList);
		}


		public void AddPlaylist(Playlist playlist)
		{
			if(playlist!=null)
				ListOfPlaylists.Add(playlist);
		}

		public List<string> AddPlaylist(string path)
		{
			var parser = new M3UParser(path);
			

			CurrPlaylist = new Playlist(parser.Songs);
			ListOfPlaylists.Add(CurrPlaylist);
			CurrPlaying = 0;
			CurrSong = CurrPlaylist.SongList[CurrPlaying];
			if (initialized)
			{
				plChanged = true;
				Pause();
				CurrPlaying--;
			}
			
			initialized = true;
			return ParseForListView(CurrPlaylist.SongList);

		}

		public List<string> ParseForListView(List<Song> songs )
		{
			var list = new List<string>();
			int counter = 1;
			foreach (var p in songs)
			{
				list.Add(counter + ". " + p.SongName);
				counter++;
			}
			return list;
		}

		

		#endregion

		#region initialized/end

		public bool IsInitialized()
		{
			return initialized;
		}

		public void Dispose()
		{
			soundSource.Dispose();
			soundOut.Dispose();
		}


		#endregion
	}

	public delegate void OnSongChangedHandler(object source, OnSongChanged songInfo);
	public class OnSongChanged : EventArgs
	{

		private int Id;
		private string SongName;

		public OnSongChanged(string SongName)
		{
			this.SongName = SongName;
		}

		public OnSongChanged(int id)
		{
			Id = id;
		}

		public int GetSongId()
		{
			return Id;
		}
		public string GetSongName()
		{
			return SongName;
		}
	}


}
