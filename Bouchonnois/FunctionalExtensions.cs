namespace Bouchonnois
{
    // cf scope function in kotlin
    public static class FunctionalExtensions
    {
        public static TResult? Run<T, TResult>(this T obj, Func<T, TResult> func) => obj != null ? func(obj) : default;

        public static T Apply<T>(this T obj, Action<T> action)
        {
            if (obj != null) action(obj);
            return obj;
        }

        public static T And<T>(this T obj, Action<T> action) => Apply(obj, action);
    }
}
