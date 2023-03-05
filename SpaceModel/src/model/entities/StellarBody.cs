using no.hvl.DAT154.V23.GROUP14.SpaceModel.graphics;

namespace no.hvl.DAT154.V23.GROUP14.SpaceModel.model;

public abstract class StellarBody {
    protected double orbital_period;

    protected double orbital_radius;
    protected StellarBody parent;

    public abstract void render(GraphicsAPI graphics, long time);

    public abstract string getName();
}