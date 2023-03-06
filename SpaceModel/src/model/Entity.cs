using System.Numerics;

namespace no.hvl.DAT154.V23.GROUP14.SpaceModel;

public class Entity {
    
    public readonly string name;
    internal Model model;
    private double cacheTime;

    public Orbit? orbit;
    private Vector3 _position = Vector3.Zero;

    public Type? type;

    public Vector3 position {
        get => getPosition();
        set => _position = value;
    }

    public Entity(string name) {
        this.name = name;
        _position = position;
    }
    
    public Entity(string name, Orbit? orbit) {
        this.name = name;
        this.orbit = orbit;
        _position = position;
    }

    public string getName() {
        return name;
    }

    private Vector3 getPosition() {
        if (model == null)
            return Vector3.Zero;

        if (cacheTime == model.time)
            return _position;
        cacheTime = model.time;
        
        if (!(orbit is Orbit o)) 
            return _position;

        float theta = (o.period > 0.0) ? (2.0f * float.Pi * model.time / o.period) : 0.0f;

        _position = (o.origin?.getPosition() ?? Vector3.Zero);
        _position += new Vector3{ X = float.Sqrt(o.distance + 1.0f) * float.Sin(theta), Y = float.Sqrt(o.distance + 1.0f) * float.Cos(theta), Z = 0.0f } / 4.0f;

        return _position;
    }
}

public struct Orbit {
    public Entity origin;
    public float distance;
    public float period;
}

public enum Type {
    STAR,
    GASGIANT,
    TERRESTIAL,
}