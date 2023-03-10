using System.Numerics;

namespace no.hvl.DAT154.V23.GROUP14.SpaceModel;

public class Entity {
    
    public readonly string name;
    public float radius;
    public string color;

    public Orbit? orbit;
    
    public int satallite_amount = 0;

    public Type? type;

    public Entity(string name) {
        this.name = name;
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