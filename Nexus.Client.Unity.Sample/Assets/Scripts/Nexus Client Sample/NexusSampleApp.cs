using UnityEngine;

namespace Nexus.Client.Unity.Sample
{
    [DefaultExecutionOrder(-10)]
    [DisallowMultipleComponent]
    public sealed class NexusSampleApp : MonoBehaviour
    {
        private static NexusSampleApp instance;
        
        [Tooltip("Attempt to open the shop when raised")] [SerializeField]
        private NexusGameEvent openShop = null;

        [Tooltip("Attempt to close the shop when raised")] [SerializeField]
        private NexusGameEvent closeShop = null;

        [Tooltip("Attempt to purchase item from the shop when raised")] [SerializeField]
        private NexusGameEvent purchaseFromShop = null;

        [Tooltip("Invoked whenever the current selected creator changes")] [SerializeField]
        private NexusGameEvent selectedCreatorChanged = null;
        
        [Space]
        [Tooltip("Animator/state machine used by the app")] [SerializeField]
        private Animator animator = null;

        private static readonly int OpenShopTrigger = Animator.StringToHash("Open Shop");

        private static readonly int CloseShopTrigger = Animator.StringToHash("Close Shop");

        private static readonly int PurchaseFromShopTrigger = Animator.StringToHash("Purchase from Shop");

        private static readonly int ReviewPurchaseTrigger = Animator.StringToHash("Review Purchase");

        private NexusSampleAppState currentState = NexusSampleAppState.Home;

        private NexusCreator selectedCreator;

        public static NexusSampleApp Instance => NexusSampleApp.instance;

        public NexusCreator SelectedCreator => selectedCreator;

        internal void SetSelectedCreator(NexusCreator creator)
        {
            this.selectedCreator = creator;
            this.selectedCreatorChanged.RaiseEvent();
        }

        private void Awake()
        {
            this.openShop.RegisterListener(this.TryOpenShop);
            this.closeShop.RegisterListener(this.TryCloseShop);
            this.purchaseFromShop.RegisterListener(this.TryPurchase);
            
            if (NexusSampleApp.instance == null)
            {
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

        private void OnDestroy()
        {
            this.openShop.UnregisterListener(this.TryOpenShop);
            this.closeShop.UnregisterListener(this.TryCloseShop);
            this.purchaseFromShop.UnregisterListener(this.TryPurchase);
        }

        private void TryOpenShop()
        {
            if (this.currentState == NexusSampleAppState.Home)
            {
                this.animator.SetTrigger(OpenShopTrigger);
                this.currentState = NexusSampleAppState.Shopping;
            }
        }

        private void TryCloseShop()
        {
            if (this.currentState == NexusSampleAppState.Shopping ||
                this.currentState == NexusSampleAppState.ReviewingPurchase)
            {
                this.animator.SetTrigger(CloseShopTrigger);
                this.currentState = NexusSampleAppState.Home;
            }
        }

        private async void TryPurchase()
        {
            if (this.currentState == NexusSampleAppState.Shopping)
            {
                // enter purchasing state
                this.currentState = NexusSampleAppState.Purchasing;
                this.animator.SetTrigger(PurchaseFromShopTrigger);
                
                // trigger purchase on server, wait for results
                // TODO
                bool result = await NexusManager.Instance.Client.AttributeCreator();
                
                // show confirmation to player
                this.currentState = NexusSampleAppState.ReviewingPurchase;
                this.animator.SetTrigger(ReviewPurchaseTrigger);
            }
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