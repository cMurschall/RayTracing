namespace RayTracing.Obstacles;

internal abstract class Hittable
{
    public abstract bool Hit(Ray r, Interval interval, out HitRecord obstacle);
}