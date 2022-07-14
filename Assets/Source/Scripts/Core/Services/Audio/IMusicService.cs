using System.Collections.Generic;
using UnityEngine;

namespace Core.Services
{
    public interface IMusicService : IService
    {
        #region Public Properties

        bool IsEnabled { get; set; }

        #endregion

        #region Public Methods and Operators

        void SetMusicState(bool state);

        #endregion
    }
}