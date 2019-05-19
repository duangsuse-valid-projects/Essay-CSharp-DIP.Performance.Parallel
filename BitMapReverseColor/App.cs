using System;
using System.IO;
using Gtk;
using Gdk;

namespace BitMapReverseColor
{
	public class App
	{
		readonly MainWindow win;
		volatile bool uiSetup = false;

		readonly Gtk.Image noobImg, masterImg;
		readonly TextView noobLog, masterLog;
		readonly Button doNoob, doMaster;
		readonly FileChooserWidget imageFileChoose;

		DateTime operationStarted;

		public App (MainWindow window)
		{
			win = window;
			noobImg = path<Gtk.Image> (0, 0, 0);
			masterImg = path<Gtk.Image> (0, 0, 1);

			noobLog = path <TextView> (0, 1, 1, 1, 0);
			masterLog = path<TextView> (0, 1, 0, 1, 0);

			doNoob = path<Button> (0, 1, 1, 2);
			doMaster = path<Button> (0, 1, 0, 2);

			imageFileChoose = path<FileChooserWidget> (0, 2);
		}

		static Action<string> textTv(TextView v) { return (text) => v.Buffer.InsertAtCursor(text + "\n"); }

		void textNoob(string t) => textTv(noobLog)(t);
		void textMaster(string t) => textTv(masterLog)(t);

		T path<T>(params int[] route) where T: class {
			return winPath<T>(win, route);
		}

		static T winPath<T>(Container root, int[] path) where T: class {
			Container c = root;
			Widget w = null;

			for (int i = 0; i < path.Length; i++) {
				int locate = path [i];
				if (c.Children.Length > locate)
					w = c.Children [locate];
				else
					return null;
				Container cast = w as Container;
				if (cast == null) break;
				c = (Container) w;
			}

			return w as T;
		}

		static void placeCursorEnd(TextView tv) { tv.Buffer.PlaceCursor (tv.Buffer.EndIter); }

		void DoSetUpUI() {
			placeCursorEnd (noobLog);
			placeCursorEnd (masterLog);

			doNoob.Released += DoNoob_Released;
			doMaster.Released += DoMaster_Released;
			imageFileChoose.FileActivated += ImageFileChoose_FileActivated;
		}

		void ImageFileChoose_FileActivated (object sender, EventArgs e)
		{
			var filename = imageFileChoose.Filename;
			Console.WriteLine (filename);

			var bytes = File.ReadAllBytes (filename);
			Pixbuf pixbuf = null;

			try {
				pixbuf = new Pixbuf (bytes);
			} catch (GLib.GException ex) {
				textMaster ($"Failed to load file {filename}: {ex}");
				return;
			}

			textMaster ($"Loaded file {filename}");
			textMaster ($"{pixbuf.Colorspace}: {pixbuf.BitsPerSample}bits / {pixbuf.Rowstride}bytes/row :: {pixbuf.Width} * {pixbuf.Height}");

			noobImg.Pixbuf = pixbuf;
			masterImg.Pixbuf = pixbuf.Clone () as Pixbuf;
		}

		void LogTimeEscaped(System.Action fn, Action<long> callback) {
			operationStarted = DateTime.Now;
			fn ();
			callback (DateTime.Now.Ticks - operationStarted.Ticks);
		}

		void DoMaster_Released (object sender, EventArgs e)
		{
			textMaster ("Master: inverse color");

			LogTimeEscaped (() => PixmapOpts.masterInversePixbuf (masterImg), (esc) => textMaster($"Master finished in {esc}"));
		}

		void DoNoob_Released (object sender, EventArgs e)
		{
			textNoob ("Noob: inverse color");

      LogTimeEscaped (() => noobImg.Pixbuf = PixmapOpts.noobInversePixbuf (noobImg.Pixbuf), (esc) => textNoob($"Noob finished in {esc}"));
		}

		void SetUpUI() {
			if (uiSetup) return;
			DoSetUpUI ();
			uiSetup = true;
		}

		public void Show() {
			SetUpUI ();
			win.Show ();
		}
	}
}

