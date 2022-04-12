namespace Utils.Tweening
{
    public static class Curves
    {
        public delegate float TimeCurveFunction(float time, float duration);

        public static float Linear(float time, float duration)
        {
            return time / duration;
        }
    }
}