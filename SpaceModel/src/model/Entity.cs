using System.Numerics;

namespace no.hvl.DAT154.V23.GROUP14.SpaceModel;

public class Entity {
    
    public readonly string name;
    internal Model model;
    private double cacheTime;

    public Orbit? orbit;
    private Vector3 _position;

    public Vector3 position {
        get => getPosition(model.time);
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

    private Vector3 getPosition(float time) {

        if (cacheTime == time)
            return _position;
        cacheTime = time;
        
        if (orbit == null) 
            return _position;
        Orbit o = orbit.Value;

        float theta = 2.0f * float.Pi * time / o.period;
        
        position = o.origin?.getPosition(time) ?? Vector3.Zero + new Vector3() { X = o.distance * float.Sin(theta), Y = o.distance * float.Cos(theta), Z = 0.0f };

        return position;
    }
}

public struct Orbit {
    public Entity origin;
    public float distance;
    public float period;
}