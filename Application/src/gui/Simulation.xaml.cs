using System;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using no.hvl.DAT154.V23.GROUP14.SpaceModel;
using Type = no.hvl.DAT154.V23.GROUP14.SpaceModel.Type;

namespace DAT154_project2.gui; 

public partial class Simulation : Canvas {

    private Model model;
    private DispatcherTimer timer;
    private float time;
    
    public Simulation() {
        InitializeComponent();
        
        // TODO: Instantiate model
        model = Model.LoadFromFile("");

        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1.0 / 60.0);
        timer.Tick += OnTick;
        timer.Start();
    }

    private void OnTick(object? sender, EventArgs e) {
        time = DateTime.Now.Ticks / (float) TimeSpan.TicksPerSecond;
        
        model.OnTick(time);
        
        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext dc) {
        base.OnRender(dc);
        
        model.ForEach(
            (entity) => {
                Vector3 pos = entity.position;
                
                switch (entity.type) {
                    case Type.STAR:
                        break;
                    case Type.GASGIANT:
                        break;
                    case Type.TERRESTIAL:
                        break;
                    default:
                        dc.DrawEllipse(Brushes.White, null, new Point(pos.X, pos.Y), 50.0, 50.0);
                        break;
                }
            });
    }
}
