using UnityEngine;

namespace Nexus.Client.Unity.Sample
{
    [DisallowMultipleComponent]
    public sealed class NexusSampleApp : MonoBehaviour
    {
        [Tooltip("Attempt to open the shop when raised")] [SerializeField]
        private NexusGameEvent openShop = null;

        [Tooltip("Attempt to close the shop when raised")] [SerializeField]
        private NexusGameEvent closeShop = null;

        [Tooltip("Attempt to purchase item from the shop when raised")] [SerializeField]
        private NexusGameEvent purchaseFromShop = null;
        
        [Space]
        [Tooltip("Animator/state machine used by the app")] [SerializeField]
        private Animator animator = null;

        private static readonly int OpenShopTrigger = Animator.StringToHash("Open Shop");

        private static readonly int CloseShopTrigger = Animator.StringToHash("Close Shop");

        private static readonly int PurchaseFromShopTrigger = Animator.StringToHash("Purchase from Shop");

        private static readonly int ReviewPurchaseTrigger = Animator.StringToHash("Review Purchase");

        private NexusSampleAppState currentState = NexusSampleAppState.Home;

        private void Awake()
        {
            this.openShop.RegisterListener(this.TryOpenShop);
            this.closeShop.RegisterListener(this.TryCloseShop);
            this.purchaseFromShop.RegisterListener(this.TryPurchase);
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