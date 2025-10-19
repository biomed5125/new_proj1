using SkiaSharp;
using ZXing;
using ZXing.Common;
using ZXing.SkiaSharp;
using ZXing.SkiaSharp.Rendering;

namespace HMS.Api.Infrastructure.Barcodes;

public static class BarcodeRenderer
{
    public static byte[] MakeCode128(string payload, int width = 600, int height = 200)
    {
        var writer = new BarcodeWriter<SKBitmap>
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions { Width = width, Height = height, Margin = 0, PureBarcode = true },
            Renderer = new SKBitmapRenderer()
        };
        using var bmp = writer.Write(payload);
        using var img = SKImage.FromBitmap(bmp);
        using var data = img.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }

    public static byte[] MakeDataMatrix(string payload, int size = 200)
    {
        var writer = new BarcodeWriter<SKBitmap>
        {
            Format = BarcodeFormat.DATA_MATRIX,
            Options = new EncodingOptions { Width = size, Height = size, Margin = 0, PureBarcode = true },
            Renderer = new SKBitmapRenderer()
        };
        using var bmp = writer.Write(payload);
        using var img = SKImage.FromBitmap(bmp);
        using var data = img.Encode(SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }
}
