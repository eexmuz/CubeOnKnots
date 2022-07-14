using System.Collections.Generic;
using System.Linq;
using Core.Attributes;
using Core.Services;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using Utility;

namespace Core.Settings
{
    [CreateAssetMenu(menuName = "Settings/Game Settings")]
    public class GameSettings : ScriptableObject, ISettings
    {
        #region Fields

        /// <summary>
        ///     Prefab used for loading the game
        /// </summary>
        public GameObject loader;

        public List<LevelConfig> Levels;
        public BlockColorSettings BlockColors;

        public Block BlockPrefab;
        public Vector2 CellSize;

        public float SwipeAnimationDuration;

        #endregion

    }
}