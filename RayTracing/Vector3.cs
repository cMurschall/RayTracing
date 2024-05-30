using System.Numerics;

namespace RayTracing;

internal class Vector3
{
    public double[] E = new double[3];

    private Vector3()
    {
        E[0] = 0;
        E[1] = 0;
        E[2] = 0;
    }

    public static Vector3 Create(double e0, double e1, double e2)
    {
        var v = new Vector3
        {
            E =
            {
                [0] = e0,
                [1] = e1,
                [2] = e2
            }
        };
        return v;
    }
    public static Vector3 Random() => Create(MathHelper.RandomDouble(), MathHelper.RandomDouble(), MathHelper.RandomDouble());

    public static Vector3 Random(double min, double max) => Create(MathHelper.RandomDouble(min, max), MathHelper.RandomDouble(min, max), MathHelper.RandomDouble(min, max));

    public double X()
    {
        return E[0];
    }

    public double Y()
    {
        return E[1];
    }

    public double Z()
    {
        return E[2];
    }

    public static Vector3 operator -(Vector3 v)
    {
        return Create(-v.E[0], -v.E[1], -v.E[2]);
    }



    public double this[int i]
    {
        get => E[i];
        set => E[i] = value;
    }

    public Vector3 Add(Vector3 v)
    {
        E[0] += v.E[0];
        E[1] += v.E[1];
        E[2] += v.E[2];
        return this;
    }

    public Vector3 Multiply(double t)
    {
        E[0] *= t;
        E[1] *= t;
        E[2] *= t;
        return this;
    }

    public static Vector3 Reflect(Vector3 v, Vector3 n) => v - 2 * Dot(v, n) * n;

    public static Vector3 Refract(Vector3 uv, Vector3 n, double etaiOverEtat)
    {
        var cosTheta = Math.Min(Dot(-uv, n), 1.0);
        var rOutPerpendicular = etaiOverEtat * (uv + cosTheta * n);
        var rOutParallel = -Math.Sqrt(Math.Abs(1.0 - rOutPerpendicular.LengthSquared())) * n;
        return rOutPerpendicular + rOutParallel;
    }

    public Vector3 Divide(double t) => Multiply(1 / t);

    public double Length() => Math.Sqrt(LengthSquared());

    public double LengthSquared() => E[0] * E[0] + E[1] * E[1] + E[2] * E[2];

    public bool IsNearZero()
    {
        // Return true if the vector is close to zero in all dimensions.
        const double s = 1e-8;
        return Math.Abs(E[0]) < s && Math.Abs(E[1]) < s && Math.Abs(E[2]) < s;
    }

    public override string ToString() => $"{E[0]} {E[1]} {E[2]}";

    // Vector Utility Functions

    public static Vector3 operator +(Vector3 u, Vector3 v) =>
        Create(u.E[0] + v.E[0], u.E[1] + v.E[1], u.E[2] + v.E[2]);

    public static Vector3 operator -(Vector3 u, Vector3 v) =>
        Create(u.E[0] - v.E[0], u.E[1] - v.E[1], u.E[2] - v.E[2]);

    public static Vector3 operator *(Vector3 u, Vector3 v) =>
        Create(u.E[0] * v.E[0], u.E[1] * v.E[1], u.E[2] * v.E[2]);

    public static Vector3 operator *(double t, Vector3 v) =>
        Create(t * v.E[0], t * v.E[1], t * v.E[2]);

    public static Vector3 operator *(Vector3 v, double t) => t * v;

    public static Vector3 operator /(Vector3 v, double t) => (1 / t) * v;

    public static double Dot(Vector3 u, Vector3 v) =>
        u.E[0] * v.E[0] + u.E[1] * v.E[1] + u.E[2] * v.E[2];

    public static Vector3 Cross(Vector3 u, Vector3 v) =>
        Create(
            u.E[1] * v.E[2] - u.E[2] * v.E[1],
            u.E[2] * v.E[0] - u.E[0] * v.E[2],
            u.E[0] * v.E[1] - u.E[1] * v.E[0]
        );

    public static Vector3 UnitVector(Vector3 v) => v / v.Length();


    public static Vector3 RandomInUnitDisk()
    {
        while (true)
        {
            var p = Create(MathHelper.RandomDouble(-1, 1), MathHelper.RandomDouble(-1, 1), 0);
            if (p.LengthSquared() < 1)
            {
                return p;
            }
        }
    }

    public static Vector3 RandomUnitVector() => UnitVector(RandomInUnitSphere());

    public static Vector3 RandomInUnitSphere()
    {
        do
        {
            var p = Random(-1, 1);
            if (p.LengthSquared() < 1)
                return p;
        } while (true);
    }


    public static Vector3 RandomOnHemisphere(Vector3 normal)
    {
        var onUnitSphere = RandomUnitVector();
        return Dot(onUnitSphere, normal) > 0.0
            ? onUnitSphere // In the same hemisphere as the normal
            : -onUnitSphere;
    }
}