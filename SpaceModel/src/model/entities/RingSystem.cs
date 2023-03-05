using no.hvl.DAT154.V23.GROUP14.SpaceModel.graphics;
using System.Collections.ObjectModel;

namespace no.hvl.DAT154.V23.GROUP14.SpaceModel.model;

public class RingSystem : StellarBody
{
    private Collection<SphericalBody> ring_objects;
    private string name;

    public RingSystem(string name) {
        this.ring_objects = new Collection<SphericalBody>();
        this.name = name;
    }

    public override string getName() {
        return name;
    }

    public void addRingObject(SphericalBody body) {
        ring_objects.Add(body);
    }

    public override void render(GraphicsAPI graphics, long time)
    {
        // TODO: 
        throw new NotImplementedException();
    }

}