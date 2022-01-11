using UnityEngine;
using UnityEngine.Events;

namespace Nexus.Client.Unity.Sample
{
    public sealed class NexusGameEventListener : MonoBehaviour
    {
        public NexusGameEvent[] GameEvents;

        public UnityEvent OnEventRaised;

        private void OnEnable()
        {
            foreach (NexusGameEvent gameEvent in this.GameEvents)
            {
                gameEvent.RegisterListener(this.TriggerOnEventRaised);
            }
        }

        private void OnDisable()
        {
            foreach (NexusGameEvent gameEvent in this.GameEvents)
            {
                gameEvent.UnregisterListener(this.TriggerOnEventRaised);
            }
        }

        private void TriggerOnEventRaised()
        {
            this.OnEventRaised.Invoke();
        }
    }
}