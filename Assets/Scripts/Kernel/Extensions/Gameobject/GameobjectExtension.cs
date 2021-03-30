using UnityEngine;

public static class GameobjectExtension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        T result = go.transform.GetComponent<T>();
        if (result == null)
        {
            result = go.AddComponent<T>();
        }
        Behaviour bcomp = result as Behaviour;
        if (bcomp != null)
        {
            bcomp.enabled = true;
        }

        return result;
    }
}