using System;
using Gtk;

namespace BitMapReverseColor
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Application.Init ();
			var win = new MainWindow ();
			var app = new App(win);
			app.Show ();
			Application.Run ();
		}
	}
}
