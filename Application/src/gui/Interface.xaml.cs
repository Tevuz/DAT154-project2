using System.Windows;
using System.Windows.Controls;

namespace DAT154_project2.gui; 

public partial class Interface : UserControl {
    public Interface() {
        InitializeComponent();
    }

    private void SliderChanged(object sender, RoutedPropertyChangedEventArgs<double> e) {
        if (sender is not Slider slider)
            return;

        switch (slider.Name) {
            case "slider_planet_scale":
                break;
            case "slider_distance_scale":
                break;
        }
    }
}