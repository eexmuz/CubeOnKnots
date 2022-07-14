namespace Core
{
    public enum NotificationType
    {
        /// <summary>
        ///     None notification type
        /// </summary>
        Null,

        /// <summary>
        ///     Send this notification along with ShowAlertNotificationParams to display an alert.
        /// </summary>
        ShowAlert,

        /// <summary>
        ///     The user interface blocking operation started.
        /// </summary>
        UiBlockingOperationStart,

        /// <summary>
        ///     The user interface blocking operation ended.
        /// </summary>
        UiBlockingOperationEnd,

        /// <summary>
        ///     This notification should be sent along with ShowDialogNotificationParams whenever a new dialog should be displayed.
        /// </summary>
        ShowView,

        /// <summary>
        ///     This notificatoin is sent when a dialog is closed
        /// </summary>
        CloseView,
        LoadingLogoFaded,

        LanguageChanged,
        
        RateUsDialogClosed,
        PrivacyDialogClosed,
        PurchaseNoAdsComplete,

        ToggleSoundState,
        ToggleMusicState,
        
        DisplayingDialogStateChanged,
        
        PauseStateChanged,
        OpenLevelFromMenu,
        LoadNewLevel,
        LoadNextLevel,
        LevelEnd,
        LevelLoaded,
        LevelFailed,
        GenerateRandomLevel,
        MovesCounterChanged,
        BlocksMerge,
        PlayerReachedTargetNumber,
        RestartLevel,
        LoadSavedLevel,
        BlurGame,
        RestartConfirmed,
        BoardSetupComplete,
    }
}