// StoreService.cs
//
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.
//
// 
// -------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Aig.Client.Integration.Runtime.Subsystem;
using Core.Settings;
using Core.Attributes;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Core.Services
{
    [InjectionAlias(typeof(IInAppPurchaseService))]
    public class InAppPurchaseService : Service, IInAppPurchaseService
    {
        [Inject]
        private IapProductsSettings _iapProductsSettings;

        [Inject]
        private IPlayerDataService _playerDataService;

        [Inject]
        private GameSettings _gameSettings;

        //[Inject]
        //private IAnalyticsService _analyticsService;

        private IStoreController _storeController;          // The Unity Purchasing system.
        private IExtensionProvider _storeExtensionProvider; // The store-specific Purchasing subsystems.

        private Action<string> _onCompletteHandler;
        private Action<string> _onErrorHandler;

        private bool _isRealPurchase = false;

        public static readonly string PRODUCT_NO_ADS = "sushijump.noads";

        // Product identifiers for all products capable of being purchased: 
        // "convenience" general identifiers for use with Purchasing, and their store-specific identifier 
        // counterparts for use with and outside of Unity Purchasing. Define store-specific identifiers 
        // also on each platform's publisher dashboard (iTunes Connect, Google Play Developer Console, etc.)

        // General product identifiers for the consumable, non-consumable, and subscription products.
        // Use these handles in the code to reference which product to purchase. Also use these values 
        // when defining the Product Identifiers on the store. Except, for illustration purposes, the 
        // kProductIDSubscription - it has custom Apple and Google identifiers. We declare their store-
        // specific mapping to Unity Purchasing's AddProduct, below.
        //public string kProductIDConsumable = "consumable";
        //public string kProductIDNonConsumable = "nonconsumable";
        //public string kProductIDSubscription = "subscription";

        // Apple App Store-specific product identifier for the subscription product.
        //private string kProductNameAppleSubscription = "com.unity3d.subscription.new";

        // Google Play Store-specific product identifier subscription product.
        //private string kProductNameGooglePlaySubscription = "com.unity3d.subscription.original";

        public override void Run()
        {
            base.Run();

            if (_storeController == null)
            {
                InitializePurchasing();
            }
        }
        
        public void InitializePurchasing()
        {
            // If we have already connected to Purchasing ...
            if (IsInitialized())
            {
                // ... we are done here.
                return;
            }

            // Create a builder, first passing in a suite of Unity provided stores.
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            // Add a product to sell / restore by way of its identifier, associating the general identifier
            // with its store-specific identifiers.
            //builder.AddProduct(kProductIDConsumable, ProductType.Consumable);

            foreach (IapProduct iapProduct in _iapProductsSettings.iapProductsList)
            {
                builder.AddProduct(iapProduct.productId, ProductType.Consumable);
            }

            /*builder.AddProduct(_offersSettings.StarterPackOfferInfo.ProductId, ProductType.Consumable);
            builder.AddProduct(_offersSettings.BeginnerStarterPackOfferInfo.ProductId, ProductType.Consumable);
            builder.AddProduct(_offersSettings.SuperStarterPackOfferInfo.ProductId, ProductType.Consumable);

            foreach (OfferInfo offerInfo in _offersSettings.RankOffersList)
            {
                builder.AddProduct(offerInfo.productId, ProductType.Consumable);
            }*/

            builder.AddProduct(PRODUCT_NO_ADS, ProductType.NonConsumable);

            // Continue adding the non-consumable product.
            //builder.AddProduct(kProductIDNonConsumable, ProductType.NonConsumable);
            // And finish adding the subscription product. Notice this uses store-specific IDs, illustrating
            // if the Product ID was configured differently between Apple and Google stores. Also note that
            // one uses the general kProductIDSubscription handle inside the game - the store-specific IDs 
            // must only be referenced here. 
            /*
            builder.AddProduct(kProductIDSubscription, ProductType.Subscription, new IDs(){
                { kProductNameAppleSubscription, AppleAppStore.Name },
                { kProductNameGooglePlaySubscription, GooglePlay.Name },
            });
            */
            // Kick off the remainder of the set-up with an asynchrounous call, passing the configuration 
            // and this class' instance. Expect a response either in OnInitialized or OnInitializeFailed.
            UnityPurchasing.Initialize(this, builder);
        }

        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return _storeController != null && _storeExtensionProvider != null;
        }

        public void BuyProduct(string productId, Action<string> onCompleteHandler, Action<string> onErrorHandler)
        {
            _onCompletteHandler = onCompleteHandler;
            _onErrorHandler = onErrorHandler;
            // Buy the consumable product using its general identifier. Expect a response either 
            // through ProcessPurchase or OnPurchaseFailed asynchronously.
            //BuyProductID(kProductIDConsumable);
            BuyProductId(productId);
        }

        /*
        public void BuyNonConsumable()
        {
            // Buy the non-consumable product using its general identifier. Expect a response either 
            // through ProcessPurchase or OnPurchaseFailed asynchronously.
            BuyProductID(kProductIDNonConsumable);
        }
        */
        /*
        public void BuySubscription()
        {
            // Buy the subscription product using its the general identifier. Expect a response either 
            // through ProcessPurchase or OnPurchaseFailed asynchronously.
            // Notice how we use the general product identifier in spite of this ID being mapped to
            // custom store-specific identifiers above.
            BuyProductID(kProductIDSubscription);
        }
        */

        private void BuyProductId(string productId)
        {
            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                _isRealPurchase = true;

                // ... look up the Product reference with the general product identifier and the Purchasing 
                // system's products collection.
                Product product = _storeController.products.WithID(productId);

                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                    // asynchronously.
                    _storeController.InitiatePurchase(product);
                }
                // Otherwise ...
                else
                {
                    // ... report the product look-up failure situation  
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                }
            }
            // Otherwise ...
            else
            {
                // ... report the fact Purchasing has not succeeded initializing yet. Consider waiting longer or 
                // retrying initiailization.
                Debug.Log("BuyProductID FAIL. Not initialized.");
            }
        }


        // Restore purchases previously made by this customer. Some platforms automatically restore purchases, like Google. 
        // Apple currently requires explicit purchase restoration for IAP, conditionally displaying a password prompt.
        public void RestorePurchases()
        {
            // If Purchasing has not yet been set up ...
            if (!IsInitialized())
            {
                // ... report the situation and stop restoring. Consider either waiting longer, or retrying initialization.
                Debug.Log("RestorePurchases FAIL. Not initialized.");
                return;
            }

            // If we are running on an Apple device ... 
            if (Application.platform == RuntimePlatform.IPhonePlayer ||
                Application.platform == RuntimePlatform.OSXPlayer)
            {
                // ... begin restoring purchases
                Debug.Log("RestorePurchases started ...");

                // Fetch the Apple store-specific subsystem.
                var apple = _storeExtensionProvider.GetExtension<IAppleExtensions>();
                // Begin the asynchronous process of restoring purchases. Expect a confirmation response in 
                // the Action below, and ProcessPurchase if there are previously purchased products to restore.
                apple.RestoreTransactions((result) => {
                    // The first phase of restoration. If no more responses are received on ProcessPurchase then 
                    // no purchases are available to be restored.
                    Debug.Log("RestorePurchases continuing: " + result + ". If no further messages, no purchases available to restore.");
                });
            }
            // Otherwise ...
            else
            {
                // We are not running on an Apple device. No work is necessary to restore purchases.
                Debug.Log("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
            }
        }

        public List<Product> ProductList
        {
            get
            {
                return IsInitialized() ? _storeController.products.all.ToList() : new List<Product>();
            }
        }

        public Product GetProductById(string productId)
        {
            return IsInitialized() ? ProductList.Find((p) => p.definition.id == productId) : null;
        }

        //  
        // --- IStoreListener
        //

        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            // Purchasing has succeeded initializing. Collect our Purchasing references.
            Debug.Log("OnInitialized: PASS");

            // Overall Purchasing system, configured with products for this application.
            _storeController = controller;
            // Store specific subsystem, for accessing device-specific store features.
            _storeExtensionProvider = extensions;
        }


        public void OnInitializeFailed(InitializationFailureReason error)
        {
            // Purchasing set-up has not succeeded. Check error for reason. Consider sharing this reason with the user.
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }


        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            // A consumable product has been purchased by this user.
            /*
            if (String.Equals(args.purchasedProduct.definition.id, kProductIDConsumable, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                // The consumable item has been successfully purchased, add 100 coins to the player's in-game score.
                //ScoreManager.score += 100;
            }
            */

            /*
            // Or ... a non-consumable product has been purchased by this user.
            else if (String.Equals(args.purchasedProduct.definition.id, kProductIDNonConsumable, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                // TODO: The non-consumable item has been successfully purchased, grant this item to the player.
            }
            // Or ... a subscription product has been purchased by this user.
            else if (String.Equals(args.purchasedProduct.definition.id, kProductIDSubscription, StringComparison.Ordinal))
            {
                Debug.Log(string.Format("ProcessPurchase: PASS. Product: '{0}'", args.purchasedProduct.definition.id));
                // TODO: The subscription item has been successfully purchased, grant this to the player.
            }
            */
            // Or ... an unknown product has been purchased by this user. Fill in additional products here....

            //bool validPurchase = true; // Presume valid for platforms with no R.V.

            // Unity IAP's validation logic is only included on these platforms.
