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
	/// Interaction logic for EqualizerWindow.xaml
	/// </summary>
	public partial class EqualizerWindow : Window
	{
		private PlayManager pl;
		private List<Slider> sliderList;
		

		public EqualizerWindow(PlayManager pl)
		{
			InitializeComponent();
			this.pl = pl;
			pl.OnSongChangedHandler += SongChangedHandler;
			sliderList = new List<Slider>
			{
				slider,slider1,slider2,slider3,slider4,slider5,slider6,slider7,slider8,slider9
			};
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			WinHelp.WindowHelp(this);
		}

		

		private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			
			if (pl.Equalizer == null ) return;
			if (!eqOnCheckbox.IsChecked.Value) return;
			
			changeEQ();

			
			
		}

		private void changeEQ()
		{
			if (pl.Equalizer == null) return;
			foreach (var slider in sliderList)
			{
				var perc = (slider.Value / slider.Maximum);
				var value = (float)(perc * pl.MaxDb);
				int filterIndex = int.Parse((string)slider.Tag);
				var filter = pl.Equalizer.SampleFilters[filterIndex];
				filter.SetGain(value);
			}
		}

		private void eqOnCheckbox_Onchange(object sender, RoutedEventArgs e)
		{
			var check = sender as CheckBox;
			if (!check.IsChecked.Value)
			{
				foreach (var filter in pl.Equalizer.SampleFilters)
				{
					filter.SetGain(0);
				}
			}
			else
			{
				if (pl.Equalizer == null) return;
				if (!eqOnCheckbox.IsChecked.Value) return;

				changeEQ();
			}
		}

		private void SongChangedHandler(object source, OnSongChanged e)
		{
			Dispatcher.Invoke(() =>
			{

				changeEQ();
			});

		}
	}
}
