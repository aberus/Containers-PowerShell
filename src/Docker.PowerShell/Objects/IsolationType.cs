namespace Docker.PowerShell.Objects
{
    /// <summary>
    /// How a container is isolated from its host. Only Windows hosts offer a choice.
    /// </summary>
    public enum IsolationType
    {
        Default,
        Process,
        HyperV
    }
}
