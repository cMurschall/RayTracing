namespace RayTracing;

public static class MathHelper
{
    public static double DegreesToRadians(double degrees)
    {
        return degrees * Math.PI / 180.0;
    }

    public static double RandomDouble()
    {
        return Random.Shared.NextDouble();
    }

    public static double RandomDouble(double min, double max)
    {
        return min + (max - min) * RandomDouble();
    }


    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        if (source == null)
            throw new ArgumentNullException(nameof(source));

        return source.OrderBy(x => Random.Shared.Next());
    }
}