namespace RayTracing;

internal class Ray
{
    private Ray()
    {
    }

    public static Ray Create(Point3 origin, Vector3 direction) => new() { Origin = origin, Direction = direction };

    public Point3 At(double t) => Origin + t * Direction;

    public Vector3 Direction { get; set; } = Vector3.Create(0,0,0);

    public Point3 Origin { get; set; } = Point3.Create(0, 0, 0);
}