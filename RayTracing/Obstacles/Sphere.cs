using RayTracing.Materials;

namespace RayTracing.Obstacles;

internal class Sphere(Point3 center, double radius, Material material) : Hittable
{

    public override bool Hit(Ray r, Interval interval, out HitRecord rec)
    {
        rec = new HitRecord();

        var oc = center - r.Origin;
        var a = r.Direction.LengthSquared();
        var h = Color.Dot(r.Direction, oc);
        var c = oc.LengthSquared() - radius * radius;

        var discriminant = h * h - a * c;
        if (discriminant < 0)
        {
            return false;
        }


        var sqrtd = Math.Sqrt(discriminant);

        // Find the nearest root that lies in the acceptable range.
        var root = (h - sqrtd) / a;
        if (!interval.Surrounds(root))
        {
            root = (h + sqrtd) / a;
            if (!interval.Surrounds(root))
                return false;
        }

        rec.T = root;
        rec.Point = r.At(rec.T);
        var outwardNormal = (rec.Point - center) / radius;
        rec.SetFaceNormal(r, outwardNormal);
        rec.Material = material;


        return true;
    }
}