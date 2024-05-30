using RayTracing.Materials;

namespace RayTracing.Obstacles;

internal class HitRecord
{
    public Point3 Point { get; set; } = Color.Create(0, 0, 0);
    public Color Normal { get; set; } = Color.Create(0, 0, 0);
    public double T { get; set; }

    public Material Material { get; set; } = Lambertian.Create(Color.Create(0, 0, 0));



    public bool IsFrontFace;

    public void SetFaceNormal(Ray r, Color outwardNormal)
    {
        // Sets the hit record normal vector.
        // NOTE: the parameter `outward_normal` is assumed to have unit length.

        IsFrontFace = Color.Dot(r.Direction, outwardNormal) < 0;
        Normal = IsFrontFace ? outwardNormal : -outwardNormal;
    }
}