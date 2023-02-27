using System.Security.Cryptography;

namespace DAT154_project2.model;

public abstract class StellarObject
{
    public string name;
    public StellarObject parent;

    public double OrbitDistance;
    public double OrbitalPeriod;

    public void render()
    {
        
    }

}