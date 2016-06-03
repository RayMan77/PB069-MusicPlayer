using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using PB_069_MusicPlayer.helpers;
using PB_069_MusicPlayer.MusicPlayer;

namespace PB_069_MusicPlayer
{
	/// <summary>
	/// Interaction logic for PlaylistWindow.xaml
	/// </summary>
	public partial class PlaylistWindow : Window
	{
		private PlayManager pl;
		public PlaylistWindow(PlayManager pl)
		{
			InitializeComponent();
			this.pl = pl;
			
		}

		private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			
			if (!pl.IsInitialized()) return;
			
			pl.ChangeSong(playlistBox.SelectedIndex-1);
		}

		
		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			WinHelp.WindowHelp(this);
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			playlistBox.Height = this.Height - 25;
		}

		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (!pl.IsInitialized()) return;
			if (e.Key == Key.Delete)
			{
				var selectedItemIndexes = new List<int>();
				foreach (var o in playlistBox.SelectedItems)
					selectedItemIndexes.Add(playlistBox.Items.IndexOf(o));

				playlistBox.ItemsSource = pl.DeleteSongsFromPlaylist(selectedItemIndexes);


				int curpl = pl.CurrPlaying;
				while (curpl >= playlistBox.Items.Count)
				{
					curpl--;
				}
				
					playlistBox.SelectedItem =
				pl.Shuffle ? playlistBox.Items[pl.CurrPlaylingShuff] : playlistBox.Items[curpl ];
				
				
				

				e.Handled = true;
			}
		}
	}
}
