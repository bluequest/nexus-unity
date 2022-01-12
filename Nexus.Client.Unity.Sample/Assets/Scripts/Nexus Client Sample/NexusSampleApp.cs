using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Nexus.Client.Unity.Sample
{
    [DefaultExecutionOrder(-10)]
    [DisallowMultipleComponent]
    public sealed class NexusSampleApp : MonoBehaviour
    {
        /// <summary>
        /// Singleton instance to facilitate setting selected creator by list view.
        /// </summary>
        private static NexusSampleApp instance;
        
        [Tooltip("Attempt to open the shop when raised")] [SerializeField]
        private NexusGameEvent openShop = null;

        [Tooltip("Attempt to close the shop when raised")] [SerializeField]
        private NexusGameEvent closeShop = null;

        [Tooltip("Attempt to purchase item from the shop when raised")] [SerializeField]
        private NexusGameEvent purchaseFromShop = null;

        [Tooltip("Invoked whenever an error should be shown")] [SerializeField]
        private NexusGameEvent openError = null;

        [Tooltip("Invoked whenever the current selected creator changes")] [SerializeField]
        private NexusGameEvent selectedCreatorChanged = null;
        
        [Tooltip("Invoked whenever the creators changes")] [SerializeField]
        private NexusGameEvent creatorsChanged = null;
        
        [Space]
        [Tooltip("Animator/state machine used by the app")] [SerializeField]
        private Animator animator = null;
        
        [Header("Testing")]
        [Tooltip("Time till moving from purchasing to review purchase, emulates server latency")] [SerializeField]
        private float delayBeforePurchasing = 1f;

        [Tooltip("If true, opening the shop will always fail. Otherwise run normally")] [SerializeField]
        private bool failGetCreators = false;

        [Tooltip("If true, purchase will always report failure. Otherwise run normally")] [SerializeField]
        private bool failPurchase = false;

        // animator triggers. see Skoot_Animator for more information.
        private static readonly int OpenShopTrigger = Animator.StringToHash("Open Shop");

        private static readonly int CloseShopTrigger = Animator.StringToHash("Close Shop");

        private static readonly int PurchaseFromShopTrigger = Animator.StringToHash("Purchase from Shop");

        private static readonly int ReviewPurchaseTrigger = Animator.StringToHash("Review Purchase");

        /// <summary>
        /// Current state of the app. App is a FSM which moves between four states.
        /// See Skoot_Animator for more information.
        /// </summary>
        private NexusSampleAppState currentState = NexusSampleAppState.Home;

        /// <summary>
        /// Current creator. Set to the first creator from Nexus.gg initially.
        /// </summary>
        private NexusCreator selectedCreator;

        private NexusCreators creators;

        public static NexusSampleApp Instance => NexusSampleApp.instance;

        internal NexusCreator SelectedCreator => this.selectedCreator;

        internal NexusCreators Creators => this.creators;

        internal string ErrorMessage { get; private set; }

        /// <summary>
        /// Update the selected creator, raise a change event trigger refresh in the UI.
        /// </summary>
        internal void SetSelectedCreator(NexusCreator creator)
        {
            this.selectedCreator = creator;
            this.selectedCreatorChanged.RaiseEvent();
        }

        private void Awake()
        {
            if (NexusSampleApp.instance == null)
            {
                // register for state change events from the UI, e.g. clicking on the store button
                // this happens here so we don't register for new instances after the first. also
                // note we do not unregister the listeners on destroy as new instances would trigger
                // unregister when destroyed -- we don't need to worry about unregistering, though
                // as this is a singleton which persists as long as the game is running
                this.openShop.RegisterListener(this.TryOpenShop);
                this.closeShop.RegisterListener(this.TryCloseShop);
                this.purchaseFromShop.RegisterListener(this.TryPurchase);
                
                // first instance of singleton, mark as dont destroy so it persists across scenes, register a basic
                // trace listener so logging from NexusClient appears in Unity's console.
                NexusSampleApp.instance = this;
                GameObject.DontDestroyOnLoad(this.gameObject);
            }
            else
            {
                // redundant instance of singleton, destroy
                GameObject.DestroyImmediate(this.gameObject);
            }
        }

        private async void Start()
        {
            // retrieve the list of creators then show them in the list view
            this.creators = await NexusManager.Instance.Client.GetCreators();
            this.creatorsChanged.RaiseEvent();
        }

        private void TryOpenShop()
        {
            // invoked when opening the skoot shop from the home screen
            if (this.currentState == NexusSampleAppState.Home)
            {
                if (this.failGetCreators || this.creators == null || this.creators.Creators == null || this.creators.Creators.Length == 0)
                {
                    // show error message
                    this.ShowError("Failed to get creators. Please try again.");
                }
                else
                {
                    this.animator.SetTrigger(OpenShopTrigger);
                    this.currentState = NexusSampleAppState.Shopping;
                }
            }
        }

        private void TryCloseShop()
        {
            // invoked when clicking the "X" button in the shop screen
            // ignore if the player is in the process of purchasing something
            if (this.currentState == NexusSampleAppState.Shopping ||
                this.currentState == NexusSampleAppState.ReviewingPurchase)
            {
                this.animator.SetTrigger(CloseShopTrigger);
                this.currentState = NexusSampleAppState.Home;
            }
        }

        private void TryPurchase()
        {
            // invoked when clicking the purchase button in the shop
            if (this.currentState == NexusSampleAppState.Shopping && this.selectedCreator != null)
            {
                this.StartCoroutine(this.TryPurchaseAsync());
            }
        }

        private IEnumerator TryPurchaseAsync()
        {
            // enter purchasing state
            this.currentState = NexusSampleAppState.Purchasing;
            this.animator.SetTrigger(PurchaseFromShopTrigger);
            
            // wait a spell to emulate talking to the server, show the progress ui
            for (float t = 0; t < this.delayBeforePurchasing; t += Time.deltaTime)
            {
                yield return null;
            }

            // trigger purchase on server, wait for results
            bool result;
            if (this.failPurchase)
            {
                // skip the server call, just fail for testing purposes
                result = false;
            }
            else
            {
                // note this uses `WaitUntil` as we cannot `await` in a coroutine
                Task<bool> task = NexusManager.Instance.Client.AttributeCreator();
                yield return new WaitUntil(() => task.IsCompleted);
                
                // ignoring Task.Result for now as the API is in development and always returns false
                result = true;
            }

            if (result)
            {
                // show confirmation to player
                this.currentState = NexusSampleAppState.ReviewingPurchase;
                this.animator.SetTrigger(ReviewPurchaseTrigger);
            }
            else
            {
                // show error message
                this.ShowError("Failed to complete purchase. Please try again.");
                
                // return to the previous state
                this.animator.SetTrigger(OpenShopTrigger);
                this.currentState = NexusSampleAppState.Shopping;
            }
        }

        private void ShowError(string message)
        {
            this.ErrorMessage = message;
            this.openError.RaiseEvent();
        }

        private enum NexusSampleAppState
        {
            Unknown,
            /// <summary>q
            /// Player is at the home screen
            /// </summary>
            Home,
            /// <summary>
            /// Player is viewing the shop
            /// </summary>
            Shopping,
            /// <summary>
            /// Player has initiated the purchase and is waiting for confirmation
            /// </summary>
            Purchasing,
            /// <summary>
            /// Player is viewing purchase confirmation
            /// </summary>
            ReviewingPurchase,
        }
    }
}