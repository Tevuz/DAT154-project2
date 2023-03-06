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
    private readonly Model model;
    private float time;
    private readonly DispatcherTimer timer;

    public Simulation() {
        InitializeComponent();

        // TODO: Instantiate model
        model = Model.LoadFromFile("Planets.csv");

        /*model = new Model();
        model.addObject(new Entity("Star") {
            type = Type.STAR
        });
        model.addObject(new Entity("Planet") {
            type = Type.GASGIANT,
            orbit = new Orbit {
                origin = model.findObjectByName("star"),
                distance = 150.0f,
                period = 10.0f
            }
        });
        model.addObject(new Entity("Moon") {
            type = Type.TERRESTIAL,
            orbit = new Orbit {
                origin = model.findObjectByName("Planet"),
                distance = 50.0f,
                period = 1.0f
            }
        });*/

        timer = new DispatcherTimer();
        timer.Interval = TimeSpan.FromSeconds(1.0 / 60.0);
        timer.Tick += OnTick;
        timer.Start();
    }

    private void OnTick(object? sender, EventArgs e) {
        //time += DateTime.Now.Ticks / (float) TimeSpan.TicksPerSecond - time;
        time += 365.0f * 24.0f * 1.0f / 60.0f;

        model.OnTick(time);

        InvalidateVisual();
    }

    protected override void OnRender(DrawingContext dc) {
        base.OnRender(dc);

        dc.PushTransform(new TranslateTransform(ActualWidth * 0.5, ActualHeight * 0.5));

        /*Vector3 pos = model.findObjectByName("Star").position;
        dc.DrawEllipse(Brushes.Red, null, new Point(pos.X, pos.Y), 50.0, 50.0);
        
        pos = model.findObjectByName("Planet").position;
        dc.DrawEllipse(Brushes.Red, null, new Point(pos.X, pos.Y), 30.0, 30.0);*/

        model.ForEach(
            entity => {
                Vector3 pos = entity.position;

                switch (entity.type) {
                    case Type.STAR:
                        dc.DrawEllipse(Brushes.Yellow, null, new Point(pos.X, pos.Y), 50.0, 50.0);
                        break;
                    case Type.GASGIANT:
                        dc.DrawEllipse(Brushes.Green, null, new Point(pos.X, pos.Y), 30.0, 30.0);
                        break;
                    case Type.TERRESTIAL:
                        dc.DrawEllipse(Brushes.Gray, null, new Point(pos.X, pos.Y), 10.0, 10.0);
                        break;
                    default:
                        dc.DrawEllipse(Brushes.Gray, null, new Point(pos.X, pos.Y), 2.0, 2.0);
                        break;
                }
            });
    }
}