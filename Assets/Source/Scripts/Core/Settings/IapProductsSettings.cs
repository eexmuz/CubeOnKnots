using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Settings
{
    [CreateAssetMenu(menuName = "Settings/IAP Products Settings")]
    public class IapProductsSettings : ScriptableObject, ISettings
    {
        #region Fields

        public List<ExchangeProduct> exchangeProductsList;

        public List<IapProduct> iapProductsList;

        #endregion

        #region Public Methods and Operators

        public IapProduct GetIapProductById(string productId)
        {
            return iapProductsList.First(p => p.productId == productId);
        }

        #endregion
    }

    [Serializable]
    public class ExchangeProduct
    {
        #region Fields

        public bool bestValue;

        public Sprite icon;
        
        public bool mostPopular;

        public string productName;

        public override string ToString()
        {
            return productName;
        }

        #endregion
    }

    [Serializable]
    public class IapProduct
    {
        #region Fields

        public bool bestValue;

        //public MoneyType MoneyType;

        public int count;

        public Sprite icon;

        public bool mostPopular;
        public string productId;

        public string productName;

        public override string ToString()
        {
            return productName;
        }
        
        #endregion
    }
}