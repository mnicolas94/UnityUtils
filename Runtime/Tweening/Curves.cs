namespace Utils.Tweening
{
    public static class Curves
    {
        public static float Linear(float time, float duration)
        {
            return time / duration;
        }
    }
}