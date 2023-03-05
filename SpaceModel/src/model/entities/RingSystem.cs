using System.Collections.ObjectModel;
using no.hvl.DAT154.V23.GROUP14.SpaceModel.graphics;

namespace no.hvl.DAT154.V23.GROUP14.SpaceModel.model;

public class RingSystem : StellarBody {
    private readonly string name;
    private readonly Collection<SphericalBody> ring_objects;

    public RingSystem(string name) {
        ring_objects = new Collection<SphericalBody>();
        this.name = name;
    }

    public override string getName() {
        return name;
    }

    public void addRingObject(SphericalBody body) {
        ring_objects.Add(body);
    }

    public override void render(GraphicsAPI graphics, long time) {
        // TODO: 
        throw new NotImplementedException();
    }
}