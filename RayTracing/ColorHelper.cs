namespace RayTracing;

internal static class ColorHelper
{
    public static void WriteColor(this StreamWriter writer, Vector3 pixelColor)
    {

        var r = pixelColor.X();
        var g = pixelColor.Y();
        var b = pixelColor.Z();

        // Translate the [0,1] component values to the byte range [0,255].
        var rByte = (int)(255.999 * r);
        var gByte = (int)(255.999 * g);
        var bByte = (int)(255.999 * b);

        writer.WriteLine($"{rByte} {gByte} {bByte}");
    }

    public static System.Drawing.Color ToDrawingColor(Color pixelColor)
    {
        // Apply a linear to gamma transform for gamma 2
        var r = LinearToGamma(pixelColor.X());
        var g = LinearToGamma(pixelColor.Y());
        var b = LinearToGamma(pixelColor.Z());

        // Translate the [0,1] component values to the byte range [0,255].
        var intensity = Interval.Create(0.000, 0.999);
        var  rByte = (int)(256 * intensity.Clamp(r));
        var  gByte = (int)(256 * intensity.Clamp(g));
        var  bByte = (int)(256 * intensity.Clamp(b));
        return System.Drawing.Color.FromArgb(rByte, gByte, bByte);
    }


    public static double LinearToGamma(double linearComponent)
    {
        return linearComponent > 0 ? Math.Sqrt(linearComponent) : 0d;
    }
}