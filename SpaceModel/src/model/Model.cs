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

            Entity parent = model.findObjectByName(row[1]);

            float orbital_distance;
            if (row[2].Equals("-"))
                orbital_distance = 0;
            else
                orbital_distance = Convert.ToSingle(row[2]);

            float orbital_period;
            if (row[3].Equals("-"))
                orbital_period = 0;
            else
                orbital_period = Convert.ToSingle(row[3]);

            Entity entity = new(name);
            entity.orbit = new Orbit { origin = parent, distance = orbital_distance, period = orbital_period };

            if (!model.addObject(entity)) 
                throw new InvalidOperationException("Possible duplicate in csv file! Could not parse the file!");
            
            Debug.WriteLine(row[0]);
        }

        return model;
    }
    
    public void OnTick(float time)
    {
        this.time = time;
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