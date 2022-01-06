using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Nexus.Client.Unity.Sample
{
    /// <summary>
    /// Show creator details.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class NexusCreatorDetailsPane : MonoBehaviour
    {
        [Tooltip("Show the creator's name.")] [SerializeField]
        private TMP_Text creatorName = null;

        [Tooltip("Show the creator's url.")] [SerializeField]
        private Button creatorUrl = null;

        [Tooltip("Show the creator's unique id.")] [SerializeField]
        private TMP_Text creatorUniqueId = null;

        [Tooltip("Used to hide the details pane when no creator is selected.")] [SerializeField]
        private CanvasGroup canvasGroup = null;

        [SerializeField] private Button purchaseButton = null;

        public void ShowDetails(NexusCreator creator)
        {
            this.creatorName.text = creator.Name;
            this.creatorUrl.GetComponentInChildren<TMP_Text>().text = creator.NexusURL;
            this.creatorUniqueId.text = creator.UniqueId;
            this.canvasGroup.alpha = 1;
            this.canvasGroup.interactable = true;
        }

        private void Awake()
        {
            // hide the details pane by default, register callbacks on buttons
            this.purchaseButton.onClick.AddListener(this.OnPurchased);
            this.creatorUrl.onClick.AddListener(this.OpenCreatorURL);
            this.canvasGroup.alpha = 0;
            this.canvasGroup.interactable = false;
        }

        private void OnDestroy()
        {
            // unregister button callbacks
            this.purchaseButton.onClick.RemoveListener(this.OnPurchased);
            this.creatorUrl.onClick.RemoveListener(this.OpenCreatorURL);
        }

        private async void OnPurchased()
        {
            bool result = await NexusManager.Instance.Client.AttributeCreator();
            Debug.LogFormat("Creator {0} attributed", this.creatorName.text);
        }

        private void OpenCreatorURL()
        {
            Application.OpenURL(this.creatorUrl.GetComponentInChildren<TMP_Text>().text);
        }
    }
}