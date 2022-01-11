using UnityEngine;
using UnityEngine.Events;

namespace Nexus.Client.Unity.Sample
{
    /// <summary>
    /// Re-usable listener for <see cref="NexusGameEvent"/> in the scene. 
    /// </summary>
    public sealed class NexusGameEventListener : MonoBehaviour
    {
        [SerializeField] private NexusGameEvent[] gameEvents = null;

        /// <summary>
        /// Callback for custom event handling.
        /// </summary>
        [SerializeField] private UnityEvent onEventRaised = null;

        private void OnEnable()
        {
            foreach (NexusGameEvent gameEvent in this.gameEvents)
            {
                gameEvent.RegisterListener(this.TriggerOnEventRaised);
            }
        }

        private void OnDisable()
        {
            foreach (NexusGameEvent gameEvent in this.gameEvents)
            {
                gameEvent.UnregisterListener(this.TriggerOnEventRaised);
            }
        }

        private void TriggerOnEventRaised()
        {
            this.onEventRaised.Invoke();
        }
    }
}