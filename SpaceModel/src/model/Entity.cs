using no.hvl.DAT154.V23.GROUP14.SpaceModel.graphics;

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

    public void render(GraphicsAPI graphics, long time) {
        Vector center = new() {
            x = double.Sin(2.0 * double.Pi * time / orbital_period) * orbital_radius,
            y = double.Cos(2.0 * double.Pi * time / orbital_period) * orbital_radius,
            z = 0.0
        };

        Properties properties = new() {
            fillColor = new Color { r = 255, g = 255, b = 255 }
        };

        // graphics.drawCircle(center, radius, properties);
    }

    public void setParent(Entity parent) {
        this.parent = parent;
    }

    public string getName() {
        return name;
    }
}