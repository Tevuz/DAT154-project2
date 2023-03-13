using no.hvl.DAT154.V23.GROUP14.SpaceModel.math;

namespace no.hvl.DAT154.V23.GROUP14.SpaceModel;

public class Entity {
    
    public readonly string Name;
    public double Radius;
    public string Color;

    public Orbit? Orbit;

    public Type? Type;

    public Vector3d Position;

    public Entity(string name) {
        Name = name;
        Color = "white";
        Position = Vector3d.ZERO;
    }

    public void Update(double time) {
        Position = Vector3d.ZERO;
        if (this.Orbit is not Orbit orbit)
            return;
        
        Position = orbit.Origin.Position;
        double theta = double.Tau * time / orbit.Period;
        Position += new Vector3d(double.Cos(theta), double.Sin(theta), 0.0) * orbit.Distance;
    }
}

public struct Orbit {
    public Entity Origin;
    public double Distance;
    public double Period;
    
    public static Orbit? Of(Entity? origin, double distance, double period) {
        if (origin == null)
            return null;

        return new Orbit() {
            Origin = origin,
            Distance = distance,
            Period = period
        };
    }
}

public enum Type {
    Star,
    Gas,
    Habitable,
    Rock,
    Ice
}