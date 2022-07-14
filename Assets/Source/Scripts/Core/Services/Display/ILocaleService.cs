using System;
using System.Collections.Generic;
using Core;
using Core.Services;
using TMPro;
using Utility;

public interface ILocaleService : IService
{
    #region Public Properties

    Dictionary<string, string> AvailableLanguages { get; set; }
    string CurrentLanguage { get; set; }

    List<Pair<TextMeshProUGUI, Func<string>>> LocalizedFieldAction { get; set; }

    #endregion

    #region Public Methods and Operators

    void AddLocalizedFieldAction(TextMeshProUGUI textField, Func<string> func);

    string GetString(string localeId);

    string[] GetStringArray(string localeId, char delimiter = ';');

    void SetLanguage(string lang, Action callback = null);

    void SetLocale(LocaleData localeVO);

    void SetString(string localeId, string value = "");

    #endregion
}