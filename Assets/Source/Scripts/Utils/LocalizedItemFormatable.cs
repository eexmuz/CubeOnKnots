using System.Collections;
using System.Text;
using Core;
using Core.Attributes;
using TMPro;

public class LocalizedItemFormatable : DIBehaviour
{
    public string StringId = string.Empty;

    [Inject] 
    private ILocaleService _localeService;

    private TextMeshProUGUI _label;
    
    private string GetLocalizedString()
    {
        if (string.IsNullOrEmpty(StringId))
        {
            return StringId;
        }
        
        string localizedString = _localeService.GetString(StringId);
        return string.IsNullOrEmpty(localizedString) == false ? localizedString : StringId;
    }

    public void FormatLocalizedText(string prefix, string postfix)
    {
        if (_localeService == null)
        {
            StartCoroutine(FormatLocalizedText_co(prefix, postfix));
            return;
        }
        
        FormatLocalizedTextInternal(prefix, postfix);
    }

    private void FormatLocalizedTextInternal(string prefix, string postfix)
    {
        _label ??= GetComponent<TextMeshProUGUI>();
        
        StringBuilder builder = new StringBuilder();
        
        if (string.IsNullOrEmpty(prefix) == false)
        {
            builder.Append(prefix);
        }

        builder.Append(GetLocalizedString());
        
        if (string.IsNullOrEmpty(postfix) == false)
        {
            builder.Append(postfix);
        }
        
        _label.text = builder.ToString();
    }

    private IEnumerator FormatLocalizedText_co(string prefix, string postfix)
    {
        yield return WaitForStarted();
        FormatLocalizedTextInternal(prefix, postfix);
    }
}