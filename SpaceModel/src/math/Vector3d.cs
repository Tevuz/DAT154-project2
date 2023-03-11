using System.Windows;

namespace no.hvl.DAT154.V23.GROUP14.SpaceModel.math;

public struct Vector3d {

    public double x, y, z;

    public Vector3d(double x, double y, double z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }
    public Vector3d(double x) {
        this.x = x;
        this.y = x;
        this.z = x;
    }

    public static Vector3d ZERO => default;
    public static Vector3d ONE => new Vector3d(1.0);
    public static Vector3d UNIT_X => new Vector3d(1.0, 0.0, 0.0);
    public static Vector3d UNIT_Y => new Vector3d(0.0, 1.0, 0.0);
    public static Vector3d UNIT_Z => new Vector3d(0.0, 0.0, 1.0);

    public static Vector3d operator +(Vector3d left, Vector3d right) {
        return new Vector3d(left.x + right.x, left.y + right.y, left.z + right.z);
    }

    public static Vector3d operator +(Vector3d left, double right) {
        return new Vector3d(left.x + right, left.y + right, left.z + right);
    }
    
    public static Vector3d operator -(Vector3d left, Vector3d right) {
        return new Vector3d(left.x - right.x, left.y - right.y, left.z - right.z);
    }
    
    public static Vector3d operator -(Vector3d left, double right) {
        return new Vector3d(left.x - right, left.y - right, left.z - right);
    }
    
    public static Vector3d operator *(Vector3d left, Vector3d right) {
        return new Vector3d(left.x * right.x, left.y * right.y, left.z * right.z);
    }
    
    public static Vector3d operator *(Vector3d left, double right) {
        return new Vector3d(left.x * right, left.y * right, left.z * right);
    }
    
    public static Vector3d operator /(Vector3d left, Vector3d right) {
        return new Vector3d(left.x / right.x, left.y / right.y, left.z / right.z);
    }
    
    public static Vector3d operator /(Vector3d left, double right) {
        return new Vector3d(left.x / right, left.y / right, left.z / right);
    }
}