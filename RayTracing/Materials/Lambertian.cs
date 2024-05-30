using RayTracing.Obstacles;

namespace RayTracing.Materials;

internal class Lambertian : Material
{
    public Color Albedo { get; private init; } = Color.Create(0, 0, 0);

    private Lambertian()
    {
    }


    public static Lambertian Create(Color albedo)
    {
        return new Lambertian { Albedo = albedo };
    }

    public override bool Scatter(Ray rayIn, HitRecord hitRecord, out Color attenuation, out Ray scattered)
    {
        var scatterDirection = hitRecord.Normal + Color.RandomUnitVector();
        // Catch degenerate scatter direction
        if (scatterDirection.IsNearZero())
        {
            scatterDirection = hitRecord.Normal;
        }

        scattered = Ray.Create(hitRecord.Point, scatterDirection);
        attenuation = Albedo;
        return true;
    }
}