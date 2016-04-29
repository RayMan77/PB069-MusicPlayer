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

namespace PB_069_MusicPlayer
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{

		private readonly EqualizerWindow _equalizerWindow;
		private readonly PlaylistWindow _playlistWindow;
		private bool _pLactive = false;
		private bool _eQactive = false;
		public MainWindow()
		{
			InitializeComponent();
			_equalizerWindow= new EqualizerWindow();
			_playlistWindow = new PlaylistWindow();


		}

		private void PlaylistBtn_Click(object sender, RoutedEventArgs e)
		{
			_playlistWindow.Left = Application.Current.MainWindow.Left ;
			_playlistWindow.Top = Application.Current.MainWindow.Top + Application.Current.MainWindow.Height ;





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

		private void Window_Closed(object sender, EventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void EQBtn_Click(object sender, RoutedEventArgs e)
		{
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


			_equalizerWindow.Left = Application.Current.MainWindow.Left + Application.Current.MainWindow.Width-5;
			_equalizerWindow.Top = Application.Current.MainWindow.Top;


			_playlistWindow.Left = Application.Current.MainWindow.Left;
			_playlistWindow.Top = Application.Current.MainWindow.Top + Application.Current.MainWindow.Height ;
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

		private void menu_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				Application.Current.MainWindow.DragMove();

		}

		private void Window_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left)
				this.DragMove();
		}

		private void PlayBtn_Click(object sender, RoutedEventArgs e)
		{
			var openFileD = new OpenFileDialog();
			openFileD.ShowDialog();
			
			var pl = new Playing(openFileD.FileName);
			var thread = new Thread(new ThreadStart(pl.Play));
			thread.Start();
		}
	}
}
