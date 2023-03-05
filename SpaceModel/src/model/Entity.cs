namespace no.hvl.DAT154.V23.GROUP14.SpaceModel;

public class Entity {
    protected readonly string name;
    protected double orbital_period;

    protected double orbital_radius;
    protected Entity parent;

    public Entity(string name, double orbitalRadius, double orbitalPeriod) {
        this.name = name;
        this.orbital_radius = orbitalRadius;
        this.orbital_period = orbitalPeriod;
    }

    public void setParent(Entity parent) {
        this.parent = parent;
    }

    public string getName() {
        return name;
    }
}