using System;
using System.Collections.Generic;
using Core.Attributes;
using Data;
using TMPro;
using UnityEngine;
using Utility;

namespace Core.Services
{
    public class LocaleData
    {
        #region Fields

        public readonly string LocaleId;
        public readonly string Text;

        #endregion

        #region Constructors and Destructors

        public LocaleData(string localeId, string text)
        {
            LocaleId = localeId;
            Text = text;
        }

        #endregion
    }

    [InjectionAlias(typeof(ILocaleService))]
    public class LocaleService : Service, ILocaleService
    {
        #region Fields

        private Dictionary<string, LocaleData> _locales;
        
        [Inject]
        private IPlayerPrefsService _playerPrefsService;

        #endregion

        #region Public Properties

        public Dictionary<string, string> AvailableLanguages { get; set; }

        public string CurrentLanguage { get; set; }

        public List<Pair<TextMeshProUGUI, Func<string>>> LocalizedFieldAction { get; set; }

        #endregion

        #region Public Methods and Operators

        public void AddLocalizedFieldAction(TextMeshProUGUI textField, Func<string> func)
        {
            LocalizedFieldAction.Add(new Pair<TextMeshProUGUI, Func<string>>(textField, func));
            textField.text = func();
        }

        public string GetString(string localeId)
        {
            //byte[] codes = System.Text.Encoding.ASCII.GetBytes(locales[localeId].Text);
            if (_locales.ContainsKey(localeId)) return _locales[localeId].Text;
            return "";
        }

        public string[] GetStringArray(string localeId, char delimiter = ';')
        {
            return GetString(localeId).Split(delimiter);
        }

        public override void Run()
        {
            base.Run();

            _locales = new Dictionary<string, LocaleData>();

            LocalizedFieldAction = new List<Pair<TextMeshProUGUI, Func<string>>>();

            AvailableLanguages = new Dictionary<string, string>
            {
                {"English", "EN"},
                {"Russian", "RU"},
                {"German", "DE"},
                {"Spanish", "ES"},
                {"French", "FR"},
                {"Spanish(LATAM)", "ES(LATAM)"},
                {"Portuguese", "PT"},
                {"Portuguese(BR)", "PT(BR)"},
                {"Turkish", "TR"},
                {"Thai", "TH"}
                //{ "Korean", "KO" },
                //{ "Japanese", "JA" }
            };

            var language = "English";

            if (AvailableLanguages.ContainsKey(Application.systemLanguage.ToString()))
            {
                language = Application.systemLanguage.ToString();
            }

            SetLanguage(language);
        }

        public void SetLanguage(string lang = "English", Action callback = null)
        {
            _playerPrefsService.SetString(PlayerPrefsKeys.LANGUAGE, lang);

            CurrentLanguage = lang;
            AvailableLanguages.TryGetValue(lang, out var code);

            var txt = Resources.Load<TextAsset>("localization/text_" + code);

            var localeData = JSON.Parse(txt.text);

            for (var i = 0; i < localeData.Count; i++)
            {
                if (localeData[i]["text"] == null) localeData[i]["text"] = " ";
                SetLocale(new LocaleData(localeData[i]["id"], localeData[i]["text"]));
            }

            Debug.Log("Language changed: " + lang);

            Dispatch(NotificationType.LanguageChanged);
            RunLocalizedFieldAction();

            if (callback != null) callback();
        }

        public void SetLocale(LocaleData localeData)
        {
            if (_locales.ContainsKey(localeData.LocaleId))
                _locales[localeData.LocaleId] = localeData;
            else
                _locales.Add(localeData.LocaleId, localeData);
        }

        public void SetString(string localeId, string value = "")
        {
            SetLocale(new LocaleData(localeId, value));
        }

        #endregion

        #region Methods

        private void RunLocalizedFieldAction()
        {
            for (var i = 0; i < LocalizedFieldAction.Count; i++)
            {
                var pair = LocalizedFieldAction[i];
                if (pair.First == null)
                {
                    LocalizedFieldAction.RemoveAt(i);
                    i--;
                }
                else
                {
                    pair.First.text = pair.Second();
                }
            }
        }

        #endregion
    }
}