using RayTracing.Obstacles;

namespace RayTracing.Materials;

internal class Dielectric : Material
{
    // Refractive index in vacuum or air, or the ratio of the material's refractive index over
    // the refractive index of the enclosing media
    private double _refractionIndex;

    public static Dielectric Create(double refractionIndex)
    {
        return new Dielectric { _refractionIndex = refractionIndex };
    }


    public override bool Scatter(Ray rayIn, HitRecord rec, out Color attenuation, out Ray scattered)
    {
        attenuation = Color.Create(1, 1, 1);
        var ri = rec.IsFrontFace ? (1.0 / _refractionIndex) : _refractionIndex;

        var unitDirection = Vector3.UnitVector(rayIn.Direction);


        var cosTheta = Math.Min(Vector3.Dot(-unitDirection, rec.Normal), 1.0);
        var sinTheta = Math.Sqrt(1.0 - cosTheta * cosTheta);

        var cannotRefract = ri * sinTheta > 1.0;
        // Schlick Approximation
        var reflectProbability = Reflectance(cosTheta, ri) > MathHelper.RandomDouble();
        var  direction = cannotRefract || reflectProbability
            ? Vector3.Reflect(unitDirection, rec.Normal)
            : Vector3.Refract(unitDirection, rec.Normal, ri);

        scattered = Ray.Create(rec.Point, direction);
        return true;

    }


    private  double Reflectance(double cosine, double refractionIndex)
    {
        // Use Schlick's approximation for reflectance.
        var r0 = (1 - refractionIndex) / (1 + refractionIndex);
        r0 *= r0;
        return r0 + (1 - r0) * Math.Pow(1 - cosine, 5);
    }
}