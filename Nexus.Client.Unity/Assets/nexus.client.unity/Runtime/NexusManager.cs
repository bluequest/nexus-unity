using System.Diagnostics;
using UnityEngine;

namespace Nexus.Client.Unity
{
    /// <summary>
    /// Convenience singleton to access the <see cref="NexusClient"/>.
    /// </summary>
    /// <remarks>Uses GameObject.DontDestroyOnLoad to persist across scenes.</remarks>
    public sealed class NexusManager : MonoBehaviour
    {
        private static NexusManager instance;

        private NexusClient client;

        public static NexusManager Instance => NexusManager.instance;

        public NexusClient Client
        {
            get
            {
                if (this.client == null)
                {
                    this.client = new NexusClient(NexusUnitySettings.GetSettings());
                }

                return this.client;
            }
        }

        private void Awake()
        {
            if (NexusManager.instance == null)
            {
                // first instance of singleton, mark as dont destroy so it persists across scenes, register a basic
                // trace listener so logging from NexusClient appears in Unity's console.
                NexusManager.instance = this;
                GameObject.DontDestroyOnLoad(this.gameObject);
                Trace.Listeners.Add(new UnityTraceListener());
            }
            else
            {
                // redundant instance of singleton, destroy
                GameObject.DestroyImmediate(this.gameObject);
            }
        }
    }
}