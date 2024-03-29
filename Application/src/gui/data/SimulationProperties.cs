﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using no.hvl.DAT154.V23.GROUP14.SpaceModel;

namespace DAT154_project2.gui.data;

public class SimulationProperties : INotifyPropertyChanged {
    private double _timeStep = 1.0;
    public double timeStep { get => _timeStep; set => SetField(ref _timeStep, value); }

    private bool _showOrbits = true;
    public bool showOrbits { get => _showOrbits; set => SetField(ref _showOrbits, value); }
    
    private bool _showOutline = true;
    public bool showOutline { get => _showOutline; set => SetField(ref _showOutline, value); }
    
    private (string?, Entity?) _follow = ("", null);
    public (string?, Entity?) follow { get => _follow; set => SetField(ref _follow, value); }
    
    private Entity? _selected = null;
    public Entity? selected { get => _selected; set => SetField(ref _selected, value); }

    private ObservableCollection<string> _names = new ObservableCollection<string>();
    public ObservableCollection<string> names { get => _names; private set => SetField(ref _names, value); }
    
    public SimulationProperties() {
    }
    
    public event PropertyChangedEventHandler? PropertyChanged;

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null) {
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}