using no.hvl.DAT154.V23.GROUP14.SpaceModel.graphics;

namespace no.hvl.DAT154.V23.GROUP14.SpaceModel.model;

public abstract class StellarBody
{
    protected StellarBody parent;

    protected double orbital_radius;
    protected double orbital_period;

    public abstract void render(GraphicsAPI graphics, long time);
}