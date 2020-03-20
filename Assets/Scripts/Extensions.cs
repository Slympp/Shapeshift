public static class Extensions {
    public static float Normalize(this float v, float inMin, float inMax, float outMin = 0, float outMax = 1f) {
        return outMin + (v - inMin) * (outMax - outMin) / (inMax - inMin);
    }
}