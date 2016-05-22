using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Documents;
using PlaylistParsers;

namespace PB_069_MusicPlayer
{
	public class Playlist
	{
		#region properties/variables


		

		

		public List<Song> SongList { get; }

		

//
//		public int NowPlayingNum
//		{
//			get
//			{
//				return _nowPlayingNum;
//			}
//			set
//			{
//				
//				_nowPlayingNum = value;
//
//				if (_nowPlayingNum >= SongList.Count)
//				{
//					_nowPlayingNum = SongList.Count-1;
//				}
//				if (_nowPlayingNum < 0)
//				{
//					_nowPlayingNum = 0;
//				}
//				NowPlayingSong = SongList[_nowPlayingNum];
//			}
//		}

		#endregion

//		public void SetNowPlaying(int num)
//		{
//			_nowPlayingNum += num;
//
//			if (_nowPlayingNum >= SongList.Count)
//			{
//				_nowPlayingNum = 0;
//			}
//			if (_nowPlayingNum < 0)
//			{
//				_nowPlayingNum = SongList.Count-2 ;
//			}
//			Console.WriteLine(_nowPlayingNum);
//			NowPlayingSong = SongList[_nowPlayingNum];
//		}



		#region constructors
		public Playlist(List<Song> songList)
		{
			SongList = songList;
		}
		
		public Playlist():this(new List<Song>()){}

		#endregion

		#region shuffle
		private static void ShufflePL<T>(IList<T> list)
		{
			int n = list.Count;
			var rnd = new Random();
			while (n > 1)
			{
				int k = (rnd.Next(0, n) % n);
				n--;
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
		}
#endregion


		

	}
}