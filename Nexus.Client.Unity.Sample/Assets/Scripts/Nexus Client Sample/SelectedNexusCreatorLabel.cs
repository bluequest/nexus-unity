using System;
using TMPro;
using UnityEngine;

namespace Nexus.Client.Unity.Sample
{
    /// <summary>
    /// Sets the creator's name for the currently selected creator whenever the `refresh` event is raised.
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    [DisallowMultipleComponent]
    public sealed class SelectedNexusCreatorLabel : MonoBehaviour
    {
        /// <summary>
        /// Triggers the label to refresh with the latest creator.
        /// </summary>
        [SerializeField] private NexusGameEvent refresh = null;

        /// <summary>
        /// Optional. Sets the label using the given format string.
        /// </summary>
        [SerializeField] private string formatString = null;
        
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
            NexusCreator creator = NexusSampleApp.Instance.SelectedCreator;

            string text;
            if (creator == null)
            {
                // this will happen `OnEnable` but should refresh as soon as the Nexus creators have been retrieved
                text = "lorem ipsum";
            }
            else if (string.IsNullOrEmpty(this.formatString))
            {
                text = creator.Name;
            }
            else
            {
                try
                {
                    text = string.Format(this.formatString, creator.Name);
                }
                catch (FormatException)
                {
                    Debug.LogErrorFormat(this, "Invalid format string: {0}", formatString);
                    text = creator.Name;
                }
            }

            this.label.text = text;
        }
    }
}