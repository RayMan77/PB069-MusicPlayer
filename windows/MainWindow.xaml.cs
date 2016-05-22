using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
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

		public readonly EqualizerWindow equalizerWindow;
		public readonly PlaylistWindow playlistWindow;
		private PlayManager pl;
		private Thread thread;
		


		public MainWindow()
		{
			pl = new PlayManager();
			thread = new Thread(pl.Play);
			InitializeComponent();
			
			equalizerWindow = new EqualizerWindow();
			playlistWindow = new PlaylistWindow(pl);
			

			StabilizeWindows();
			
			pl.OnSongChanged += SongChanged;

		}

		#region Window Closing disposing

		private void quitDispoze()
		{
			if (!thread.IsAlive) return;
			pl.Dispose();
			thread.Abort();
		}
		private void Window_Closed(object sender, EventArgs e)
		{
					
			Application.Current.Shutdown();
			quitDispoze();
			
		}
		private void Button_Click(object sender, RoutedEventArgs e)
		{
					
			Application.Current.Shutdown();
			quitDispoze();
			
		}
		#endregion

		#region WindowStabilizator/open

		private bool deactive = false;
		private bool _pLactive;
		private bool _eQactive;
		

		private void Window_Activated(object sender, EventArgs e)
		{
			if (!deactive) return;
			if (_eQactive)
			{
				equalizerWindow.Activate();
			}

			if (_pLactive)
			{
				playlistWindow.Activate();
			}
			Activate();
			deactive = false;
		}

		private void Window_Deactivated(object sender, EventArgs e)
		{
			deactive = true;

		}

		private void StabilizeWindows()
		{
			
			
			playlistWindow.Left = Application.Current.MainWindow.Left;
			playlistWindow.Top = Application.Current.MainWindow.Top + Application.Current.MainWindow.Height-1;
			equalizerWindow.Left = Application.Current.MainWindow.Left + Application.Current.MainWindow.Width -1;
			equalizerWindow.Top = Application.Current.MainWindow.Top;
		}

		private void PlaylistBtn_Click(object sender, RoutedEventArgs e)
		{

			StabilizeWindows();
			if (!_pLactive)
			{
				playlistWindow.Activate();
				playlistWindow.Show();
				_pLactive = true;
			}
			else
			{
				playlistWindow.Hide();
				_pLactive = false;
			}

			
		}

		private void EQBtn_Click(object sender, RoutedEventArgs e)
		{
			StabilizeWindows();
			if (!_eQactive)
			{
				equalizerWindow.Activate();
				equalizerWindow.Show();
				_eQactive = true;
			}
			else
			{
				_eQactive = false;
				equalizerWindow.Hide();
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

		#region baseBtns (play/stop) etc
		private void PlayBtn_Click(object sender, RoutedEventArgs e)
		{
			if (!pl.IsInitialized()) return;
			if (!thread.IsAlive)
			{
				thread.Start();
			}
			else if (!pl.IsPlaying())
			{
				pl.UnPause();
			}
			else
			{
				pl.RestartSong();
			}
			
		}

		private void StopBtn_Click(object sender, RoutedEventArgs e)
		{
			if (!pl.IsInitialized()) return;
			if (!thread.IsAlive)
			{
				thread.Start();
			}
			else if (pl.IsPlaying())
			{
				pl.Pause();
			}
			else if (!pl.IsPlaying())
			{
				pl.UnPause();
			}
			

		}

		private void PrevSongStopBtnBtn_Click(object sender, RoutedEventArgs e)
		{
			if (!pl.IsInitialized()) return;
			pl.PreviousSong();
			
		}

		private void NextSongStopBtnBtn_Click(object sender, RoutedEventArgs e)
		{
			if (!pl.IsInitialized()) return;
			pl.NextSong();
			


		}

		private void StopPlaybackBtn_Click(object sender, RoutedEventArgs e)
		{
			if (!pl.IsInitialized()) return;
			pl.RestartAndPause();

			
			
		}



		private void rollDown(object sender, RoutedEventArgs e)
		{
			var btn = sender as Button;
			if (btn.ContextMenu.IsEnabled)
			{
				btn.ContextMenu.IsEnabled = false;
				btn.ContextMenu.StaysOpen = false;
				btn.ContextMenu.IsOpen = false;
			}

			btn.ContextMenu.IsEnabled = true;
			btn.ContextMenu.PlacementTarget = (sender as Button);
			btn.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
			btn.ContextMenu.IsOpen = true;
		}
		#endregion

		private void SongChanged(object source, OnSongChanged e)
		{
			Dispatcher.Invoke(() =>
			{
				nowPlayingLabel.Content = e.GetSongName();
			});

		}

		#region loadSongs

		/// <summary>
		/// Loading songFiles to current playlist
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void loadPlaylistBtn_Click(object sender, RoutedEventArgs e)
		{


			var openFileD = new OpenFileDialog
			{
				Filter = "PlaylistFiles (.m3u)|*.m3u|All Files (*.*)|*.*",
				FilterIndex = 1
				
			};
			openFileD.ShowDialog();
			if (openFileD.FileName.Equals(""))
			{
				return;
			}
			
			playlistWindow.playlistBox.ItemsSource = pl.AddPlaylist(openFileD.FileName); ;

			
			_pLactive = true;
			playlistWindow.Show();
			StabilizeWindows();
		}


		private void loadSongs_OnClick(object sender, RoutedEventArgs e)
		{
			
			var openFileD = new OpenFileDialog
			{
				Filter = "All Supported Audio | *.mp3; *.wma | MP3s | *.mp3 | WMAs | *.wma |All Files (*.*)|*.*",
				FilterIndex = 1,
				Multiselect = true

			}; 
			openFileD.ShowDialog();
			if (openFileD.FileName.Equals(""))
			{
				return;
			}
			
			
			playlistWindow.playlistBox.ItemsSource = pl.AddToPlaylist(openFileD.FileNames); ;



			_pLactive = true;

			playlistWindow.Show();
			StabilizeWindows();

		}

		private void loadFolder_OnClick(object sender, RoutedEventArgs e)
		{
			
			var browserDialog = new System.Windows.Forms.FolderBrowserDialog();
			browserDialog.ShowDialog();
			if (browserDialog.SelectedPath.Equals(""))
			{
				return;
			}


			var ext = new List<string> { ".mp3", ".wma" };

			var musicFiles = Directory.GetFiles(browserDialog.SelectedPath, "*.*", SearchOption.AllDirectories)
				 .Where(s => ext.Any(s.EndsWith));
			
			
			
			playlistWindow.playlistBox.ItemsSource = pl.AddToPlaylist(musicFiles.Select(Path.GetFullPath).ToArray());



			_pLactive = true;
			playlistWindow.Show();
			StabilizeWindows();
		}

		#endregion

		private void VolumeTracker_DragCompleted(object sender, DragCompletedEventArgs e)
		{
			pl.Volume = (float) (volumeTracker.Value/10);
		}

		private void volumeTracker_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			pl.Volume = (float)(volumeTracker.Value / 10);
		}
	}
	




}
