/// <summary>
/// Test attribute.
/// </summary>
public class TestAttribute : Attribute
{
    /// <summary>
    /// Argument about exception.
    /// </summary>
    public Type Expected { get; private set; }

    /// <summary>
    /// Is ignored method.
    /// </summary>
    public string Ignore { get; private set; }
}