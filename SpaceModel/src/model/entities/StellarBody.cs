﻿namespace no.hvl.DAT154.V23.GROUP14.SpaceModel.model;

public abstract class StellarBody
{
    public string name;
    
    protected StellarBody parent;

    protected double orbital_radius;
    protected double orbital_period;
}