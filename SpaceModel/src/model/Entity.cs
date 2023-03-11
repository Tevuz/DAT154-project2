using System.Numerics;

namespace no.hvl.DAT154.V23.GROUP14.SpaceModel;

public class Entity {
    
    public readonly string name;
    public float radius;
    public string color;

    public Orbit? orbit;
    
    public int satallite_amount = 0;

    public Type? type;

    public Vector3 position;

    public Entity(string name) {
        this.name = name;
    }

}

public struct Orbit {
    public Entity origin;
    public float distance;
    public float period;
    public int index;
    
    public static Orbit? Of(Entity? origin, float distance, float period) {
        if (origin == null)
            return null;

        return new Orbit() {
            origin = origin,
            distance = distance,
            period = period,
            index = origin.satallite_amount++
        };
    }
}


public enum Type {
    STAR,
    GAS,
    HABITABLE,
    ROCK,
    ICE
}