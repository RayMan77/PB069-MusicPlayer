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

namespace PB_069_MusicPlayer
{
	/// <summary>
	/// Interaction logic for PlaylistWindow.xaml
	/// </summary>
	public partial class PlaylistWindow : Window
	{
		public PlaylistWindow()
		{
			InitializeComponent();
			
		}

		private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
		{
			if(playlistBox.SelectedItems.Count>0)
			Console.WriteLine(playlistBox.SelectedItems[0].ToString().Split('.').First());
			
		}

		

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			WinHelp.WindowHelp(this);
		}
		
	}
}
