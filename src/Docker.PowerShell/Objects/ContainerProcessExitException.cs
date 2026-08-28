using System;

namespace Docker.PowerShell.Objects
{
    /// <summary>
    /// Thrown when a container's process ends with a non-zero exit code.
    /// </summary>
    public class ContainerProcessExitException : Exception
    {
        /// <summary>
        /// Creates the exception for the given exit code.
        /// </summary>
        public ContainerProcessExitException(long exitCode) : base(string.Format("Container process exited with non-zero exit code: {0}", exitCode)) { }
    }
}
