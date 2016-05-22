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
	/// Interaction logic for EqualizerWindow.xaml
	/// </summary>
	public partial class EqualizerWindow : Window
	{
		public EqualizerWindow()
		{
			InitializeComponent();
			
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			WinHelp.WindowHelp(this);
		}


		
	}
}
