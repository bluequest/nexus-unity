using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nexus.Client.Unity.Sample
{
    /// <summary>
    /// A single creator in the list view.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class NexusCreatorListViewItem : MonoBehaviour
    {
        [Tooltip("Show the creator's name in the list view.")] [SerializeField]
        private TMP_Text label = null;

        [Tooltip("Show creator details using this details pane. Optional")] [SerializeField]
        private NexusCreatorDetailsPane detailsPane = null;

        private NexusCreator creator;

        internal void Show(NexusCreator creator)
        {
            this.creator = creator;
            this.label.text = creator.Name;
        }

        private void Awake()
        {
            // register for click
            this.GetComponent<Toggle>().onValueChanged.AddListener(this.OnClick);
        }

        private void OnDestroy()
        {
            // unregister for click
            this.GetComponent<Toggle>().onValueChanged.RemoveListener(this.OnClick);
        }

        private void OnClick(bool isOn)
        {
            if (this.detailsPane != null)
            {
                // if a details pane has been specified, show current creator details
                this.detailsPane.ShowDetails(this.creator);
            }
        }
    }
}