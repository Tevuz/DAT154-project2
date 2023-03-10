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

        const int column_name = 0;
        const int column_orbits = 1;
        const int column_distance = 2;
        const int column_period = 3;
        const int column_radius = 4;
        const int column_color = 5;
        const int column_type = 6;

        {
            // ignore first title line
            string[] row = parser.ReadFields();
        }

        while (!parser.EndOfData) {
            string[] row = parser.ReadFields();

            if (string.IsNullOrEmpty(row[column_name])) 
                continue;

            Entity entity = new Entity(row[column_name]);
            
            entity.orbit = Orbit.Of(
                model.findObjectByName(row[column_orbits]), 
                float.TryParse(row[column_distance], out float distance) ? distance : 0.0f, 
                float.TryParse(row[column_period], out float period) ? period : 0.0f);
            
            entity.radius = float.TryParse(row[column_radius], out float row4) ? row4 : 1.0f;
            
            entity.color = row[column_color];
            
            entity.type = Enum.TryParse(row[column_type], out Type type) ? type : null;

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