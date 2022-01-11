using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nexus.Client.Unity.Sample
{
    /// <summary>
    /// A single creator in the list view.
    /// </summary>
    [RequireComponent(typeof(Button))]
    [DisallowMultipleComponent]
    public sealed class NexusCreatorListViewItem : MonoBehaviour
    {
        [Tooltip("Show the creator's name in the list view.")] [SerializeField]
        private TMP_Text label = null;

        private NexusCreator creator;

        internal void Show(NexusCreator creator)
        {
            this.creator = creator;
            this.label.text = creator.Name;
        }

        private void Awake()
        {
            // register for click
            this.GetComponent<Button>().onClick.AddListener(this.OnClick);
        }

        private void OnDestroy()
        {
            // unregister for click
            this.GetComponent<Button>().onClick.RemoveListener(this.OnClick);
        }

        private void OnClick()
        {
            NexusSampleApp.Instance.SetSelectedCreator(this.creator);
        }
    }
}