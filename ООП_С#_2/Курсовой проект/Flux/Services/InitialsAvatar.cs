using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Flux.Services;

/// <summary>
/// Строит аватар-плашку с буквой на лету:
/// - Фон: цвет, детерминированно вычисляемый по никнейму.
/// - Передний план: первая буква никнейма, в верхнем регистре.
/// Кешируется в памяти, чтобы повторные запросы выполнялись мгновенно.
/// </summary>
public static class InitialsAvatar
{
    private static readonly ConcurrentDictionary<string, ImageSource> _cache = new();

    public static ImageSource Generate(string? nickname, int size = 128)
    {
        var key = $"{nickname ?? ""}|{size}";
        return _cache.GetOrAdd(key, _ => Render(nickname, size));
    }

    private static ImageSource Render(string? nickname, int size)
    {
        string letter = string.IsNullOrWhiteSpace(nickname)
            ? "?"
            : char.ToUpperInvariant(nickname.Trim()[0]).ToString();

        var bg = Color.FromRgb(0x1A, 0x1A, 0x1A);
        var fg = Color.FromRgb(0xFF, 0xCC, 0x00);

        var dv = new DrawingVisual();
        using (var ctx = dv.RenderOpen())
        {
            ctx.DrawEllipse(new SolidColorBrush(bg), null,
                new Point(size / 2.0, size / 2.0), size / 2.0, size / 2.0);

            var typeface = new Typeface(new FontFamily("Segoe UI"),
                FontStyles.Normal, FontWeights.Bold, FontStretches.Normal);
            var formatted = new FormattedText(
                letter,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                typeface,
                size * 0.55,
                new SolidColorBrush(fg),
                VisualTreeHelper.GetDpi(dv).PixelsPerDip);

            var origin = new Point(
                (size - formatted.Width) / 2,
                (size - formatted.Height) / 2);
            ctx.DrawText(formatted, origin);
        }

        var rtb = new RenderTargetBitmap(size, size, 96, 96, PixelFormats.Pbgra32);
        rtb.Render(dv);
        rtb.Freeze();
        return rtb;
    }

    public static ImageSource? LoadFromFile(string? path)
    {
        if (string.IsNullOrWhiteSpace(path) || !File.Exists(path)) return null;
        try
        {
            var bmp = new BitmapImage();
            bmp.BeginInit();
            bmp.CacheOption = BitmapCacheOption.OnLoad;    
            bmp.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
            bmp.UriSource = new Uri(path, UriKind.Absolute);
            bmp.EndInit();
            bmp.Freeze();
            return bmp;
        }
        catch
        {
            return null;
        }
    }
}

public class AvatarImageConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object? parameter, CultureInfo culture)
    {
        byte[]? bytes = null;
        string? path = null;
        string? nick = null;

        if (values.Length > 0)
        {
            if (values[0] is byte[] b) bytes = b;
            else if (values[0] is string sp) path = sp;
        }
        if (values.Length > 1 && values[1] is string n) nick = n;

        int size = 128;
        if (parameter is string s && int.TryParse(s, out var parsed)) size = parsed;


        if (bytes != null && bytes.Length > 0)
        {
            try
            {
                using var ms = new System.IO.MemoryStream(bytes);
                var bmp = new System.Windows.Media.Imaging.BitmapImage();
                bmp.BeginInit();
                bmp.CacheOption = System.Windows.Media.Imaging.BitmapCacheOption.OnLoad;
                bmp.StreamSource = ms;
                bmp.EndInit();
                bmp.Freeze();
                return bmp;
            }
            catch { }
        }

        var fromFile = InitialsAvatar.LoadFromFile(path);
        if (fromFile != null) return fromFile;

        return InitialsAvatar.Generate(nick, size);
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}
