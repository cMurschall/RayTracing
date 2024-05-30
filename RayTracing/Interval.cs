namespace RayTracing;

internal class Interval
{

    public double Min { get; set; } = double.NegativeInfinity;
    public double Max { get; set; } = double.PositiveInfinity;

    private Interval()
    {
    }
    public static Interval Create(double min, double max)
    {
        var interval = new Interval
        {
            Min = min,
            Max = max
        };
        return interval;
    }

    public double Size => Max - Min;


    public bool Contains(double t) => Min <= t && t <= Max;

    public bool Surrounds(double t) => Min < t && t < Max;

    public double Clamp(double t) => Math.Max(Min, Math.Min(Max, t));


    public static Interval Empty { get; } = Create(double.PositiveInfinity, double.NegativeInfinity);
    public static Interval Universe { get; } = Create(double.NegativeInfinity, double.PositiveInfinity);




}