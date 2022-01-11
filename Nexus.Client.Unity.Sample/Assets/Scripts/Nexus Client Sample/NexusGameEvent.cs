using System.Collections.Generic;
using UnityEngine;

namespace Nexus.Client.Unity.Sample
{
    [CreateAssetMenu(fileName = "Game Event", menuName = "Nexus/Sample/Nexus Game Event")]
    public sealed class NexusGameEvent : ScriptableObject
    {
        public delegate void GameEventListener();
        
        private List<GameEventListener> listeners = new List<GameEventListener>();

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