using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DAT154_project2.gui.data;
using no.hvl.DAT154.V23.GROUP14.SpaceModel;

namespace DAT154_project2.gui;

public partial class Interface : UserControl {

    public static readonly DependencyProperty PropertiesProperty = DependencyProperty.Register(nameof(Properties), typeof(SimulationProperties), typeof(Interface), null);
    
    public SimulationProperties Properties {
        get => (SimulationProperties)GetValue(PropertiesProperty);
        set => SetValue(PropertiesProperty, value);
    }

    public Interface() {
        InitializeComponent();

        time_slider.Ticks.Add(time_min_log);
        time_slider.Ticks.Add(time_hour_log);
        time_slider.Ticks.Add(time_day_log);
        time_slider.Ticks.Add(time_year_log);

        time_slider.Value = time_day_log;

        Loaded += (_, _) => {
            Properties.PropertyChanged += Follow_Changed;
            Properties.PropertyChanged += Selected_Changed;
        };
    }


    private void Time_TextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e) {
        if (sender is not TextBox box)
            return;
        
        if (!box.IsFocused)
            return;

        if (e.Text == null || !double.TryParse(box.Text[..box.SelectionStart] + e.Text + box.Text[(box.SelectionStart + box.SelectionLength)..], out double value) || value < 0.0) {
            e.Handled = true;
            return;
        }
        
        int t = time_unit.SelectedIndex;
        
        Properties.timeStep = t switch {
            0 => value / 24.0 / 60.0,
            1 => value / 24.0,
            2 => value,
            3 => value * 365.0
        };
        
        value = Properties.timeStep * 24.0 * 60.0;
        //value = double.Log(value + 1.0) / double.Log(time_slider_maxValue + 1.0);
        value = Time_toLogarithmic(value);
        value = double.Min(1.0, value);
        value = double.Max(0.0, value);
        
        time_slider.Value = value;
    }

    private void Time_ComboBox_OnSelectionChanged(object sender, RoutedEventArgs e) {
        if (sender is not ComboBoxItem item)
            return;

        if (!item.IsSelected)
            return;

        if (Properties == null)
            return;

        int t = time_unit.Items.IndexOf(item);

        double value = t switch {
            0 => (Properties.timeStep * 60.0 * 24.0),
            1 => (Properties.timeStep * 24.0),
            2 => Properties.timeStep,
            3 => (Properties.timeStep / 365.0),
            _ => 0.0
        };

        time_input.Text = value.ToString("0.00");
    }

    // nearest round number to for pluto to orbit (248 earth years)
    const double time_slider_maxValue = 60.0 * 24.0 * 365.0 * 250.0;
    private static readonly double time_min_log = Time_toLogarithmic(1.0);
    private static readonly double time_hour_log = Time_toLogarithmic(60.0);
    private static readonly double time_day_log = Time_toLogarithmic(60.0 * 24.0);
    private static readonly double time_year_log = Time_toLogarithmic(60.0 * 24.0 * 365.0);
    private const double time_slider_snap = 0.03;

    private static double Time_toLogarithmic(double value) {
        return double.Log(value + 1.0) / double.Log(time_slider_maxValue + 1.0);
    }
    
    private static double Time_toLinear(double value) {
        return double.Pow(time_slider_maxValue + 1.0, value) - 1.0;
    }

    private void Time_Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
        if (sender is not Slider slider)
            return;

        if (!slider.IsFocused)
            return;
        
        double value = e.NewValue;
        
        // snapping
        if (double.Abs(value - 0.0) < time_slider_snap)
            value = 0.0;
        if (double.Abs(value - time_min_log) < time_slider_snap)
            value = time_min_log + 0.0001;
        if (double.Abs(value - time_hour_log) < time_slider_snap)
            value = time_hour_log + 0.0001;
        if (double.Abs(value - time_day_log) < time_slider_snap)
            value = time_day_log + 0.0001;
        if (double.Abs(value - time_year_log) < time_slider_snap)
            value = time_year_log + 0.0001;
        if (double.Abs(value - 1.0) < time_slider_snap)
            value = 1.0;

        time_slider.Value = value;
        
        value = Time_toLinear(value);
        
        Properties.timeStep = value / 24.0 / 60.0;

        int t = 0;
        switch (value) {
            case >= 60.0 * 24.0 * 365.0:
                value /= 60.0 * 24.0 * 365.0;
                t = 3;
                break;
            case >= 60.0 * 24.0:
                value /= 60.0 * 24.0;
                t = 2;
                break;
            case >= 60.0:
                value /= 60.0;
                t = 1;
                break;
        }

        time_unit.SelectedIndex = t;
        time_input.Text = value.ToString("0.00");
    }

    private void Orbit_Lines_Checked(object sender, RoutedEventArgs e) {
        if (sender is not CheckBox box)
            return;
        
        if (Properties == null)
            return;
        
        Properties.showOrbits = box.IsChecked.Value;
    }
    
    private void Planet_Outlines_Checked(object sender, RoutedEventArgs e) {
        if (sender is not CheckBox box)
            return;
        
        if (Properties == null)
            return;
        
        Properties.showOutline = box.IsChecked.Value;
    }

    private void Follow_OnTextChanged(object sender, TextChangedEventArgs e) {
        if (sender is not TextBox box)
            return;
        
        if (Properties == null)
            return;
        
        Properties.follow = (box.Text, null);
    }
    
    private void Follow_Changed(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName != nameof(Properties.follow))
            return;

        follow_display.Children.Clear();
        follow_display.Margin = new Thickness(10, 0, 10, 0);
        
        if (string.IsNullOrEmpty(Properties.follow.Item1)) {
            follow_input.Background = Brushes.White;
            return;
        }
        
        Entity follow = Properties.follow.Item2;
        
        if (follow == null) {
            follow_input.Background = Brushes.LightCoral;
            return;
        }
        
        follow_input.Background = Brushes.LightGreen;

        follow_display.Margin = new Thickness(0, 0, 0, 15);

        Thickness title = new Thickness(0, 0, 0, 15);
        Thickness field = new Thickness(10, 0, 10, 5);

        follow_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Following", FontSize = 21 }}, Margin = title});
        follow_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Name:"}, new TextBlock() { Text = $"{follow.name}", TextAlignment = TextAlignment.Right }}, Margin = field});
        follow_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Color:"}, new TextBlock() { Text = $"{follow.color}", TextAlignment = TextAlignment.Right }}, Margin = field});
        follow_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Radius:"}, new TextBlock() { Text = $"{follow.radius * 1000.0:0.0} km", TextAlignment = TextAlignment.Right }}, Margin = field});
        if (follow.orbit != null) {
            follow_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Orbit:"}, new TextBlock() { Text = $"{follow.orbit?.origin.name}", TextAlignment = TextAlignment.Right }}, Margin = field});
            follow_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Distance:"}, new TextBlock() { Text = $"{follow.orbit?.distance} Mm", TextAlignment = TextAlignment.Right }}, Margin = field});
            follow_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Period:"}, new TextBlock() { Text = $"{follow.orbit?.period} days", TextAlignment = TextAlignment.Right }}, Margin = field});
        } else {
            follow_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Orbit:"}, new TextBlock() { Text = "none", TextAlignment = TextAlignment.Right }}, Margin = field});
        }
    }
    
    private void Selected_Changed(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName != nameof(Properties.select))
            return;
        
        selected_display.Children.Clear();
        selected_display.Margin = new Thickness(10, 0, 10, 0);

        Entity selected = Properties.select;

        if (selected == null)
            return;

        selected_display.Margin = new Thickness(0, 0, 0, 15);

        Thickness title = new Thickness(0, 0, 0, 15);
        Thickness field = new Thickness(10, 0, 10, 5);

        selected_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "At Cursor", FontSize = 21 }}, Margin = title});
        selected_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Name:"}, new TextBlock() { Text = $"{selected.name}", TextAlignment = TextAlignment.Right }}, Margin = field});
        selected_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Color:"}, new TextBlock() { Text = $"{selected.color}", TextAlignment = TextAlignment.Right }}, Margin = field});
        selected_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Radius:"}, new TextBlock() { Text = $"{selected.radius * 1000.0:0.0} km", TextAlignment = TextAlignment.Right }}, Margin = field});
        if (selected.orbit != null) {
            selected_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Orbit:"}, new TextBlock() { Text = $"{selected.orbit?.origin.name}", TextAlignment = TextAlignment.Right }}, Margin = field});
            selected_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Distance:"}, new TextBlock() { Text = $"{selected.orbit?.distance} Mm", TextAlignment = TextAlignment.Right }}, Margin = field});
            selected_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Period:"}, new TextBlock() { Text = $"{selected.orbit?.period} days", TextAlignment = TextAlignment.Right }}, Margin = field});
        } else {
            selected_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Orbit:"}, new TextBlock() { Text = "none", TextAlignment = TextAlignment.Right }}, Margin = field});
        }

    }
}