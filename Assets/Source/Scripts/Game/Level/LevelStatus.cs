using UnityEngine;

[System.Serializable]
public class LevelStatus
{
    public bool Unlocked;
    
    [HideInInspector]
    public int Stars;

    [HideInInspector]
    public bool Complete;
}