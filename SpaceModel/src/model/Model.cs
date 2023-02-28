using System.Collections.ObjectModel;
using no.hvl.DAT154.V23.GROUP14.SpaceModel.graphics;
using no.hvl.DAT154.V23.GROUP14.SpaceModel.model;

namespace no.hvl.DAT154.V23.GROUP14.SpaceModel;

public class Model
{

    public Model()
    {
        objects = new Collection<StellarBody>();
    }
    
    public static Model LoadFromFile(string filename)
    {
        // TODO: implement loading Excel (.xlsx) or comma-separated values (.txt)
        throw new NotImplementedException();
    }
    
    private Collection<StellarBody> objects;

    public void addObject(StellarBody body)
    {
        objects.Add(body);
    }

    public void render(GraphicsAPI graphics, long time, Action<StellarBody, Exception>? onException)
    {
        foreach (var body in objects)
        {
            try
            {
                body.render(graphics, time);
            }
            catch (Exception e)
            {
                onException?.Invoke(body, e);
            }
        }
    }
}