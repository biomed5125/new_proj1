using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace HMS.Api.Endpoints.Barcodes
{
    /// <summary>
    /// Minimal Code 128 Set B encoder/renderer (Windows GDI+).
    /// </summary>
    public static class Code128Render
    {
        // Code128 patterns as strings of module widths (bars+spaces, starting with bar).
        // 0..102 = data/control, 103 = startA, 104 = startB, 105 = startC, 106 = stop (13 modules).
        static readonly string[] PAT =
        {
            "212222","222122","222221","121223","121322","131222","122213","122312","132212","221213",
            "221312","231212","112232","122132","122231","113222","123122","123221","223211","221132",
            "221231","213212","223112","312131","311222","321122","321221","312212","322112","322211",
            "212123","212321","232121","111323","131123","131321","112313","132113","132311","211313",
            "231113","231311","112133","112331","132131","113123","113321","133121","313121","211331",
            "231131","213113","213311","213131","311123","311321","331121","312113","312311","332111",
            "314111","221411","431111","111224","111422","121124","121421","141122","141221","112214",
            "112412","122114","122411","142112","142211","241211","221114","413111","241112","134111",
            "111242","121142","121241","114212","124112","124211","411212","421112","421211","212141",
            "214121","412121","111143","111341","131141","114113","114311","411113","411311","113141",
            "114131","311141","411131","211412","211214","211232",   // 103,104,105 (Start A,B,C)
            "2331112"                                               // 106 Stop (13 modules)
        };

        // Parsed numeric patterns (each is 6 widths; stop is 7).
        static readonly int[][] P = PAT.Select(p => p.Select(c => c - '0').ToArray()).ToArray();

        /// <summary>
        /// Render Code128-B PNG bytes. Only ASCII 32..126 supported.
        /// </summary>
        public static byte[] RenderPng(string text, bool textBelow = false, int height = 60, int scale = 2)
        {
            if (string.IsNullOrWhiteSpace(text)) text = "-";

            // Build the code sequence: StartB(104), data (ascii-32), checksum, Stop(106)
            var codes = new List<int> { 104 };               // Start B
            foreach (var ch in text)
            {
                if (ch < 32 || ch > 126)
                    throw new ArgumentException($"Unsupported character for Code128-B: '{ch}' (0x{(int)ch:X2})");
                codes.Add(ch - 32);
            }

            // checksum = (start + sum(data[i]*i)) % 103
            int checksum = 104;
            for (int i = 1; i < codes.Count; i++)
                checksum += codes[i] * i;
            checksum %= 103;

            codes.Add(checksum);
            codes.Add(106); // stop

            // Flatten modules (bars/spaces) widths
            var modules = new List<int>();
            foreach (var code in codes)
                modules.AddRange(P[code]);

            // Total width in modules; scale → pixels; add margins
            int moduleCount = modules.Sum();
            int width = moduleCount * scale + 20;
            int extraH = textBelow ? 18 : 0;

            using var bmp = new Bitmap(width, height + extraH);
            using var g = Graphics.FromImage(bmp);
            g.Clear(Color.White);

            int x = 10;
            bool black = true;
            foreach (var m in modules)
            {
                int w = m * scale;
                if (black) g.FillRectangle(Brushes.Black, x, 0, w, height);
                x += w;
                black = !black;
            }

            if (textBelow)
            {
                using var f = new Font("Arial", 10, GraphicsUnit.Point);
                var sz = g.MeasureString(text, f);
                g.DrawString(text, f, Brushes.Black, (width - sz.Width) / 2f, height + 2);
            }

            using var ms = new MemoryStream();
            bmp.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }
    }
}
