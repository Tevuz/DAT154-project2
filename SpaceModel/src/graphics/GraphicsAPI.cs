namespace no.hvl.DAT154.V23.GROUP14.SpaceModel.graphics;

public class GraphicsAPI {
    // Vector center, double radius, Properties properties
    public readonly Action<Vector, double, Properties?> drawCircle;

    // Vector[] vertices, Properties properties
    private Action<Vector[], Properties?> drawLine;

    // Vector[] vertices, Properties properties
    private Action<Vector[], Properties?> drawPolygon;

    public GraphicsAPI(Action<Vector, double, Properties?>? drawCircle, Action<Vector[], Properties?>? drawPolygon, Action<Vector[], Properties?>? drawLine) {
        this.drawCircle = drawCircle ?? ((_, _, _) => throw new NotImplementedException());
        this.drawPolygon = drawPolygon ?? ((_, _) => throw new NotImplementedException());
        this.drawLine = drawLine ?? ((_, _) => throw new NotImplementedException());
    }
}

public struct Vector {
    public double x, y, z;
}

public struct Properties {
    public Color? fillColor;
    public Color? lineColor;
}

public struct Color {
    public byte a, r, g, b;
}