# Essay-CSharp-DIP.Performance.Parallel

🐔  versus 👨🏻‍🎓: WHO is faster? Is CSharp "slow"?

__菜鸡 vs. 大佬__: 哪个更快一些？CSharp “慢”吗？

这个 benchmark 使用 `DateTime.Now.Ticks` 比较了利用不同算法对 Gdk `Pixbuf` 进行反色 DIP 操作（串行进行，但可能使用并行计算）的计算效率

包含两种算法实现，一种是『小白』的 `Array.Reverse` 和 `foreach x y.` 计算，另一种是大佬的 `Parallel.ForEach` 和 `while (row < end)` 算法

## Screenshots

![result](https://github.com/duangsuse/Essay-CSharp-DIP.Performance.Parallel/raw/master/Screenshot_20190519_235758.png)
![inverseUsingNoobAlgorithm](https://github.com/duangsuse/Essay-CSharp-DIP.Performance.Parallel/raw/master/Screenshot_20190519_230851.png)
![noobInverseCode](https://github.com/duangsuse/Essay-CSharp-DIP.Performance.Parallel/raw/master/Screenshot_20190519_230802.png)
![masterInverseCode](https://github.com/duangsuse/Essay-CSharp-DIP.Performance.Parallel/raw/master/Screenshot_20190519_235404.png)

## Implementation <sup>[source code](https://github.com/duangsuse/Essay-CSharp-DIP.Performance.Parallel/blob/master/BitMapReverseColor/PixmapOpts.cs)</sup>

### Noob implementation 🐔

```csharp
public static Pixbuf noobInversePixbuf(Pixbuf buffer)
{
  var result = buffer.Clone() as Pixbuf;
  var pixels = result.Pixels;

  for (int h = 0; h < buffer.Height; h++)
    for (int w = 0; w < buffer.Rowstride; w++) unsafe
      {
        int index = h * buffer.Rowstride + w;
        uint* pix = (uint*)(pixels + index).ToPointer();
        var bytes = BitConverter.GetBytes(*pix);

        for (int i = 1; i <= 3; i++) bytes[i] = (byte)(0xff - bytes[i]);
        *pix = BitConverter.ToUInt32(bytes, 0); // Alpha Red Green Blue
                                                //Console.Write (index);
      }
  //Console.WriteLine ();
  return result;
}
```

### Master implementation 👨🏻‍🎓

```csharp
static public unsafe void masterInversePixbuf(Gtk.Image gtkImg)
{
  var buffer = gtkImg.Pixbuf;
  IntPtr data = buffer.Pixels;

  int roww = buffer.Rowstride;

  Parallel.ForEach(Partitioner.Create(0, buffer.Height), (h) =>
  {
    for (int i = h.Item1; i < h.Item2; i++)
    {
      byte* row = (byte*)(data + i * roww).ToPointer();
      byte* end = row + roww;
      for (; row < end; row++) *row = (byte)~*(row);

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
```

## References 📚

[你觉得 .NET 性能低，可能只是因为你的能力低](https://www.cnblogs.com/conmajia/p/low-performance-csharp-all-your-fault.html)
