using UnityEngine;

public static class ComponentExtension
{
    /// <summary>
    /// Gets or add a component. Usage example:
    /// BoxCollider boxCollider = transform.GetOrAddComponent<BoxCollider>();
    /// </summary>
    public static T GetOrAddComponent<T>(this Component child, bool set_enable = false) where T : Component
    {
        T result = child.GetComponent<T>();
        if (result == null)
        {
            result = child.gameObject.AddComponent<T>();
        }
        Behaviour bcomp = result as Behaviour;
        if (set_enable)
        {
            if (bcomp != null)
            {
                bcomp.enabled = true;
            }
        }
        return result;
    }
}