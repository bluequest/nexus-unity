namespace Nexus.Client
{
    /// <summary>
    /// Settings repository used by <see cref="NexusClient"/>.
    /// </summary>
    public interface INexusSettings
    {
        string SharedSecret { get; }
    }
}