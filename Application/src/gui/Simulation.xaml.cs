using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DAT154_project2.gui.data;
using no.hvl.DAT154.V23.GROUP14.SpaceModel;
using no.hvl.DAT154.V23.GROUP14.SpaceModel.math;

namespace DAT154_project2.gui; 

public partial class Simulation : Canvas {
    public SimulationProperties Properties { get; set; }

    private readonly Model model;
    private double time;
    private const double updateInterval = 1.0 / 60.0;

    private Vector3d mouse;
    private Vector3d view;

    public Simulation() {
        
        Properties = new SimulationProperties();
        InitializeComponent();
        
        model = Model.LoadFromFile("res/Planets.csv");

        DispatcherTimer timer = new();
        timer.Interval = TimeSpan.FromSeconds(updateInterval);
        timer.Tick += OnTick;
        timer.Start();

        view = Vector3d.UNIT_Z;
    }

    private void OnTick(object? sender, EventArgs e) {
        time += Properties.timeStep * updateInterval;
        
        model.ForEach(
            entity => {
                entity.position = Vector3d.ZERO;
                if (entity.orbit is not Orbit orbit)
                    return;

                entity.position = orbit.origin.position;
                double theta = double.Tau * time / orbit.period;
                entity.position += new Vector3d(double.Cos(theta), double.Sin(theta), 0.0) * orbit.distance;
            });


        if (Properties.follow != null) {
            Entity? entity = model.findObjectByName(Properties.follow.name);
            if (entity != null) {
                view.x = -entity.position.x;
                view.y = -entity.position.y;
                if (Properties.follow != entity)
                    Properties.follow = entity;
            }
        }

        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext dc) {
        base.OnRender(dc);

        dc.PushTransform(new TranslateTransform(ActualWidth * 0.5, ActualHeight * 0.5));

        Pen orbitLine = new(Brushes.Yellow, 1.0f);
        Pen planetOutline = new(Brushes.White, 1.0f);

        model.ForEach(
            entity => {
                Vector3d p = (entity.position + view) / view.z;
                dc.PushTransform(new TranslateTransform(p.x, p.y));

                if (Properties.showOrbits && entity.orbit is Orbit orbit) {
                    double theta = 360.0 * time / orbit.period;
                    dc.PushTransform(new RotateTransform(theta));
                    
                    double distance = orbit.distance / view.z;
                    dc.DrawEllipse(null, orbitLine, new Point(-distance, 0.0), distance, distance);
                    
                    dc.Pop();
                }

                double radius = entity.radius / view.z + 1.0f;
                Brush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(entity.color));
                dc.DrawEllipse(brush, null, new Point(0.0, 0.0), radius, radius);
                brush = null;
                
                if (Properties.showOutline)
                    dc.DrawEllipse(null, planetOutline, new Point(0.0,0.0), radius + 3.0, radius + 3.0);
                
                dc.Pop();
            });
        
        dc.Pop();
    }

    private void Handler_MouseDown(object sender, MouseEventArgs e) {
        Point point = e.GetPosition(null);
        mouse = new Vector3d(point.X, point.Y, 0.0);
    }

    private void Handler_MouseMoved(object sender, MouseEventArgs e) {
        if (e.LeftButton != MouseButtonState.Pressed)
            return;

        Point point = e.GetPosition(null);
        Vector3d next = new(point.X, point.Y, 0.0);
        view += (next - mouse) * view.z;
        mouse = next;
        debug.Text = $"{view.x}, {view.y}, {view.z}";
    }

    private void Handle_MouseWheel(object sender, MouseWheelEventArgs e) {
        Point point = e.GetPosition(null);
        Vector3d next = new(point.X - ActualWidth * 0.5, point.Y - ActualHeight * 0.5, 0.0);

        view -= (next) * view.z;
        view.z *= float.Exp(-e.Delta / 800.0f);
        view += (next) * view.z;
    }
}