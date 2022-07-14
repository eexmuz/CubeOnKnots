// IStoreService.cs
//
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.
//
// 
// -------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEngine.Purchasing;

namespace Core.Services
{
    public interface IInAppPurchaseService : IService, IStoreListener
    {
        void InitializePurchasing();

        void BuyProduct(string productId, Action<string> onCompleteHandler = null, Action<string> onErrorHandler = null);

        void RestorePurchases();

        List<Product> ProductList { get; }

        Product GetProductById(string productId);
    }
}

