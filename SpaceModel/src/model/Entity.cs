using System.Numerics;

namespace no.hvl.DAT154.V23.GROUP14.SpaceModel;

public class Entity {
    
    public readonly string name;
    internal Model? model;
    private double cacheTime;


    public Orbit? orbit;
    private Vector3 _position = Vector3.Zero;
    
    public int satallite_amount = 0;

    public Type? type;

    public Vector3 position {
        get => getAbsolutePosition();
        set => _position = value;
    }

    public Entity(string name) {
        this.name = name;
        getAbsolutePosition();
    }
    
    public Entity(string name, Orbit? orbit) {
        this.name = name;
        this.orbit = orbit;
        getAbsolutePosition();
    }

    public string getName() {
        return name;
    }

    public Vector3 getAbsolutePosition() {
        if (model == null)
            return Vector3.Zero;

        if (cacheTime == model.time)
            return _position;
        cacheTime = model.time;
        
        if (!(orbit is Orbit o)) 
            return _position;

        float theta = (o.period > 0.0) ? (2.0f * float.Pi * model.time / o.period) : 0.0f;

        _position = (o.origin?.getAbsolutePosition() ?? Vector3.Zero);
        _position += o.distance * new Vector3{ X = float.Sin(theta), Y = float.Cos(theta), Z = 0.0f };

        return _position;
    }

    public Vector3 getRelativePosition() {
        if (model == null)
            return Vector3.Zero;
        
        if (!(orbit is Orbit o))
            return Vector3.Zero;

        float theta = (o.period > 0.0) ? (2.0f * float.Pi * model.time / o.period) : 0.0f;

        return o.distance * new Vector3() { X = float.Sin(theta), Y = float.Cos(theta), Z = 0.0f };
    }
}

public struct Orbit {
    public Entity origin;
    public float distance;
    public float period;
    public int index;
}

public enum Type {
    STAR,
    GAS,
    HABITABLE,
    ROCK,
    ICE
}