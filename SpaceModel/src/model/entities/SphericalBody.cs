using no.hvl.DAT154.V23.GROUP14.SpaceModel.graphics;

namespace no.hvl.DAT154.V23.GROUP14.SpaceModel.model;

public class SphericalBody : StellarBody
{
    private string name;
    // private double radius;
    
    // TODO: Constructor
    public SphericalBody(string name, double orbital_radius, double orbital_period) {
        this.name = name;
        // this.radius = radius;
        this.orbital_radius = orbital_radius;
        this.orbital_period = orbital_period;
    }

    public void setParent(StellarBody parent) {
        this.parent = parent;
    }

    public override string getName() {
        return name;
    }

    public override void render(GraphicsAPI graphics, long time)
    {
        var center = new Vector()
        {
            x = double.Sin(2.0 * double.Pi * time / orbital_period) * orbital_radius, 
            y = double.Cos(2.0 * double.Pi * time / orbital_period) * orbital_radius, 
            z = 0.0
        };
        
        var properties = new Properties()
        {
            fillColor = new Color() { r = 255, g = 255, b = 255 }
        };
        
        // graphics.drawCircle(center, radius, properties);
    }
}