using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using PB_069_MusicPlayer.MusicPlayer;
using PlaylistParsers;

namespace PB_069_MusicPlayer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		private readonly EqualizerWindow _equalizerWindow;
		private readonly PlaylistWindow _playlistWindow;
		private bool _pLactive;
		private bool _eQactive;
		private Playing pl;


		private List<Playlist> ListOfPlaylists { get; }
		private Playlist currentlyPlayingPlaylist;
		private int currentlyPlayingSong;

		private Thread thread;


		public MainWindow()
		{
			InitializeComponent();
			_equalizerWindow= new EqualizerWindow();
			_playlistWindow = new PlaylistWindow();
			currentlyPlayingPlaylist =new Playlist(new List<Song>());

			ListOfPlaylists = new List<Playlist> {currentlyPlayingPlaylist};
			StabilizeWindows();
			pl = new Playing(currentlyPlayingPlaylist);


		}

		#region Window Closed
				private void Window_Closed(object sender, EventArgs e)
				{
					Application.Current.Shutdown();
				}
				private void Button_Click(object sender, RoutedEventArgs e)
				{
					Application.Current.Shutdown();
				}
		#endregion
		#region WindowStab

		private void StabilizeWindows()
		{
			_playlistWindow.Left = Application.Current.MainWindow.Left;
			_playlistWindow.Top = Application.Current.MainWindow.Top + Application.Current.MainWindow.Height;
			_equalizerWindow.Left = Application.Current.MainWindow.Left + Application.Current.MainWindow.Width - 5;
			_equalizerWindow.Top = Application.Current.MainWindow.Top;
		}

		private void PlaylistBtn_Click(object sender, RoutedEventArgs e)
		{
			StabilizeWindows();
			if (!_pLactive)
			{
				_playlistWindow.Show();
				_pLactive = true;
			}
			else
			{
				_playlistWindow.Hide();
				_pLactive = false;
			}
			
		}

		private void EQBtn_Click(object sender, RoutedEventArgs e)
		{
			StabilizeWindows();
			if (!_eQactive)
			{
				_equalizerWindow.Show();
				_eQactive = true;
			}
			else
			{
				_eQactive = false;
				_equalizerWindow.Hide();
			}
		}

		private void Window_LocationChanged(object sender, EventArgs e)
		{

			StabilizeWindows();
		}

		#endregion
		#region WindowMove
		private void menu_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				Application.Current.MainWindow.DragMove();

		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				Application.Current.MainWindow.DragMove();
		}
#endregion

		private void PlayBtn_Click(object sender, RoutedEventArgs e)
		{
			
			var kappa = currentlyPlayingPlaylist.PlayList.ToArray();
			pl.Song = kappa[currentlyPlayingSong].SongPath;
			thread = new Thread(pl.Play);
			thread.Start();
			
			
			
		}

		private void loadPlaylistBtn_Click(object sender, RoutedEventArgs e)
		{
			
			var openFileD = new OpenFileDialog();
			openFileD.ShowDialog();
			if (openFileD.FileName.Equals(""))
			{
				return;
			}
			var parser = new M3UParser(openFileD.FileName);

			int counter = 1;
			foreach (var path in parser.Songs)
			{
				_playlistWindow.listBox.Items.Add(counter+". "+path.SongName);
				counter++;
			}

//			if (currentlyPlayingPlaylist.PlayList.Count==0)
//			{
				currentlyPlayingPlaylist.PlayList.AddRange(parser.Songs);
//
//			}
//			else
//			{
//				ListOfPlaylists.Add(new Playlist(parser.Songs));
//			}

			_pLactive = true;
			_playlistWindow.Show();
			StabilizeWindows();
		}
	}
}
