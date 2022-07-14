using UnityEngine;

public static class MonoBehaviourExtension
{
    public static void StopCoroutineSafe(this MonoBehaviour mb, Coroutine coroutine)
    {
        if (coroutine == null)
        {
            return;
        }
        
        mb.StopCoroutine(coroutine);
    }
}