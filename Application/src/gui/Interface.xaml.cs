using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DAT154_project2.gui.data;
using no.hvl.DAT154.V23.GROUP14.SpaceModel;

namespace no.hvl.DAT154.V23.GROUP14.Application.gui;

public partial class Interface : UserControl {

    public static readonly DependencyProperty PropertiesProperty = DependencyProperty.Register(nameof(Properties), typeof(SimulationProperties), typeof(Interface), null);
    
    public SimulationProperties? Properties {
        get => (SimulationProperties)GetValue(PropertiesProperty);
        set => SetValue(PropertiesProperty, value);
    }

    public Interface() {
        InitializeComponent();

        time_slider.Ticks.Add(TimeMinLog);
        time_slider.Ticks.Add(TimeHourLog);
        time_slider.Ticks.Add(TimeDayLog);
        time_slider.Ticks.Add(TimeYearLog);

        time_slider.Value = TimeDayLog;

        Loaded += (_, _) => {
            if (Properties == null)
                throw new NullReferenceException("SimulationProperties is null");
            
            Properties.PropertyChanged += Follow_Changed;
            Properties.PropertyChanged += Selected_Changed;
        };
    }


    private void Time_TextBox_OnPreviewTextInput(object sender, TextCompositionEventArgs e) {
        if (sender is not TextBox box)
            return;
        
        if (!box.IsFocused)
            return;
        
        if (Properties == null)
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
            3 => value * 365.0,
            _ => 0.0
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
    const double TimeSliderMaxValue = 60.0 * 24.0 * 365.0 * 250.0;
    private static readonly double TimeMinLog = Time_toLogarithmic(1.0);
    private static readonly double TimeHourLog = Time_toLogarithmic(60.0);
    private static readonly double TimeDayLog = Time_toLogarithmic(60.0 * 24.0);
    private static readonly double TimeYearLog = Time_toLogarithmic(60.0 * 24.0 * 365.0);
    private const double TimeSliderSnap = 0.03;

    private static double Time_toLogarithmic(double value) {
        return double.Log(value + 1.0) / double.Log(TimeSliderMaxValue + 1.0);
    }
    
    private static double Time_toLinear(double value) {
        return double.Pow(TimeSliderMaxValue + 1.0, value) - 1.0;
    }

    private void Time_Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
        if (sender is not Slider slider)
            return;

        if (!slider.IsFocused)
            return;
        
        if (Properties == null)
            return;
        
        double value = e.NewValue;
        
        // snapping
        if (double.Abs(value - 0.0) < TimeSliderSnap)
            value = 0.0;
        if (double.Abs(value - TimeMinLog) < TimeSliderSnap)
            value = TimeMinLog + 0.0001;
        if (double.Abs(value - TimeHourLog) < TimeSliderSnap)
            value = TimeHourLog + 0.0001;
        if (double.Abs(value - TimeDayLog) < TimeSliderSnap)
            value = TimeDayLog + 0.0001;
        if (double.Abs(value - TimeYearLog) < TimeSliderSnap)
            value = TimeYearLog + 0.0001;
        if (double.Abs(value - 1.0) < TimeSliderSnap)
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
        
        Properties.showOrbits = box.IsChecked ?? false;
    }
    
    private void Planet_Outlines_Checked(object sender, RoutedEventArgs e) {
        if (sender is not CheckBox box)
            return;
        
        if (Properties == null)
            return;
        
        Properties.showOutline = box.IsChecked ?? false;
    }

    private void Follow_OnTextChanged(object sender, TextChangedEventArgs e) {
        if (sender is not TextBox box)
            return;
        
        if (Properties == null)
            return;
        
        Follow_setSuggestionBoxOpen(!string.IsNullOrEmpty(box.Text));

        var suggestions = Properties.names.Where(e => e.ToLower().Contains(box.Text.ToLower().TrimStart())).ToList();
        follow_suggestion.ItemsSource = suggestions;
        
        Properties.follow = (box.Text.TrimStart(), null);
    }

    private void Follow_setSuggestionBoxOpen(bool open) {
        follow_suggestion.Visibility = open ? Visibility.Visible : Visibility.Collapsed;
        follow_suggestion_popup.Visibility = open ? Visibility.Visible : Visibility.Collapsed;
        follow_suggestion_popup.IsOpen = open;

        follow_suggestion_popup.Width = follow_input.ActualWidth;
    }

