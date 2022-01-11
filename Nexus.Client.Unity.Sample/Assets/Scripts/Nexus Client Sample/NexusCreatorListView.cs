using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Nexus.Client.Unity.Sample
{
    /// <summary>
    /// Show a filterable list of creators.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class NexusCreatorListView : MonoBehaviour
    {
        [Tooltip("Input used to filter creator list.")] [SerializeField]
        private TMP_InputField inputField = null;
        
        [Tooltip("A row in the list view. One per a creator.")] [SerializeField]
        private NexusCreatorListViewItem listViewItem = null;

        [Tooltip("Delay before refreshing list view to reduce refresh rate when typing.")] [SerializeField]
        private float delayBeforeRefresh = 0.05f;

        private NexusCreators creators;

        private void Awake()
        {
            // hide prefab list view item, register for filter changes
            this.inputField.onValueChanged.AddListener(this.OnFilterChanged);
            this.listViewItem.gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            // unregister for filter changes
            this.inputField.onValueChanged.RemoveListener(this.OnFilterChanged);
        }

        private async void Start()
        {
            // retrieve the list of creators then show them in the list view
            this.creators = await NexusManager.Instance.Client.GetCreators();
            this.Refresh();
        }

        private void OnFilterChanged(string filter)
        {
            if (this.creators != null)
            {
                // refresh the list view, but only if we've already loaded creators successfully
                this.Refresh();
            }
        }

        private void Refresh()
        {
            // convenience method, we actually want to refresh asynchronously
            this.StopAllCoroutines();
            this.StartCoroutine(this.RefreshInternal());
        }

        private IEnumerator RefreshInternal()
        {
            // wait a moment in case player is still typing
            for (float t = 0; t < this.delayBeforeRefresh; t += Time.deltaTime)
            {
                yield return null;
            }
            
            // clear existing items
            for (int i = this.listViewItem.transform.parent.childCount - 1; i >= 0; i--)
            {
                Transform child = this.listViewItem.transform.parent.GetChild(i);
                if (child.gameObject.activeSelf)
                {
                    // don't destroy the prefab, which is disabled
                    GameObject.Destroy(child.gameObject);
                }
            }
            
            // filter creators
            IEnumerable<NexusCreator> creators;
            if (string.IsNullOrEmpty(this.inputField.text))
            {
                // no filter
                creators = this.creators.Creators;
            }
            else
            {
                Regex regex = new Regex(string.Format(@".*{0}.*", this.inputField.text), RegexOptions.IgnoreCase);
                creators = this.creators.Creators.Where(c => regex.IsMatch(c.Name));
            }

            // update list view
            foreach (NexusCreator creator in creators)
            {
                NexusCreatorListViewItem nexusCreatorListViewItem = GameObject.Instantiate(this.listViewItem, this.listViewItem.transform.parent);
                nexusCreatorListViewItem.Show(creator);
                nexusCreatorListViewItem.gameObject.SetActive(true);
            }

            // set currently selected creator if not already set
            if (NexusSampleApp.Instance.SelectedCreator == null && this.creators.Creators.Length > 0)
            {
                NexusSampleApp.Instance.SetSelectedCreator(this.creators.Creators[0]);
            }
        }
    }
}