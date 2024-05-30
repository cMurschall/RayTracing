using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using RayTracing.Obstacles;

namespace RayTracing;

internal class Camera
{
    private readonly object _lock = new();
    private int _imageHeight; // Rendered image height
    private Point3 _center = Point3.Create(0, 0, 0);                // Camera center
    private Point3 _pixel00Loc = Point3.Create(0, 0, 0);            // Location of pixel 0, 0
    private Vector3 _pixelDeltaU = Vector3.Create(0, 0, 0);         // Offset to pixel to the right
    private Vector3 _pixelDeltaV = Vector3.Create(0, 0, 0);         // Offset to pixel below
    private double _pixelSamplesScale;                              // Color scale factor for a sum of pixel samples

    private Vector3 _u = Vector3.Create(0, 0, 0);                   // Camera frame basis vectors
    private Vector3 _v = Vector3.Create(0, 0, 0);                   // Camera frame basis vectors
    private Vector3 _w = Vector3.Create(0, 0, 0);                   // Camera frame basis vectors
    private Vector3 _deFocusDiskU = Vector3.Create(0, 0, 0);        // De-focus disk horizontal radius
    private Vector3 _deFocusDiskV = Vector3.Create(0, 0, 0);        // De-focus disk vertical radius

    // public properties
    public double AspectRatio { get; set; } = 1.0;                  // Ratio of image width over height
    public int ImageWidth { get; set; } = 900;                      // Rendered image width in pixel count
    public int SamplesPerPixel { get; set; } = 10;                  // Count of random samples for each pixel
    public int MaxDepth { get; set; } = 50;                         // Maximum depth of ray recursion
    public double VerticalViewAngle { get; set; } = 90;             // Vertical view angle (field of view)
    public Point3 LookFrom { get; set; } = Point3.Create(0, 0, 0);  // Point camera is looking from
    public Point3 LookAt { get; set; } = Point3.Create(0, 0, -1);   // Point camera is looking at
    public Vector3 UpDirection { get; set; } = Vector3.Create(0, 1, 0);  // Camera-relative "up" direction
    public double DeFocusAngle { get; set; } = 0;                   // Variation angle of rays through each pixel
    public double FocusDistance { get; set; } = 10;                 // Distance from camera look from point to plane of perfect focus



    public void Render(Hittable world)
    {
        Initialize();
        var imageMap = new Color[ImageWidth, _imageHeight];

        // initalize image map with white
        for (var i = 0; i < ImageWidth; i++)
        {
            for (var j = 0; j < _imageHeight; j++)
            {
                imageMap[i, j] = Color.Create(1, 1, 1);
            }
        }

        var options = new ParallelOptions { MaxDegreeOfParallelism = Environment.ProcessorCount };
        Console.WriteLine("Rendering...");
        var consoleTop = Console.CursorTop;

        double total = _imageHeight * ImageWidth;
        var remaining = (int)total;

        var startTime = DateTime.Now;

        var imageIndex = 0;

        //Parallel.For(0, _imageHeight, options, j =>
        var jRange = Enumerable.Range(0, _imageHeight).Shuffle().ToList();
        var iRange = Enumerable.Range(0, ImageWidth).Shuffle().ToList();
        var ij = jRange.SelectMany(j => iRange.Select(i => (i, j))).Shuffle().ToList();


        Parallel.ForEach(ij, options, x =>
        {
            var (i, j) = x;
            // Progress logging
            remaining = Interlocked.Decrement(ref remaining);
            //if (remaining % 10 == 0)
            //{
            //    lock (_lock)
            //    {
            //        // guess the remaining time
            //        var elapsed = DateTime.Now - startTime;
            //        var remainingTime = elapsed.TotalMinutes/ (total - remaining) * remaining;

            //        // clear console line
            //        Console.SetCursorPosition(0, consoleTop);
            //        // write progress
            //        Console.Write($"Progress: {(1 - remaining / total):P} - ({remaining} pixels to go. {(int)remainingTime} min)");
            //    }
            //}


            // save images with 100 samples

            lock (_lock)
            {
                var samples = (int)(total / 300);
                if (remaining % samples == 0)
                {

                    imageIndex++;
                    SaveToFile(imageMap, imageIndex);

                }
            }

            var pixelColor = Color.Create(0, 0, 0);
            for (var sample = 0; sample < SamplesPerPixel; sample++)
            {
                var r = GetRay(i, j);
                pixelColor += RayColor(r, world, MaxDepth);
            }

            imageMap[i, j] = _pixelSamplesScale * pixelColor;
            if (remaining % 1000 == 0)
            {
                Console.WriteLine($"Progress: {(1 - remaining / total):P} - ({remaining} pixels to go.)");
            }

        });


        Console.WriteLine();
        Console.WriteLine("Done");
        // save image
        SaveToFile(imageMap);
    }


