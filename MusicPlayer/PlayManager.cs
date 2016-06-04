using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Numerics;
using System.Threading;
using CSCore;
using CSCore.Codecs;
using CSCore.Codecs.WAV;
using CSCore.CoreAudioAPI;
using CSCore.SoundIn;
using CSCore.SoundOut;
using CSCore.Streams;
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

		public bool Shuffle
		{
			get { return _shuffle; }
			set
			{
				_shuffle = value;
				if (!value && initialized)
				{
					CurrPlaying = shuffleArr[CurrPlaying];
				}
			}
		}

		private int[] shuffleArr;


		#region repeat
		private RepeatOptions repeat;
		
		public void SetRepeat(bool repeat)
		{
			this.repeat = repeat ? RepeatOptions.RepeatThisPlaylist : RepeatOptions.NoRepeat;
		}
		public enum RepeatOptions
		{
			NoRepeat, RepeatThisPlaylist, GoToNextPlaylist
		}
		#endregion


		public const double maxDb = 20 ;
		public double MaxDb => maxDb;
		private List<Playlist> ListOfPlaylists;

		public int CurrPlaying { get;private set; }

		private Song CurrSong { get; set; }

		private Playlist CurrPlaylist { get; set; }
		private int CurrPlaylistNum;


		private bool initialized;

		private bool songChange;
		private bool plChanged;

		private bool emptyPlaylist;
		private bool paused;
		private bool _shuffle;


		public int Time { get; set; }
		public bool RepeatSong { get; set; }

		public float Volume { get; set; }
		public double Progress { get; set; }
		private Equalizer equalizer;

		public Equalizer Equalizer
		{
			get
			{
				return equalizer;
			}
			private set { equalizer = value; }
		}


		public int CurrPlaylingShuff
		{
			get { return shuffleArr[CurrPlaying]; }
			private set {}
		}


		public event OnSongChangedHandler OnSongChangedHandler;
		


		



		#endregion

		#region constructors


		public PlayManager():this(false,false,false){}

		public PlayManager(bool shuffle, bool repeatPlaylist, bool reapeatSong)
		{
			initialized = false;
			CurrPlaylist = new Playlist();
			ListOfPlaylists = new List<Playlist> { CurrPlaylist };
			CurrPlaylistNum = 0;
			shuffleArr=new int[CurrPlaylist.SongList.Count];
			Shuffle = shuffle;
			ShufflePlaylist();
			RepeatSong = reapeatSong;
			SetRepeat(repeatPlaylist);
			Volume =  0.25f;
			emptyPlaylist = CurrPlaylist.SongList.Count == 0;
			emptyPlaylist = true;




		}
		#endregion

		#region PlayCore
		


		public void Play()
		{
			while (!emptyPlaylist )
			{
				while (CurrPlaying < CurrPlaylist.SongList.Count )
				{
					
					CurrSong = Shuffle ? CurrPlaylist.SongList[shuffleArr[CurrPlaying] ] : CurrPlaylist.SongList[CurrPlaying];
					


					

					
					using (soundSource = CodecFactory.Instance.GetCodec(CurrSong.SongPath)
						.ChangeSampleRate(32000)
						.AppendSource(Equalizer.Create10BandEqualizer, out equalizer).ToWaveSource())
					{
						using (soundOut = GetSoundOut())
						{
							soundOut.Initialize(soundSource);
							soundOut.Play();
							OnSongChangedHandler?.Invoke(this, new OnSongChanged(CurrSong.SongName));



							if (paused)
							{
								paused = false;
								soundOut.Pause();
							}




							while (soundOut.PlaybackState == PlaybackState.Playing || soundOut.PlaybackState == PlaybackState.Paused)
							{


								if (songChange)
								{
									songChange = false;
									break;
								}
								if (plChanged)
								{
									plChanged = false;

									break;
								}
								if (initialized)
								{
									Time = (int)(soundSource.Position / soundSource.WaveFormat.BytesPerSecond);
									Progress = (double)soundSource.Position / soundSource.Length;
									soundOut.Volume = Volume;
								}
								





								Thread.Sleep(1);
							}


						}

					}
					if (!RepeatSong)
					{
						CurrPlaying++;
					}
					Progress = 0;


				}
				#region repeat
				CurrPlaying = 0;
				switch (repeat)
				{
					case RepeatOptions.NoRepeat:
						paused = true;
						break;
					case RepeatOptions.GoToNextPlaylist:
						CurrPlaylistNum++;
						if (CurrPlaylistNum > ListOfPlaylists.Count)
						{
							CurrPlaylistNum = 0;
						}
						CurrPlaylist = ListOfPlaylists[CurrPlaylistNum];
						break;
				}

				#endregion
			}
			
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
			if (song < 0) song = -1;
			
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

			emptyPlaylist = false;
			initialized = true;
			return ParseForListView(CurrPlaylist.SongList);
		}


		public void AddPlaylist(Playlist playlist)
		{
			if(playlist!=null)
				ListOfPlaylists.Add(playlist);
			emptyPlaylist = false;
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
			emptyPlaylist = false;
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




		public void ShufflePlaylist()
		{
			if (!Shuffle) return;
			shuffleArr = new int[CurrPlaylist.SongList.Count];
			Console.WriteLine(CurrPlaying + " curr play");
			for (int i = 0; i < shuffleArr.Length; i++)
			{
				shuffleArr[i] = i;
			}
			Random r = new Random();
			for (int i = shuffleArr.Length; i > 0; i--)
			{
				int j = r.Next(i);
				int k = shuffleArr[j];
				shuffleArr[j] = shuffleArr[i - 1];
				shuffleArr[i - 1] = k;
			}
			foreach (var i in shuffleArr)
			{
				Console.WriteLine(i + " ");
			}
		}
		#endregion

		#region initialized/end

		public bool IsInitialized()
		{
			return initialized;
		}

		public void Dispose()
		{
			initialized = false;
			soundSource.Dispose();
			soundOut.Dispose();
		}


		#endregion

		public List<string> DeleteSongsFromPlaylist(List<int> indices  )
		{
			foreach (var index in indices)
			{
				if(index<CurrPlaylist.SongList.Count)
					CurrPlaylist.SongList.RemoveAt(index);
			}
			if (CurrPlaylist.SongList.Count == 0)
			{
				emptyPlaylist = true;
			}
			return ParseForListView(CurrPlaylist.SongList);
		}

		

		public void SetPosition(double pos)
		{
			if (initialized && soundSource!=null)
			{
				soundSource.Position = (long)(pos * soundSource.Length);
			}
			

		}


		public void SavePlaylist(string path)
		{
			M3UParser.Save(CurrPlaylist.SongList,path);
		}
	}

	#region events
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

	
	#endregion
}
