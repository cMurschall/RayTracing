using System.Collections;

namespace RayTracing.Obstacles;

internal class HittableList : Hittable, IEnumerable<Hittable>
{
    private readonly List<Hittable> _hittables = new();

    public void Add(Hittable hittable) => _hittables.Add(hittable);
    public void Clear() => _hittables.Clear();



    public override bool Hit(Ray r, Interval interval, out HitRecord rec)
    {
        rec = new HitRecord();
        var hitAnything = false;
        var closestSoFar = interval.Max;


        foreach (var hittable in _hittables)
        {
            if (hittable.Hit(r, Interval.Create(interval.Min, closestSoFar), out var tempRec))
            {
                hitAnything = true;
                closestSoFar = tempRec.T;
                rec = tempRec;
            }
        }
        return hitAnything;
    }

    public IEnumerator<Hittable> GetEnumerator()
    {
        return _hittables.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}