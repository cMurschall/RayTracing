using RayTracing.Obstacles;

namespace RayTracing.Materials;

internal abstract class Material
{
    public virtual bool Scatter(Ray rayIn, HitRecord hitRecord, out Color attenuation, out Ray scattered)
    {
        attenuation = Color.Create(1, 1, 1);
        scattered = Ray.Create(Point3.Create(0, 0, 0), Color.Create(0, 0, 0));
        return false;
    }

}