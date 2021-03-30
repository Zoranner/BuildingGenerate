using UnityEditor;
using UnityEngine;

namespace Longston.TrackerWalker
{
    public static class TWUIStyle
    {
        public static void BeginGroup(string title)
        {
            GUILayout.Space(10);
            GUILayout.BeginVertical("box");
            GUILayout.Label(title, EditorStyles.boldLabel);
            GUILayout.Space(5);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            GUILayout.BeginVertical();
        }

        public static void EndGroup()
        {
            GUILayout.EndVertical();
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            GUILayout.EndVertical();
        }
    }
}
