using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;

namespace no.hvl.DAT154.V23.GROUP14.SpaceModel;

public class Model {
    private readonly Dictionary<string, Entity> objects;

    public Model() {
        objects = new Dictionary<string, Entity>();
    }

    public static Model LoadFromFile(string filename) {
        Model model = new();

        TextFieldParser parser = new(filename);
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(";");

        const int columnName = 0;
        const int columnOrbits = 1;
        const int columnDistance = 2;
        const int columnPeriod = 3;
        const int columnRadius = 4;
        const int columnColor = 5;
        const int columnType = 6;

        {
            // ignore first title line
            parser.ReadFields();
        }

        while (!parser.EndOfData) {
            string[]? row = parser.ReadFields();

            if (row == null || string.IsNullOrEmpty(row[columnName])) 
                continue;
            

            Entity entity = new Entity(row[columnName]) {
                Orbit = Orbit.Of(
                    model.FindObjectByName(row[columnOrbits]), 
                    double.TryParse(row[columnDistance], out double distance) ? distance : 0.0, 
                    double.TryParse(row[columnPeriod], out double period) ? period : 0.0),
                Radius = double.TryParse(row[columnRadius], out double radius) ? (radius * 0.001) : 1.0,
                Color = row[columnColor],
                Type = Enum.TryParse(row[columnType], out Type type) ? type : null
            };

            if (!model.AddObject(entity)) 
                throw new InvalidOperationException("Possible duplicate in csv file! Could not parse the file!");
            
            Debug.WriteLine(row[0] + " " + type);
        }

        return model;
    }

    public void ForEach(Action<Entity> action) {
        foreach (Entity entity in objects.Values) {
            action(entity);
        }
    }
    
    public bool AddObject(Entity entity) {
        return objects.TryAdd(entity.Name, entity);
    }

    public bool RemoveObject(Entity entity) {
        return objects.Remove(entity.Name);
    }

    public Entity? FindObjectByName(string? name) {
        return objects.TryGetValue(name ?? "", out Entity? entity) ? entity : null;
    }
}