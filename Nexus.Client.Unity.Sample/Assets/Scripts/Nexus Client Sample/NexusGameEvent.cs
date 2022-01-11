using System.Collections.Generic;
using UnityEngine;

namespace Nexus.Client.Unity.Sample
{
    /// <summary>
    /// Static event which can be observed/raised by different parts of the Nexus Sample App.
    /// </summary>
    [CreateAssetMenu(fileName = "Game Event", menuName = "Nexus/Sample/Nexus Game Event")]
    public sealed class NexusGameEvent : ScriptableObject
    {
        public delegate void GameEventListener();

        private readonly List<GameEventListener> listeners = new List<GameEventListener>();

        /// <summary>
        /// Trigger callbacks on all listeners.
        /// </summary>
        [ContextMenu("Raise Event")]
        public void RaiseEvent()
        {
            for (int i = this.listeners.Count - 1; i >= 0; i--)
            {
                this.listeners[i]();
            }
        }

        public void RegisterListener(GameEventListener listener)
        {
            this.listeners.Add(listener);
        }

        public void UnregisterListener(GameEventListener listener)
        {
            this.listeners.Remove(listener);
        }
    }
}