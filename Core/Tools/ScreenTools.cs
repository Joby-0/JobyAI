using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Tools;

public class ScreenTools
{
    private const int SM_CXSCREEN = 0;
    private const int SM_CYSCREEN = 1;

    [DllImport("user32.dll")]
    private static extern int GetSystemMetrics(int nIndex);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(out Point point);

    [Description(
        "Takes a screenshot of the user's primary monitor. " +
        "Use this when you need to see what is currently displayed on the screen."
    )]
    public string TakeScreenshot()
    {
        try
        {
            var width = GetSystemMetrics(SM_CXSCREEN);
            var height = GetSystemMetrics(SM_CYSCREEN);

            if (width <= 0 || height <= 0)
                return "Could not determine screen size.";

            var path = Path.Combine(
                Path.GetTempPath(),
                "agent-screen.png");

            using var bitmap = new Bitmap(width, height);

            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.CopyFromScreen(
                    0,
                    0,
                    0,
                    0,
                    new Size(width, height));
            }

            bitmap.Save(path, ImageFormat.Png);

            return $"Screenshot saved to: {path}";
        }
        catch (Exception ex)
        {
            return $"Could not take screenshot: {ex.Message}";
        }
    }


    [Description("Returns the width and height of the user's primary monitor in pixels.")]
    public string GetScreenSize()
    {
        try
        {
            var width = GetSystemMetrics(SM_CXSCREEN);
            var height = GetSystemMetrics(SM_CYSCREEN);

            if (width <= 0 || height <= 0)
                return "Could not determine screen size.";

            return $"Screen size: {width}x{height} pixels.";
        }
        catch (Exception ex)
        {
            return $"Could not determine screen size: {ex.Message}";
        }
    }

    [Description("Returns the current mouse cursor position on the user's primary screen in pixels.")]
    public string GetMousePosition()
    {
        try
        {
            if (!GetCursorPos(out var point))
                return "Could not determine mouse position.";

            return $"Mouse position: ({point.X}, {point.Y}) pixels.";
        }
        catch (Exception ex)
        {
            return $"Could not determine mouse position: {ex.Message}";
        }
    }

}