using System.Numerics;

namespace no.hvl.DAT154.V23.GROUP14.SpaceModel;

public class Entity {
    
    public readonly string name;
    public Orbit? orbit;
    
    public int satallite_amount = 0;

    public Type? type;

    public Entity(string name) {
        this.name = name;
    }
    
    public Entity(string name, Orbit? orbit) {
        this.name = name;
        this.orbit = orbit;
    }

    public string getName() {
        return name;
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