//#if (UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_OSX) && !UNITY_EDITOR
            // Prepare the validator with the secrets we prepared in the Editor
            // obfuscation window.
            /* OLD LOCAL VALIDATION
            var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
                AppleTangle.Data(), Application.identifier);

            try
            {
                // On Google Play, result has a single product ID.
                // On Apple stores, receipts contain multiple products.
                var result = validator.Validate(args.purchasedProduct.receipt);
                // For informational purposes, we list the receipt(s)
                //Debug.Log("Receipt is valid. Contents:");
                foreach (IPurchaseReceipt productReceipt in result)
                {
                    //Debug.Log(productReceipt.productID);
                    //Debug.Log(productReceipt.purchaseDate);
                    //Debug.Log(productReceipt.transactionID);
                }
            }
            catch (MissingStoreSecretException)
            {
                Debug.Log("Validtion not supported");
                validPurchase = true;
            }
            catch (IAPSecurityException)
            {
                Debug.Log("Invalid receipt, not unlocking content");
                validPurchase = false;

                if (OnErrorHandler != null)
                {
                    OnErrorHandler(args.purchasedProduct.definition.id);
                }
                Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
            }
            */
//#endif

            Debug.Log("Processing transaction: " + args.purchasedProduct.transactionID);

            try
            {
                // Deserialize receipt
//                GooglePurchase googleReceipt = GooglePurchase.FromJson(args.purchasedProduct.receipt);
//
//                // Invoke receipt validation
//                // This will not only validate a receipt, but will also grant player corresponding items
//                // only if receipt is valid.
//                if (PlayFabClientAPI.IsClientLoggedIn())
//                {
//                    PlayFabClientAPI.ValidateGooglePlayPurchase(new ValidateGooglePlayPurchaseRequest()
//                    {
//                        // Pass in currency code in ISO format
//                        CurrencyCode = args.purchasedProduct.metadata.isoCurrencyCode,
//                        // Convert and set Purchase price
//                        PurchasePrice = (uint)(args.purchasedProduct.metadata.localizedPrice * 100),
//                        // Pass in the receipt
//                        ReceiptJson = googleReceipt.PayloadData.json,
//                        // Pass in the signature
//                        Signature = googleReceipt.PayloadData.signature
//                    }, result =>
//                    {
//                        Debug.Log("Validation successful!");
//
//                        string productId = args.purchasedProduct.definition.id;
//
//                        if (OnCompletteHandler != null)
//                        {
//                            OnCompletteHandler(productId);
//                        }
//
//                        Product product = GetProductById(productId);
//
//                        _analyticsService.Transaction(productId,
//                            args.purchasedProduct.metadata.isoCurrencyCode, (float)product.metadata.localizedPrice);
//
//                        AnalyticState.IapAnalyticState.productId = productId;
//
//                        _analyticsService.Revenue(productId, 1, (float)product.metadata.localizedPrice);
//
//                        UserDBManager.user.Current.paidTotal += (float)product.metadata.localizedPrice;
//                        UserDBManager.user.Current.paidCurrency = product.metadata.isoCurrencyCode;
//                        UserDBManager.user.Current.Save();
//                    },
//                    error =>
//                    {
//#if UNITY_EDITOR
//                        if (OnCompletteHandler != null)
//                        {
//                            OnCompletteHandler(args.purchasedProduct.definition.id);
//                        }
//#else
//                        Debug.LogError("Validation failed: " + error.GenerateErrorReport());
//                        if (OnErrorHandler != null)
//                        {
//                            OnErrorHandler(args.purchasedProduct.definition.id);
//                        }
//
//                        Debug.LogError(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
//#endif
//                    });
//                }
//                else
//                {
                    
                    var productId = args.purchasedProduct.definition.id;

                    if (productId == PRODUCT_NO_ADS)
                    {
                        var purchase = args.purchasedProduct;

                        _onCompletteHandler?.Invoke(productId);

                        GiveNoAds();

                        if (_isRealPurchase)
                        {
                            IntegrationSubsystem.Instance.AnalyticsService.PaymentSucceed(productId,
                                purchase.metadata.isoCurrencyCode, (float) purchase.metadata.localizedPrice, "no_ads");

                            _isRealPurchase = false;
                        }
                    }

//                    AnalyticState.IapAnalyticState.productId = productId;

                    //_analyticsService.Revenue(productId, 1, (float)product.metadata.localizedPrice);

/*                    UserDBManager.user.Current.paidTotal += (float)product.metadata.localizedPrice;
                    UserDBManager.user.Current.paidCurrency = product.metadata.isoCurrencyCode;
                    UserDBManager.user.Current.Save();*/
//                }
            }
            catch (Exception exception)
            {
                Debug.LogError(exception);

                var productId = args.purchasedProduct.definition.id;

                _onCompletteHandler?.Invoke(productId);
            }
            // Return a flag indicating whether this product has completely been received, or if the application needs 
            // to be reminded of this purchase at next app launch. Use PurchaseProcessingResult.Pending when still 
            // saving purchased products to the cloud, and when that save is delayed. 
            return PurchaseProcessingResult.Complete;
        }

        public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            // A product purchase attempt did not succeed. Check failureReason for more detail. Consider sharing 
            // this reason with the user to guide their troubleshooting actions.

            _onErrorHandler?.Invoke(product.definition.storeSpecificId);

            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        }

        public void GiveNoAds()
        {
            _playerDataService.NoAds = true;
            IntegrationSubsystem.Instance.AdsService.HideBanner();
            Dispatch(NotificationType.PurchaseNoAdsComplete);
        }
    }

    public class JsonData
    {
        // Json Fields, ! Case-sensetive

        public string orderId;
        public string packageName;
        public string productId;
        public long purchaseTime;
        public int purchaseState;
        public string purchaseToken;
    }

    public class PayloadData
    {
        public JsonData JsonData;

        // Json Fields, ! Case-sensetive
        public string signature;
        public string json;

        public static PayloadData FromJson(string json)
        {
            var payload = JsonUtility.FromJson<PayloadData>(json);
            payload.JsonData = JsonUtility.FromJson<JsonData>(payload.json);
            return payload;
        }
    }

    public class GooglePurchase
    {
        public PayloadData PayloadData;

        // Json Fields, ! Case-sensetive
        public string Store;
        public string TransactionID;
        public string Payload;

        public static GooglePurchase FromJson(string json)
        {
            var purchase = JsonUtility.FromJson<GooglePurchase>(json);
            purchase.PayloadData = PayloadData.FromJson(purchase.Payload);
            return purchase;
        }
    }
}