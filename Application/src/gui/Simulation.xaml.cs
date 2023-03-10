using System;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using no.hvl.DAT154.V23.GROUP14.SpaceModel;
using Vector = System.Windows.Vector;

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
        time += 3600f * 1.0f / 60.0f;

        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext dc) {
        base.OnRender(dc);

        dc.PushTransform(new TranslateTransform(ActualWidth * 0.5, ActualHeight * 0.5));
        dc.PushTransform(new TranslateTransform(view.X, view.Y));

        model.ForEach(
            entity => {
                float scale = 1.0f;
                float size = 2000.0f;

                float radius = 5.0f + 0.0f * entity.radius * 0.0005f;
                
                Vector3 pos = Vector3.Zero;
                Entity e = entity;
                while (e != null) {
                    pos *= (1.0f - 0.98f * scale);
                    if (e.orbit is Orbit o) {
                        float theta = (o.period > 0.0) ? (2.0f * float.Pi * time / o.period) : 0.0f;
                        pos += new Vector3() { X = float.Cos(theta), Y = float.Sin(theta), Z = 0.0f } * (size * o.index * scale + o.distance * (1.0f - scale));
                    }
                    e = (e.orbit?.origin ?? null)!;
                }
                
                dc.DrawEllipse(new SolidColorBrush((Color) ColorConverter.ConvertFromString(entity.color)), null, new Point(pos.X, pos.Y), radius, radius);
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
        view += new Vector3((next - mouse) * view.Z, 0.0f);
        mouse = next;
    }

}

public class SimulationProperties {
    public Transform transform;

    public float timeStep = 365.0f * 24.0f;
}