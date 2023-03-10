using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;

namespace no.hvl.DAT154.V23.GROUP14.SpaceModel;

public class Model {
    private readonly Dictionary<string, Entity> objects;
    
    internal float time;

    public Model() {
        objects = new Dictionary<string, Entity>();
    }

    public static Model LoadFromFile(string filename) {
        Model model = new();

        TextFieldParser parser = new(filename);
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(";");

        while (!parser.EndOfData) {
            string[] row = parser.ReadFields();

            string name = row[0];

            // ignore first title line
            if (string.Equals(name, "Name")) 
                continue;

            if (string.IsNullOrEmpty(name)) 
                continue;

            Entity entity = new Entity(name);
            entity.radius = float.TryParse(row[4], out float row4) ? row4 : 1.0f;
            entity.color = row[5];
            entity.orbit = Orbit.Of(
                model.findObjectByName(row[1]), 
                float.TryParse(row[2], out float row2) ? row2 : 0.0f, 
                float.TryParse(row[3], out float row3) ? row3 : 0.0f);
            entity.type = Enum.TryParse(row[6], out Type type) ? type : null;

            if (!model.addObject(entity)) 
                throw new InvalidOperationException("Possible duplicate in csv file! Could not parse the file!");
            
            Debug.WriteLine(row[0]);
        }

        return model;
    }

    public void ForEach(Action<Entity> action) {
        foreach (Entity entity in objects.Values) {
            action(entity);
        }
    }
    
    public bool addObject(Entity entity) {
        return objects.TryAdd(entity.name, entity);
    }

    public bool removeObject(Entity entity) {
        return objects.Remove(entity.name);
    }

    public Entity? findObjectByName(string name) {
        return objects.TryGetValue(name, out Entity? entity) ? entity : null;
    }
}