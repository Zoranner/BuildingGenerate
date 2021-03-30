using UnityEditor;
using UnityEngine;

namespace Longston.TrackerWalker
{
    [CustomEditor(typeof(CurvySpline))]
    public class CurvySplineEditor : Editor
    {
        CurvySpline _CurvySpline;

        public override void OnInspectorGUI()
        {
            _CurvySpline = (CurvySpline)serializedObject.targetObject;

            TWUIStyle.BeginGroup("Spline Info");
            GUILayout.Label(string.Format("Degree: {0}\r\nDistance: {1}", _CurvySpline.KeyPoint.Count, _CurvySpline.Distance));
            TWUIStyle.EndGroup();
            //for(int i = 0; i < curvySpline.KeyPoint.Count; i++)
            //    EditorGUILayout.Vector3Field("", curvySpline.KeyPoint[i]);
        }
    }
}
