namespace Constellation.Variables {
    public struct Vec3 {
        public float x;
        public float y;
        public float z;

        public Vec3 (float _x, float _y, float _z) {
            x = _x;
            y = _y;
            z = _z;
        }

        public void Set (float _x, float _y, float _z) {
            x = _x;
            y = _y;
            z = _z;
        }
    }
}