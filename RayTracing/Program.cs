global using Point3 = RayTracing.Vector3;
global using Color = RayTracing.Vector3;
using System.Drawing;
using RayTracing.Materials;
using System.Diagnostics.Metrics;


namespace RayTracing;

internal class Program
{


    static void Main(string[] args)
    {
        RenderRelease();


    }

    static void RenderRelease()
    {

        var materialGround = Lambertian.Create(Color.Create(0.5, 0.5, 0.5));
        var world = new HittableList
        {
            new Sphere(Point3.Create(0, -1000, 0), 1000.0, materialGround)
        };
        for (var a = -11; a < 11; a++)
        {
            for (var b = -11; b < 11; b++)
            {
                var chooseMat = MathHelper.RandomDouble();
                var center = Point3.Create(a + 0.9 * MathHelper.RandomDouble(), 0.2,
                    b + 0.9 * MathHelper.RandomDouble());

                if ((center - Point3.Create(4, 0.2, 0)).Length() > 0.9)
                {
                    if (chooseMat < 0.8)
                    {
                        // diffuse
                        var albedo = Color.Random() * Color.Random();
                        var sphereMaterial = Lambertian.Create(albedo);
                        world.Add(new Sphere(center, 0.2, sphereMaterial));
                    }
                    else if (chooseMat < 0.95)
                    {
                        // metal
                        var albedo = Color.Random(0.5, 1);
                        var fuzz = MathHelper.RandomDouble(0, 0.5);
                        var sphereMaterial = Metal.Create(albedo, fuzz);
                        world.Add(new Sphere(center, 0.2, sphereMaterial));
                    }
                    else
                    {
                        // glass
                        var sphereMaterial = Dielectric.Create(1.5);
                        world.Add(new Sphere(center, 0.2, sphereMaterial));
                    }
                }
            }
        }

        var material1 = Dielectric.Create(1.5);
        world.Add(new Sphere(Point3.Create(0, 1, 0), 1.0, material1));

        var material2 = Lambertian.Create(Color.Create(0.4, 0.2, 0.1));
        world.Add(new Sphere(Point3.Create(-4, 1, 0), 1.0, material2));

        var material3 = Metal.Create(Color.Create(0.7, 0.6, 0.5), 0.0);
        world.Add(new Sphere(Point3.Create(4, 1, 0), 1.0, material3));



        var camera = new Camera
        {
            AspectRatio = 16.0 / 9.0,
            ImageWidth = 1200,
            SamplesPerPixel = 500,
            MaxDepth = 50,

            VerticalViewAngle = 20,
            LookFrom = Point3.Create(13, 2, 3),
            LookAt = Point3.Create(0, 0, 0),
            UpDirection = Vector3.Create(0, 1, 0),

            DeFocusAngle = 0.6,
            FocusDistance = 10.0,

        };


        camera.Render(world);
    }

    static void RenderDebug()
    {
        // World
        var materialGround = Lambertian.Create(Color.Create(0.8, 0.8, 0.0));
        var materialCenter = Lambertian.Create(Color.Create(0.1, 0.2, 0.5));
        var materialLeft = Dielectric.Create(1.5);
        var materialBubble = Dielectric.Create(1.00 / 1.50);
        var materialRight = Metal.Create(Color.Create(0.8, 0.6, 0.2), 1.0);

        var world = new HittableList
        {
            new Sphere(Point3.Create(0.0, -100.5, -1.0), 100.0, materialGround),
            new Sphere(Point3.Create(0.0, 0.0, -1.2), 0.5, materialCenter),
            new Sphere(Point3.Create(-1.0, 0.0, -1.0), 0.5, materialLeft),
            new Sphere(Point3.Create(-1.0, 0.0, -1.0), 0.4, materialBubble),
            new Sphere(Point3.Create(1.0, 0.0, -1.0), 0.5, materialRight)
        };

        var camera = new Camera
        {
            ImageWidth = 900,
            AspectRatio = 16.0 / 9.0,
            SamplesPerPixel = 100,

            MaxDepth = 50,

            VerticalViewAngle = 20,
            LookFrom = Vector3.Create(-2, 2, 1),
            LookAt = Vector3.Create(0, 0, -1),
            UpDirection = Vector3.Create(0, 1, 0),


            DeFocusAngle = 10.0,
            FocusDistance = 3.4

        };
        camera.Render(world);
    }
}