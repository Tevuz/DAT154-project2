using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;
using no.hvl.DAT154.V23.GROUP14.SpaceModel.graphics;

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

        while (!parser.EndOfData) {
            string[] row = parser.ReadFields();

            string name = row[0];

            // ignore first title line
            if (string.Equals(name, "Name")) 
                continue;

            if (string.IsNullOrEmpty(name)) 
                continue;

            Entity parent = model.findObjectByName(row[1]);

            double orbital_radius;
            if (row[2].Equals("-"))
                orbital_radius = 0;
            else
                orbital_radius = Convert.ToDouble(row[2]);

            double orbital_period;
            if (row[3].Equals("-"))
                orbital_period = 0;
            else
                orbital_period = Convert.ToDouble(row[3]);

            Entity entity = new(name, orbital_radius, orbital_period);
            entity.setParent(parent);

            if (!model.addObject(entity)) 
                throw new InvalidOperationException("Possible duplicate in csv file! Could not parse the file!");
            
            Debug.WriteLine(row[0]);
        }

        return model;
    }

    public bool addObject(Entity entity) {
        return objects.TryAdd(entity.getName(), entity);
    }

    public bool removeObject(Entity entity) {
        return objects.Remove(entity.getName());
    }

    public Entity? findObjectByName(string name) {
        return objects.TryGetValue(name, out Entity? entity) ? entity : null;
    }

    public void render(GraphicsAPI graphics, long time, Action<Entity, Exception>? onException) {
        foreach (Entity entity in objects)
            try {
                entity.render(graphics, time);
            } catch (Exception e) {
                onException?.Invoke(entity, e);
            }
    }
}