using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Nexus.Client.Unity
{
    /// <summary>
    /// Super simple conduit for .Net Trace's to appear in Unity's console.
    /// </summary>
    internal sealed class UnityTraceListener : TraceListener
    {
        public override void Write(string message)
        {
        }

        public override void WriteLine(string message)
        {
            Debug.Log(message);
        }
    }
}