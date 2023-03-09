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
        parser.SetDelimiters(",");

        Dictionary<string, int> fields = new Dictionary<string, int> {
            { "name", -1 },
            { "type", -1 },
            { "orbits", -1 },
            { "orbital_distance", -1 },
            { "orbital_period", -1 }
        };

        {
            string[] row = parser.ReadFields();
            foreach (string key in fields.Keys) {
                fields[key] = Array.IndexOf(row, key);
            }
        }

        while (!parser.EndOfData) {
            string[] row = parser.ReadFields();

            if (string.IsNullOrEmpty(row[fields["name"]])) 
                continue;

            Entity entity = new(row[fields["name"]]);

            entity.type = fields["type"] >= 0 && Enum.TryParse(row[fields["type"]].ToUpper(), out Type type) ? type : Type.ROCK;

            Entity? orbits = fields["orbits"] >= 0 ? model.findObjectByName(row[fields["orbits"]]) : null;
            if (orbits != null) {
                entity.orbit = new Orbit() {
                    origin = orbits,
                    distance = fields["orbital_distance"] >= 0 && float.TryParse(row[fields["orbital_distance"]], out float distance) ? distance : 0,
                    period = fields["orbital_period"] >= 0 && float.TryParse(row[fields["orbital_period"]], out float period) ? period : 0,
                    index = orbits.satallite_amount++
                };
            }

            if (!model.addObject(entity)) 
                throw new InvalidOperationException("Possible duplicate in csv file! Could not parse the file!");
            
            Debug.WriteLine(row[0]);
        }

        return model;
    }
    
    public void OnTick(float time) {
        this.time = time;
    }

    public void ForEach(Action<Entity> action) {
        foreach (Entity entity in objects.Values) {
            action(entity);
        }
    }
    
    public bool addObject(Entity entity) {
        entity.model = this;
        return objects.TryAdd(entity.getName(), entity);
    }

    public bool removeObject(Entity entity) {
        return objects.Remove(entity.getName());
    }

    public Entity? findObjectByName(string name) {
        return objects.TryGetValue(name, out Entity? entity) ? entity : null;
    }
}