using System;
using System.Windows.Controls;
using System.Windows.Threading;
using no.hvl.DAT154.V23.GROUP14.SpaceModel;

namespace DAT154_project2.gui; 

public partial class Simulation : UserControl {

    private Model model;
    private DispatcherTimer timer;
    
    public Simulation() {
        InitializeComponent();
        
        // TODO: Instantiate model

        timer = new DispatcherTimer();
        timer.Tick += Render;
    }

    private void Render(object? sender, EventArgs e) {
        // TODO: Render model
        throw new NotImplementedException();
    }
}