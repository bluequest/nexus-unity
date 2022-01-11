using System;
using TMPro;
using UnityEngine;

namespace Nexus.Client.Unity.Sample
{
    [RequireComponent(typeof(TMP_Text))]
    [DisallowMultipleComponent]
    public sealed class SelectedNexusCreatorLabel : MonoBehaviour
    {
        [SerializeField] private NexusGameEvent refresh = null;

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