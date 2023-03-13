﻿using no.hvl.DAT154.V23.GROUP14.SpaceModel.math;

namespace no.hvl.DAT154.V23.GROUP14.SpaceModel;

public class Entity {
    
    public readonly string name;
    public double radius;
    public string color;

    public Orbit? orbit;
    
    public int satallite_amount = 0;

    public Type? type;

    public Vector3d position;

    public Entity(string name) {
        this.name = name;
    }

    public void update(double time) {
        position = Vector3d.ZERO;
        if (this.orbit is not Orbit orbit)
            return;
        
        position = orbit.origin.position;
        double theta = double.Tau * time / orbit.period;
        position += new Vector3d(double.Cos(theta), double.Sin(theta), 0.0) * orbit.distance;
    }
}

public struct Orbit {
    public Entity origin;
    public double distance;
    public double period;
    
    public static Orbit? Of(Entity? origin, double distance, double period) {
        if (origin == null)
            return null;

        return new Orbit() {
            origin = origin,
            distance = distance,
            period = period
        };
    }
}


public enum Type {
    Star,
    Gas,
    Habitable,
    Rock,
    Ice
}