    private void Follow_OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
        Follow_setSuggestionBoxOpen(false);
        
        if (follow_suggestion.SelectedIndex <= -1)
            return;

        follow_input.Text = follow_suggestion.SelectedItem.ToString();
        follow_suggestion.SelectedIndex = -1;
    }
    
    private void Follow_Changed(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName != nameof(Properties.follow))
            return;
        
        if (Properties == null)
            return;

        follow_display.Children.Clear();
        follow_display.Margin = new Thickness(10, 0, 10, 0);
        
        if (string.IsNullOrEmpty(Properties.follow.Item1)) {
            follow_input.Background = Brushes.White;
            return;
        }
        
        Entity? follow = Properties.follow.Item2;
        
        if (follow == null) {
            follow_input.Background = Brushes.LightCoral;
            return;
        }
        
        Follow_setSuggestionBoxOpen(false);
        
        follow_input.Background = Brushes.LightGreen;

        follow_display.Margin = new Thickness(0, 0, 0, 15);

        Thickness title = new Thickness(0, 0, 0, 15);
        Thickness field = new Thickness(10, 0, 10, 5);

        follow_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Following", FontSize = 21 }}, Margin = title});
        follow_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Name:"}, new TextBlock() { Text = $"{follow.Name}", TextAlignment = TextAlignment.Right }}, Margin = field});
        follow_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Color:"}, new TextBlock() { Text = $"{follow.Color}", TextAlignment = TextAlignment.Right }}, Margin = field});
        follow_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Radius:"}, new TextBlock() { Text = $"{follow.Radius * 1000.0:0.0} km", TextAlignment = TextAlignment.Right }}, Margin = field});
        if (follow.Orbit != null) {
            follow_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Orbit:"}, new TextBlock() { Text = $"{follow.Orbit?.Origin.Name}", TextAlignment = TextAlignment.Right }}, Margin = field});
            follow_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Distance:"}, new TextBlock() { Text = $"{follow.Orbit?.Distance} Mm", TextAlignment = TextAlignment.Right }}, Margin = field});
            follow_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Period:"}, new TextBlock() { Text = $"{follow.Orbit?.Period} days", TextAlignment = TextAlignment.Right }}, Margin = field});
        } else {
            follow_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Orbit:"}, new TextBlock() { Text = "none", TextAlignment = TextAlignment.Right }}, Margin = field});
        }
    }
    
    private void Selected_Changed(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName != nameof(Properties.selected))
            return;

        if (Properties == null)
            return;
        
        selected_display.Children.Clear();
        selected_display.Margin = new Thickness(10, 0, 10, 0);

        Entity? selected = Properties.selected;

        if (selected == null)
            return;

        selected_display.Margin = new Thickness(0, 0, 0, 15);

        Thickness title = new Thickness(0, 0, 0, 15);
        Thickness field = new Thickness(10, 0, 10, 5);

        selected_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "At Cursor", FontSize = 21 }}, Margin = title});
        selected_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Name:"}, new TextBlock() { Text = $"{selected.Name}", TextAlignment = TextAlignment.Right }}, Margin = field});
        selected_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Color:"}, new TextBlock() { Text = $"{selected.Color}", TextAlignment = TextAlignment.Right }}, Margin = field});
        selected_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Radius:"}, new TextBlock() { Text = $"{selected.Radius * 1000.0:0.0} km", TextAlignment = TextAlignment.Right }}, Margin = field});
        if (selected.Orbit != null) {
            selected_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Orbit:"}, new TextBlock() { Text = $"{selected.Orbit?.Origin.Name}", TextAlignment = TextAlignment.Right }}, Margin = field});
            selected_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Distance:"}, new TextBlock() { Text = $"{selected.Orbit?.Distance} Mm", TextAlignment = TextAlignment.Right }}, Margin = field});
            selected_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Period:"}, new TextBlock() { Text = $"{selected.Orbit?.Period} days", TextAlignment = TextAlignment.Right }}, Margin = field});
        } else {
            selected_display.Children.Add(new Grid(){Children = { new TextBlock(){Text = "Orbit:"}, new TextBlock() { Text = "none", TextAlignment = TextAlignment.Right }}, Margin = field});
        }

    }
}