    private void SaveToFile(Color[,] imageMap, int imageIndex = -1)
    {
        var image = new Bitmap(ImageWidth, _imageHeight);
        for (var i = 0; i < image.Width; i++)
        {
            for (var j = 0; j < image.Height; j++)
            {
                image.SetPixel(i, j, ColorHelper.ToDrawingColor(imageMap[i, j]));
            }
        }
        // image index with 4 leading zeros
        var indexString = imageIndex > 0 ? $"_{imageIndex:D4}" : string.Empty;
        image.Save(@$"C:\Users\murschac\Pictures\rayTracing\shuffle\image{indexString}.bmp", ImageFormat.Bmp);
    }

    private Ray GetRay(int i, int j)
    {
        // Construct a camera ray originating from the origin and directed at randomly sampled
        // point around the pixel location i, j.

        var offset = SampleSquare();
        var pixelSample = _pixel00Loc + (i + offset.X()) * _pixelDeltaU + (j + offset.Y()) * _pixelDeltaV;

        var rayOrigin = (DeFocusAngle <= 0) ? _center : defocus_disk_sample(); ;
        var rayDirection = pixelSample - rayOrigin;

        return Ray.Create(rayOrigin, rayDirection);
    }

    private Vector3 SampleSquare()
    {
        // Returns the vector to a random point in the [-.5,-.5]-[+.5,+.5] unit square.
        return Vector3.Create(MathHelper.RandomDouble() - 0.5, MathHelper.RandomDouble() - 0.5, 0);
    }

    private void Initialize()
    {
        _imageHeight = (int)Math.Max(1, (ImageWidth / AspectRatio));

        _center = LookFrom;

        // Determine viewport dimensions.
        //var focalLength = (LookFrom - LookAt).Length();
        var theta = MathHelper.DegreesToRadians(VerticalViewAngle);
        var h = Math.Tan(theta / 2);
        var viewportHeight = 2.0 * h * FocusDistance;
        var viewportWidth = viewportHeight * ((double)ImageWidth / _imageHeight);

        // Calculate the u,v,w unit basis vectors for the camera coordinate frame.
        _w = Vector3.UnitVector(LookFrom - LookAt);
        _u = Vector3.UnitVector(Vector3.Cross(UpDirection, _w));
        _v = Vector3.Cross(_w, _u);


        // Calculate the vectors across the horizontal and down the vertical viewport edges.
        var viewportU = viewportWidth * _u; // Vector across viewport horizontal edge
        var viewportV = viewportHeight * -_v; // Vector down viewport vertical edge

        // Calculate the horizontal and vertical delta vectors from pixel to pixel.
        _pixelDeltaU = viewportU / ImageWidth;
        _pixelDeltaV = viewportV / _imageHeight;

        // Calculate the location of the upper left pixel.
        var viewportUpperLeft = _center - (FocusDistance * _w) - viewportU / 2 - viewportV / 2;
        _pixel00Loc = viewportUpperLeft + 0.5 * (_pixelDeltaU + _pixelDeltaV);

        _pixelSamplesScale = 1.0 / SamplesPerPixel;


        // Calculate the camera defocus disk basis vectors.
        var deFocusRadius = FocusDistance * Math.Tan(MathHelper.DegreesToRadians(DeFocusAngle / 2));
        _deFocusDiskU = _u * deFocusRadius;
        _deFocusDiskV = _v * deFocusRadius;

    }

    private Color RayColor(Ray ray, Hittable obstacle, int depth)
    {
        // If we've exceeded the ray bounce limit, no more light is gathered.
        if (depth <= 0)
        {
            return Color.Create(0, 0, 0);
        }

        if (obstacle.Hit(ray, Interval.Create(0.001, double.PositiveInfinity), out var hitRecord))
        {

            if (hitRecord.Material.Scatter(ray, hitRecord, out var attenuation, out var scattered))
            {
                return attenuation * RayColor(scattered, obstacle, depth - 1);
            }

            return Color.Create(0, 0, 0);
        }

        var unitDirection = Vector3.UnitVector(ray.Direction);
        var a = 0.5 * (unitDirection.Y() + 1.0);
        return (1.0 - a) * Color.Create(1.0, 1.0, 1.0) + a * Color.Create(0.5, 0.7, 1.0);
    }


    private Point3 defocus_disk_sample()
    {
        // Returns a random point in the camera defocus disk.
        var p = Vector3.RandomInUnitDisk();
        return _center + (p[0] * _deFocusDiskU) + (p[1] * _deFocusDiskV);
    }

}
