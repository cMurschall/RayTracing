using RayTracing.Obstacles;

namespace RayTracing.Materials;

internal class Metal : Material
{
    public Color Albedo { get; private init; } = Color.Create(0, 0, 0);
    private double _fuzz;

    public static Metal Create(Color albedo, double fuzz)
    {
        return new Metal { Albedo = albedo, _fuzz = fuzz };
    }

    public override bool Scatter(Ray rayIn, HitRecord hitRecord, out Color attenuation, out Ray scattered)
    {
        var reflected = Color.Reflect(rayIn.Direction, hitRecord.Normal);
        reflected = Color.UnitVector(reflected) + _fuzz * Color.RandomUnitVector();

        scattered = Ray.Create(hitRecord.Point, reflected);
        attenuation = Albedo;
        return Color.Dot(scattered.Direction, hitRecord.Normal) > 0;
    }
}