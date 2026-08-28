using System;

namespace Docker.PowerShell.Objects
{
    public class ContainerProcessExitException : Exception
    {
        public ContainerProcessExitException(long exitCode) : base(string.Format("Container process exited with non-zero exit code: {0}", exitCode)) { }
    }
}
