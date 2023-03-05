using System.Numerics;

namespace no.hvl.DAT154.V23.GROUP14.SpaceModel;

public abstract class StellarBody
{
    internal Model model { get; set; }
    public string name { get; set; }
    
    private Orbit? _orbit;
    public Orbit? orbit { get; set; }

    private Vector3 _position;
    public Vector3 position
    {
        get => getPosition(model.time);
        set => _position = value;
    }

    private float cacheTime;
    private Vector3 getPosition(float time)
    {
        if (_orbit == null) 
            return _position;

        if (cacheTime == time)
            return _position;

        cacheTime = time;

            var o = _orbit.Value;
        _position = o.origin?.getPosition(time) ?? Vector3.Zero;
        
        var theta = 2.0f * float.Pi * time / o.period;
        _position.X += o.radius * float.Sin(theta);
        _position.Y += o.radius * float.Cos(theta);

        return _position;
    }

}

public struct Orbit
{
    public StellarBody? origin;
    public float radius;
    public float period;
}