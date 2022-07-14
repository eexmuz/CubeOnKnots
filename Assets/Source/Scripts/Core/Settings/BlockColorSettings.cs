using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Block Color Settings", order = 0)]
public class BlockColorSettings : ScriptableObject
{
    [SerializeField]
    private List<BlockColor> _blockColors;

    private Dictionary<int, Color> _colors;

    private void OnValidate()
    {
        FillDictionary();
    }

    private void FillDictionary()
    {
        _colors = new Dictionary<int, Color>();
        foreach (var blockColor in _blockColors)
        {
            if (_colors.ContainsKey(blockColor.Value.POT))
            {
                continue;
            }
            
            _colors.Add(blockColor.Value.POT, blockColor.Color);
        }
    }

    public Color GetColor(IntPOT intPOT)
    {
        return GetColor(intPOT.POT);
    }
    
    public Color GetColor(int pot)
    {
        if (_colors == null)
        {
            FillDictionary();
        }
        
        return _colors.ContainsKey(pot) == false ? Color.white : _colors[pot];
    }
}
