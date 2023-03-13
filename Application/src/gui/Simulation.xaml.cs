using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using DAT154_project2.gui.data;
using no.hvl.DAT154.V23.GROUP14.SpaceModel;
using no.hvl.DAT154.V23.GROUP14.SpaceModel.math;

namespace no.hvl.DAT154.V23.GROUP14.Application.gui; 

public partial class Simulation : Canvas {
    public SimulationProperties Properties { get; set; }

    private readonly Model model;
    private double time;
    private const double UpdateInterval = 1.0 / 60.0;

    private Vector3d mouse;
    private Vector3d view;

    public Simulation() {
        
        Properties = new SimulationProperties();
        InitializeComponent();
        
        model = Model.LoadFromFile("res/Planets.csv");

        DispatcherTimer timer = new();
        timer.Interval = TimeSpan.FromSeconds(UpdateInterval);
        timer.Tick += OnTick;
        timer.Start();

        view = Vector3d.UNIT_Z * 5000.0;
    }
    
    private void OnTick(object? sender, EventArgs e) {
        time += Properties.timeStep * UpdateInterval;

        Vector3d cursor = ((mouse - new Vector3d(ActualWidth * 0.5, ActualHeight * 0.5, 0.0)) * view.z - view) * new Vector3d(1.0, 1.0, 0.0);
        (double, Entity?) closest = (double.PositiveInfinity, null);
        
        model.ForEach(
            entity => {
                entity.Update(time);
                
                double distance = (entity.Position - cursor).lengthSquared() - entity.Radius;
                if (distance < closest.Item1) 
                    closest = (distance, entity);
            });

        if (Properties.selected != closest.Item2)
            Properties.selected = closest.Item2;

        if (!string.IsNullOrEmpty(Properties.follow.Item1)) {
            Entity? entity = model.FindObjectByName(Properties.follow.Item1);
            if (entity != null) {
                view.x = -entity.Position.x;
                view.y = -entity.Position.y;
            }
            
            if (Properties.follow.Item2 != entity)
                Properties.follow = (entity?.Name ?? "", entity);
        }

        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext dc) {
        base.OnRender(dc);

        dc.PushTransform(new TranslateTransform(ActualWidth * 0.5, ActualHeight * 0.5));
        
        model.ForEach(entity => Render(dc, entity) );
        
        dc.Pop();
    }

    private static readonly Pen OrbitLine = new(Brushes.Yellow, 1.0f);
    private static readonly Pen PlanetOutline = new(Brushes.White, 1.0f);
    
    private void Render(DrawingContext dc, Entity entity) {
        Vector3d p = (entity.Position + view) / view.z;
        dc.PushTransform(new TranslateTransform(p.x, p.y));

        if (Properties.showOrbits && entity.Orbit is Orbit orbit) {
            double theta = 360.0 * time / orbit.Period;
            dc.PushTransform(new RotateTransform(theta));
                    
            double distance = orbit.Distance / view.z;
            dc.DrawEllipse(null, OrbitLine, new Point(-distance, 0.0), distance, distance);
                    
            dc.Pop();
        }

        double radius = entity.Radius / view.z + 1.0f;
        Brush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(entity.Color));
        dc.DrawEllipse(brush, null, new Point(0.0, 0.0), radius, radius);
                
        if (Properties.showOutline)
            dc.DrawEllipse(null, PlanetOutline, new Point(0.0,0.0), radius + 3.0, radius + 3.0);
                
        dc.Pop();
    }

    private void Handler_MouseMoved(object sender, MouseEventArgs e) {

        Point point = e.GetPosition(null);
        Vector3d next = new(point.X, point.Y, 0.0);
        
        if (e.LeftButton == MouseButtonState.Pressed)
            view += (next - mouse) * view.z;
        
        mouse = next;
    }

    private void Handle_MouseWheel(object sender, MouseWheelEventArgs e) {
        Point point = e.GetPosition(null);
        Vector3d next = new(point.X - ActualWidth * 0.5, point.Y - ActualHeight * 0.5, 0.0);

        view -= (next) * view.z;
        view.z *= float.Exp(-e.Delta / 800.0f);
        view += (next) * view.z;
    }
}