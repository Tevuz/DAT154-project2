using System;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using no.hvl.DAT154.V23.GROUP14.SpaceModel;
using no.hvl.DAT154.V23.GROUP14.SpaceModel.math;

namespace DAT154_project2.gui; 

public partial class Simulation : Canvas {
    
    private readonly Model model;
    private double time;
    private const double updateInterval = 1.0 / 60.0;

    private Vector3d mouse;
    private Vector3d view;

    public Simulation() {
        InitializeComponent();
        
        model = Model.LoadFromFile("res/Planets.csv");

        DispatcherTimer timer = new();
        timer.Interval = TimeSpan.FromSeconds(updateInterval);
        timer.Tick += OnTick;
        timer.Start();

        view = Vector3d.UNIT_Z;
    }

    private void OnTick(object? sender, EventArgs e) {
        time += 1.0f * 7.0f / 60.0f;
        
        model.ForEach(
            entity => {
                entity.position = Vector3d.ZERO;
                if (entity.orbit is not Orbit orbit)
                    return;
                
                entity.position = orbit.origin.position;
                double theta = (orbit.period > 0.0) ? (2.0 * double.Pi * time / orbit.period) : 0.0;
                entity.position += new Vector3d(double.Cos(theta), double.Sin(theta), 0.0) * orbit.distance;
            });

        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext dc) {
        base.OnRender(dc);

        dc.PushTransform(new TranslateTransform(ActualWidth * 0.5, ActualHeight * 0.5));

        model.ForEach(
            entity => {
                float scale = 0.0f;
                float size = 2000.0f;

                double radius = entity.radius / view.z;
                
                Vector3d p;

                if (entity.orbit is Orbit orbit) {
                    p = ((orbit.origin.position + view) / view.z);
                    dc.DrawEllipse(null, new Pen(Brushes.Yellow, 1.0f), new Point(p.x, p.y), orbit.distance / view.z, orbit.distance / view.z);
                }

                p = ((entity.position + view) / view.z);
                dc.DrawEllipse(new SolidColorBrush((Color) ColorConverter.ConvertFromString(entity.color)), null, new Point(p.x, p.y), radius, radius);
                
                dc.DrawEllipse(null, new Pen(Brushes.White, 1.0f), new Point(p.x, p.y), radius + 3.0f, radius + 3.0f);
                /*switch (entity.type) {
                    case Type.STAR:
                        dc.DrawEllipse(Brushes.Yellow, null, new Point(pos.X, pos.Y), 6.0, 6.0);
                        break;
                    case Type.GAS:
                        dc.DrawEllipse(Brushes.Green, null, new Point(pos.X, pos.Y), 5.0, 5.0);
                        break;
                    case Type.ROCK:
                        dc.DrawEllipse(Brushes.Gray, null, new Point(pos.X, pos.Y), 4.0, 4.0);
                        break;
                    case Type.ICE:
                        dc.DrawEllipse(Brushes.Blue, null, new Point(pos.X, pos.Y), 3.0, 3.0);
                        break;
                    default:
                        dc.DrawEllipse(Brushes.Gray, null, new Point(pos.X, pos.Y), 2.0, 2.0);
                        break;
                }*/
            });
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
        view.z *= float.Exp(-e.Delta / 800.0f);
        debug.Text = $"{view.x}, {view.y}, {view.z}";
    }
}

public class SimulationProperties {
    public Transform transform;

    public float timeStep = 365.0f * 24.0f;
}