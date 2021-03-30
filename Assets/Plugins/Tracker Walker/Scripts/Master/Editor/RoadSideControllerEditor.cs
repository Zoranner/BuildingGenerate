using UnityEditor;
using UnityEngine;

namespace Longston.TrackerWalker
{
    [CustomEditor(typeof(RoadSideController))]
    public class SideObjectControllerEditor : Editor
    {
        RoadSideController _SignalController;

        public override void OnInspectorGUI()
        {
            _SignalController = (RoadSideController)serializedObject.targetObject;

            TWUIStyle.BeginGroup("Signal Object"); // 开始

            GUILayout.Label("Spline");
            _SignalController.Spline = (CurvySpline)EditorGUILayout.ObjectField(_SignalController.Spline, typeof(CurvySpline), true);

            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.Label("Get In");
            _SignalController.GetInObject = (GameObject)EditorGUILayout.ObjectField(_SignalController.GetInObject, typeof(GameObject), true);
            EditorGUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label("Get Out");
            _SignalController.GetOutObject = (GameObject)EditorGUILayout.ObjectField(_SignalController.GetOutObject, typeof(GameObject), true);
            EditorGUILayout.EndVertical();

            GUILayout.BeginVertical();
            GUILayout.Label("Notice");
            _SignalController.NoticeObject = (GameObject)EditorGUILayout.ObjectField(_SignalController.NoticeObject, typeof(GameObject), true);
            EditorGUILayout.EndVertical();

            EditorUtility.SetDirty(_SignalController);

            GUILayout.EndHorizontal();

            TWUIStyle.EndGroup(); // 结束

            TWUIStyle.BeginGroup("Signal Object"); // 开始

            if (_SignalController.transform.childCount != 0)
            {
                GUILayout.BeginHorizontal();
                EditorGUILayout.TextField("SignType", EditorStyles.label);
                EditorGUILayout.TextField("Identifier", EditorStyles.label);
                EditorGUILayout.TextField("Location", EditorStyles.label);
                if(GUILayout.Button("D", EditorStyles.miniButton))
                {
                    EditorApplication.Beep();
                    if (EditorUtility.DisplayDialog("Really?", "Do you really want to remove the state?", "Yes", "No") == true)
                        _SignalController.ClearAllSignals();
                }
                GUILayout.EndHorizontal();
                for (int i = 0; i < _SignalController.transform.childCount; i++)
                {
                    RoadSideExecutor signalExecutor = _SignalController.transform.GetChild(i).GetComponent<RoadSideExecutor>();
                    if (signalExecutor != null)
                    {
                        GUILayout.BeginHorizontal();

                        string[] popup = new string[3];
                        for(int ii = 0; ii < 3; ii++)
                        {
                            popup[ii] = string.Format("string{0}", ii);
                        }
                        EditorGUILayout.Popup(0, popup);

                        signalExecutor.Type = (RoadSideExecutor.SignalType)EditorGUILayout.EnumPopup(signalExecutor.Type);
                        signalExecutor.Identifier = EditorGUILayout.IntField(signalExecutor.Identifier);
                        signalExecutor.Location = EditorGUILayout.FloatField(signalExecutor.Location);

                        if (GUILayout.Button("D", EditorStyles.miniButton))
                        {
                            GameObject se = _SignalController.transform.GetChild(i).gameObject;
                            if (Application.isEditor) DestroyImmediate(se);
                            else Destroy(se);
                            EditorUtility.SetDirty(_SignalController);
                        }

                        GUILayout.EndHorizontal();
                    }
                }
            }
            else GUILayout.Label(string.Format("No signal here."));

            GUILayout.Space(5);

            if (GUILayout.Button("Add"))
                _SignalController.AddSignal();
            
            TWUIStyle.EndGroup(); // 结束

            GUILayout.Space(5);
        }
    }
}