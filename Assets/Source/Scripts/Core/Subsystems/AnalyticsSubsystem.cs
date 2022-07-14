// AnalyticsSubsystem.cs
//
// Unauthorized copying of this file, via any medium is strictly prohibited.
// Proprietary and confidential.
//
// 
// -------------------------------------------------------------------------

using System.Collections.Generic;
using Core.Services;

namespace Core.Subsystems
{
    /// <summary>
    /// Responsible for providing a singular API for the entire application
    /// to make analytics calls through. This system will implement calls to
    /// many analytics sources using a single point of access.
    /// </summary>
    public class AnalyticsSubsystem : Subsystem
    {

        #region Hidden Members
        /// <summary>
        /// 
        /// </summary>
        IAnalyticsService _analyticsService;
        #endregion

        #region Monobehaviour
        /// <summary>
        /// Used to initialize any variables or game state before the game starts.
        /// Only once during the lifetime of the script instance. 
        /// </summary>
        void Awake()
        {
            _analyticsService = gameObject.AddComponent<AnalyticsService>();

            Services = new List<IService>
            {
                _analyticsService
            };
        }
        #endregion
    }
}
