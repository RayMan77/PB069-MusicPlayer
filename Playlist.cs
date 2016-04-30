using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using PlaylistParsers;

namespace PB_069_MusicPlayer
{
	public class Playlist
	{
		public List<Song> PlayList { get; }

		


		public Song NowPlaying { get; set; }

		public Playlist(List<Song> playList, Song nowPlaying)
		{
			PlayList = playList;
			
			NowPlaying = nowPlaying;
			
		}

		public Playlist(List<Song> playList)
		{
			PlayList = playList;
			
			
		}


		private static void Shuffle<T>(IList<T> list)
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
	}
}