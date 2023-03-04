using System.Collections.ObjectModel;
using no.hvl.DAT154.V23.GROUP14.SpaceModel.graphics;
using no.hvl.DAT154.V23.GROUP14.SpaceModel.model;

namespace no.hvl.DAT154.V23.GROUP14.SpaceModel;

public class Model
{
    
    private Dictionary<string, StellarBody> objects;
    
    public Model()
    {
        objects = new Dictionary<string, StellarBody>();
    }
    
    public static Model LoadFromFile(string filename)
    {
        // TODO: implement loading Excel (.xlsx) or comma-separated values (.txt)
        throw new NotImplementedException();
    }

    public void add(StellarBody element)
    {
        objects.Add(element.name, element);
    }

    public bool remove(StellarBody element)
    {
        return objects.Remove(element.name);
    }

    public StellarBody? find(string name)
    {
        return objects.TryGetValue(name, out var value) ? value : null;
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