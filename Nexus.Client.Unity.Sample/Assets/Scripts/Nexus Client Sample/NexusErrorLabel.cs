using TMPro;
using UnityEngine;

namespace Nexus.Client.Unity.Sample
{
    /// <summary>
    /// Sets the current error message whenever the `refresh` event is raised.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    [DisallowMultipleComponent]
    public sealed class NexusErrorLabel : MonoBehaviour
    {
        /// <summary>
        /// Triggers the label to refresh with the latest creator.
        /// </summary>
        [SerializeField] private NexusGameEvent refresh = null;
        
        private TMP_Text label;

        private void Awake()
        {
            this.label = this.GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            this.RefreshLabel();
            this.refresh.RegisterListener(this.RefreshLabel);
        }

        private void OnDisable()
        {
            this.refresh.UnregisterListener(this.RefreshLabel);
        }

        private void RefreshLabel()
        {
            string text = NexusSampleApp.Instance.ErrorMessage;
            this.label.text = text;
        }
    }
}