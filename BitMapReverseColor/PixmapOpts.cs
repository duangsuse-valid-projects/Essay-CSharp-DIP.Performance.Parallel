using System;

using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

using static Gdk.Pixbuf;
using static Gtk.Image;

using Pixbuf = Gdk.Pixbuf;

namespace BitMapReverseColor
{
	public static class PixmapOpts
	{
		/**
		 * Noob??? ...
		 */
		public static Pixbuf noobInversePixbuf(Pixbuf buffer) {
      var result = buffer.Clone () as Pixbuf;
      var pixels = result.Pixels;

			for (int h = 0; h < buffer.Height; h++)
      for (int w = 0; w < buffer.Rowstride; w++) unsafe {
					int index = h * buffer.Rowstride + w;
          uint *pix = (uint*) (pixels + index).ToPointer();
          var bytes = BitConverter.GetBytes (*pix);

          for (int i = 1; i <= 3; i++) bytes [i] = (byte)(0xff - bytes [i]);
          *pix = BitConverter.ToUInt32(bytes, 0); // Alpha Red Green Blue
          //Console.Write (index);
			}
      //Console.WriteLine ();
      return result;
		}

		static public unsafe void masterInversePixbuf(Gtk.Image gtkImg) {
      var buffer = gtkImg.Pixbuf;
      IntPtr data = buffer.Pixels;

      int roww = buffer.Rowstride;

      Parallel.ForEach (Partitioner.Create(0, buffer.Height), (h) => {
        for (int i = h.Item1; i < h.Item2; i++) {
          byte *row = (byte*) (data + i*roww).ToPointer();
          byte *end = row+roww;
          for (; row < end; row++) *row = (byte) ~*(row);

          // also
          /*while (row < end) {
            row++; // skip alpha
            *row = (byte) ~*(row);
            row++; // skip Red
            *row = (byte) ~*(row);
            row++; // skip Green
            *row = (byte) ~*(row);
            row++; // skip Blue
            }
           */
        }
      });

      gtkImg.Pixbuf = buffer;
		}
	}
}

