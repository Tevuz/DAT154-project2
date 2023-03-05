using System.Collections.ObjectModel;
using System.Diagnostics;
using Microsoft.VisualBasic.FileIO;
using no.hvl.DAT154.V23.GROUP14.SpaceModel.graphics;
using no.hvl.DAT154.V23.GROUP14.SpaceModel.model;

namespace no.hvl.DAT154.V23.GROUP14.SpaceModel;

public class Model {
    private readonly Collection<StellarBody> objects;

    public Model() {
        objects = new Collection<StellarBody>();
    }

    public static Model LoadFromFile(string filename) {
        Model model = new();

        TextFieldParser parser = new(filename);
        parser.TextFieldType = FieldType.Delimited;
        parser.SetDelimiters(";");

        var ring_system_counter = 0;
        RingSystem? ring_system = null;

        while (!parser.EndOfData) {
            string[] row = parser.ReadFields();

            string name = row[0];

            // ignore first title line
            if (string.Equals(name, "Name")) 
                continue;

            if (string.IsNullOrEmpty(name)) {
                Debug.WriteLine("\nnew ring system:");
                ring_system = new RingSystem(ring_system_counter.ToString());
                model.addObject(ring_system);
                ring_system_counter++;
                continue;
            }

            StellarBody parent = model.findObjectByName(row[1]);

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

            SphericalBody sb = new(name, orbital_radius, orbital_period);
            sb.setParent(parent);

            if (!model.addObject(sb)) throw new InvalidOperationException("Possible duplicate in csv file! Could not parse the file!");

            if (ring_system != null) ring_system.addRingObject(sb);


            Debug.WriteLine(row[0]);
        }

        return model;
    }

    public bool addObject(StellarBody body) {
        if (objects.Where(o => string.Equals(body.getName(), o.getName())).Count() > 0) 
            return false;

        objects.Add(body);
        return true;
    }

    public bool removeObject(StellarBody body) {
        return objects.Remove(body);
    }

    public StellarBody? findObjectByName(string name) {
        if (objects.Count() == 0) 
            return null;
        return objects.Where(o => string.Equals(name, o.getName())).First();
    }

    public void render(GraphicsAPI graphics, long time, Action<StellarBody, Exception>? onException) {
        foreach (StellarBody body in objects)
            try {
                body.render(graphics, time);
            } catch (Exception e) {
                onException?.Invoke(body, e);
            }
    }
}