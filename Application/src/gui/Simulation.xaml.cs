using System;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using no.hvl.DAT154.V23.GROUP14.SpaceModel;

namespace DAT154_project2.gui; 

public partial class Simulation : Canvas {
    
    private readonly Model model;
    private float time;

    private Vector2 mouse;
    private Vector3 view;

    public Simulation() {
        InitializeComponent();
        
        model = Model.LoadFromFile("res/Planets.csv");

        DispatcherTimer timer = new();
        timer.Interval = TimeSpan.FromSeconds(1.0 / 60.0);
        timer.Tick += OnTick;
        timer.Start();

        view = Vector3.UnitZ;
    }

    private void OnTick(object? sender, EventArgs e) {
        time += 1.0f * 7.0f / 60.0f;
        
        model.ForEach(
            entity => {
                entity.position = Vector3.Zero;
                if (entity.orbit is not Orbit orbit)
                    return;
                
                entity.position = orbit.origin.position;
                float theta = (orbit.period > 0.0) ? (2.0f * float.Pi * time / orbit.period) : 0.0f;
                entity.position += new Vector3(float.Cos(theta), float.Sin(theta), 0.0f) * orbit.distance;
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

                float radius = entity.radius * view.Z;
                
                Point p;

                if (entity.orbit is Orbit orbit) {
                    p = new Point((orbit.origin.position.X + view.X) *view.Z, (orbit.origin.position.Y + view.Y) * view.Z);
                    dc.DrawEllipse(null, new Pen(Brushes.Yellow, 1.0f), p, orbit.distance * view.Z, orbit.distance * view.Z);
                }

                p = new Point((entity.position.X + view.X) * view.Z, (entity.position.Y + view.Y) * view.Z);
                dc.DrawEllipse(new SolidColorBrush((Color) ColorConverter.ConvertFromString(entity.color)), null, p, radius, radius);
                
                dc.DrawEllipse(null, new Pen(Brushes.White, 1.0f), p, radius + 3.0f, radius + 3.0f);
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

    public void Handler_MouseDown(object sender, MouseEventArgs e) {
        Point point = e.GetPosition(null);
        mouse = new Vector2((float) point.X, (float) point.Y);
    }

    private void Handler_MouseMoved(object sender, MouseEventArgs e) {
        if (e.LeftButton != MouseButtonState.Pressed)
            return;

        Point point = e.GetPosition(null);
        Vector2 next = new((float) point.X, (float) point.Y);
        view += new Vector3((next - mouse) / view.Z, 0.0f);
        mouse = next;
    }

    private void Handle_MouseWheel(object sender, MouseWheelEventArgs e) {
        view.Z *= float.Exp(e.Delta / 800.0f);
        debug.Text = $"{view.Z}";
    }
}

public class SimulationProperties {
    public Transform transform;

    public float timeStep = 365.0f * 24.0f;
}