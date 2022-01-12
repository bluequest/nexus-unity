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
        [Tooltip("Refresh list view raised")] [SerializeField]
        private NexusGameEvent refresh = null;
        
        [Tooltip("Input used to filter creator list.")] [SerializeField]
        private TMP_InputField inputField = null;
        
        [Tooltip("A row in the list view. One per a creator.")] [SerializeField]
        private NexusCreatorListViewItem listViewItem = null;

        [Tooltip("Delay before refreshing list view to reduce refresh rate when typing.")] [SerializeField]
        private float delayBeforeRefresh = 0.05f;

        private void Awake()
        {
            // hide prefab list view item, register for filter changes
            this.inputField.onValueChanged.AddListener(this.OnFilterChanged);
            this.listViewItem.gameObject.SetActive(false);
            this.refresh.RegisterListener(this.Refresh);
        }

        private void OnDestroy()
        {
            // unregister for filter changes
            this.inputField.onValueChanged.RemoveListener(this.OnFilterChanged);
            this.refresh.UnregisterListener(this.Refresh);
        }

        private void OnFilterChanged(string filter)
        {
            if (NexusSampleApp.Instance.Creators != null)
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
                creators = NexusSampleApp.Instance.Creators.Creators;
            }
            else
            {
                Regex regex = new Regex(string.Format(@".*{0}.*", this.inputField.text), RegexOptions.IgnoreCase);
                creators = NexusSampleApp.Instance.Creators.Creators.Where(c => regex.IsMatch(c.Name));
            }

            // update list view
            foreach (NexusCreator creator in creators)
            {
                NexusCreatorListViewItem nexusCreatorListViewItem = GameObject.Instantiate(this.listViewItem, this.listViewItem.transform.parent);
                nexusCreatorListViewItem.Show(creator);
                nexusCreatorListViewItem.gameObject.SetActive(true);
            }

            // set currently selected creator if not already set
            if (NexusSampleApp.Instance.SelectedCreator == null && NexusSampleApp.Instance.Creators.Creators.Length > 0)
            {
                NexusSampleApp.Instance.SetSelectedCreator(NexusSampleApp.Instance.Creators.Creators[0]);
            }
        }
    }
}