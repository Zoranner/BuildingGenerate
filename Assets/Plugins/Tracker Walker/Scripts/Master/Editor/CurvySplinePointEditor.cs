using UnityEditor;
using UnityEngine;

namespace Longston.TrackerWalker
{
    [CustomEditor(typeof(CurvySplinePoint)), CanEditMultipleObjects]
    public class CurvySplinePointEditor : Editor
    {
        [System.Obsolete]
        private void OnSceneGUI()
        {
            CurvySplinePoint curvyPoint = (CurvySplinePoint)target;
            if (curvyPoint.transform.parent != null)
            {
                if (curvyPoint.transform.parent.GetComponent<CurvySpline>() != null)
                {
                    Vector3 position = curvyPoint.transform.position;
                    Vector3 controlPointIn = curvyPoint.ControlIn + position;
                    Vector3 controlPointOut = curvyPoint.ControlOut + position;
                    float handleInSize = HandleUtility.GetHandleSize(controlPointIn);
                    float handleOutSize = HandleUtility.GetHandleSize(controlPointOut);

                    // 生成名称标签
                    if (controlPointIn != position)
                    {
                        Handles.color = Color.red;
                        Handles.CubeCap(0, controlPointIn, Quaternion.identity, handleInSize * 0.1F);
                        Handles.DrawBezier(position, controlPointIn, position, controlPointIn, Color.red, null, 5);
                        curvyPoint.ControlIn = Handles.PositionHandle(controlPointIn, Quaternion.identity) - position;
                    }
                    if (controlPointOut != position)
                    {
                        Handles.color = Color.green;
                        Handles.CubeCap(0, controlPointOut, Quaternion.identity, handleOutSize * 0.1F);
                        Handles.DrawBezier(position, controlPointOut, position, controlPointOut, Color.green, null, 5);
                        curvyPoint.ControlOut = Handles.PositionHandle(controlPointOut, Quaternion.identity) - position;
                    }
                    //EditorUtility.SetDirty(curvyPoint);

                    // 通过创建一个新的ControlID我们可以把鼠标输入的Scene视图反应权从Unity默认的行为中抢过来
                    // FocusType.Passive意味着这个控制权不会接受键盘输入而只关心鼠标输入
                    int controlId = GUIUtility.GetControlID(FocusType.Passive);
                    // 把我们自己的controlId添加到默认的control里，这样Unity就会选择我们的控制权而非Unity默认的Scene视图行为
                    HandleUtility.AddDefaultControl(controlId);
                    Event newPointEvent = Event.current;
                    if (newPointEvent.control)
                    {
                        if (newPointEvent.type == EventType.MouseDown)
                        {
                            Camera screenCamera = SceneView.currentDrawingSceneView.camera;
                            Vector2 screenPoint = new Vector2(newPointEvent.mousePosition.x, screenCamera.pixelHeight - newPointEvent.mousePosition.y);
                            Ray ray = screenCamera.ScreenPointToRay(screenPoint);
                            if (Physics.Raycast(ray, out RaycastHit hit))
                            {
                                GameObject newPoint = new GameObject();
                                newPoint.transform.position = hit.point;
                                newPoint.transform.parent = curvyPoint.transform.parent;
                                Selection.activeGameObject = newPoint;
                            }
                        }
                    }
                }
            }
        }
    }
}
