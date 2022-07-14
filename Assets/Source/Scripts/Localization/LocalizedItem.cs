using Core;
using Core.Attributes;
using TMPro;

public class LocalizedItem : DIBehaviour
{
    #region Fields

    public string StringId = string.Empty;

    [Inject] private ILocaleService _localeService;

    #endregion

    #region Methods

    protected override void OnAppInitialized()
    {
        base.OnAppInitialized();

        Subscribe(NotificationType.LanguageChanged, OnLanguageChanged);

        ApplyLocalizedString();
    }

    private void ApplyLocalizedString()
    {
        if (string.IsNullOrEmpty(StringId) == false)
        {
            var localizedString = _localeService.GetString(StringId);
            SetString(string.IsNullOrEmpty(localizedString) == false ? localizedString : StringId);
        }
    }

    private void OnLanguageChanged(NotificationType notificationType, object notificationParams)
    {
        ApplyLocalizedString();
    }

    private void SetString(string text)
    {
        var label = GetComponent<TextMeshProUGUI>();
        if (label != null)
            label.text = text;
    }

    #endregion
}