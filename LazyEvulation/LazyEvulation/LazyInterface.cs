namespace LazyEvulation;

/// <summary>
/// Interface of Lazy evulation classes.
/// </summary>
/// <typeparam name="T">General Type.</typeparam>
public interface ILazy<T>
{
    T Get();
}