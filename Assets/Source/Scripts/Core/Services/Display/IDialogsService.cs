using System;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Services
{
    public interface IDialogsService : IService
    {
        #region Public Properties

        Vector2 DialogsRootSize { get; set; }
        
        bool IsDisplayingDialog { get; set; }
        
        public GraphicRaycaster Raycaster { get; set; }
        
        #endregion

        #region Public Methods and Operators

        GameObject AddViewFromPrefab(GameObject viewPrefab);

        void CloseDialog(string id);

        void CloseDialogByName(ViewName store, bool instant = false);

        void SuppressUI(float duration = .2f);

        IDialogController CreateDialog(ViewName viewName, ViewCreationOptions options = ViewCreationOptions.None);

        IDialogController GetLastDialog();

        IDialogController GetOpenedDialogByName(ViewName viewName);

        Transform GetScreensRootTransform();
        Transform GetWidgetsRootTransform();
        Transform GetDialogsRootTransform();
        Transform GetLoaderRootTransform();
        Transform GetLockerRootTransform();
        IDialogsService SetScreensRootTransform(Transform rootTransform);
        IDialogsService SetWidgetsRootTransform(Transform rootTransform);
        IDialogsService SetDialogsRootTransform(Transform rootTransform);
        IDialogsService SetLoaderRootTransform(Transform rootTransform);
        IDialogsService SetLockerRootTransform(Transform rootTransform);

        #endregion
    }

    [Flags]
    public enum ViewCreationOptions
    {
        None = 0,

        /// <summary>
        ///     Create the dialog and puts it into the pool
        /// </summary>
        CreateWithoutDisplaying = 0x00000001,

        /// <summary>
        ///     Indicates if the dialog should be displayed with animation or immediately
        /// </summary>
        DisplayWithoutAnimation = 0x00000002,

        /// <summary>
        ///     Indicates that the dialog does not require a shroud.
        /// </summary>
        NoShroud = 0x00000004,

        /// <summary>
        ///     Don't Remove after Scene changed
        /// </summary>
        DontRemoveOnSceneChange = 0x00000008
    }
}