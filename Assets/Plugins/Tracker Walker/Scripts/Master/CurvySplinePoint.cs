using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Longston.TrackerWalker
{
    [ExecuteInEditMode]
    public class CurvySplinePoint : MonoBehaviour
    {
        [HideInInspector]
        public int Index;
        public float Distance;
        [HideInInspector]
        public List<Vector3> KeyPoint;
        public bool AutoHandles = true;
        public float HandleRatio = 0.39F;
        public Vector3 ControlIn;
        public Vector3 ControlOut;

        [System.Obsolete]
        private void OnDrawGizmos()
        {
            Distance = 0;
            KeyPoint = new List<Vector3>();
            if (transform.parent != null)
            {
                if (transform.parent.GetComponent<CurvySpline>() != null)
                {
                    CurvySplinePoint lastPoint = Index > 0 ? transform.parent.GetChild(Index - 1).GetComponent<CurvySplinePoint>() : null;
                    CurvySplinePoint nextPoint = Index < transform.parent.childCount - 1 ? transform.parent.GetChild(Index + 1).GetComponent<CurvySplinePoint>() : null;
                    if (AutoHandles)
                    {
                        if (lastPoint != null && nextPoint != null)
                        {
                            ControlIn = (lastPoint.transform.position - nextPoint.transform.position).normalized * Vector3.Distance(lastPoint.transform.position, transform.position) * HandleRatio;
                            ControlOut = (nextPoint.transform.position - lastPoint.transform.position).normalized * Vector3.Distance(transform.position, nextPoint.transform.position) * HandleRatio;
                        }
                        else
                        {
                            ControlIn = Vector3.zero;
                            ControlOut = Vector3.zero;
                        }
                    }
                    if (lastPoint == null)
                    {
                        lastPoint = this;
                    }

                    Distance = TWKernel.OptimalDistance(lastPoint.transform.position, lastPoint.ControlOut, ControlIn, transform.position, out KeyPoint);
                    transform.rotation = transform.rotation != Quaternion.identity ? Quaternion.identity : Quaternion.identity;
                }
            }
            float handleSize = HandleUtility.GetHandleSize(transform.position);
            Camera screenCamera = SceneView.currentDrawingSceneView.camera;
            Vector3 screenPoint = screenCamera.WorldToScreenPoint(transform.position);
            Rect rectMouseIn = new Rect(screenPoint.x - 12, screenCamera.pixelHeight - screenPoint.y - 12, 24, 24);
            if (KeyPoint != null)
            {
                Handles.color = rectMouseIn.Contains(Event.current.mousePosition) ? Color.gray : Color.white;
                Vector3[] splineKeyPointArray = new Vector3[KeyPoint.Count];
                KeyPoint.CopyTo(splineKeyPointArray);
                Handles.DrawAAPolyLine(5, splineKeyPointArray);
                for (int i = 0; i < KeyPoint.Count; i++)
                {
                    Handles.CubeCap(0, KeyPoint[i], Quaternion.identity, 0.01F);
                }
            }
            Handles.SphereCap(0, transform.position, Quaternion.identity, handleSize * 0.2F);
            Handles.Label(transform.position, string.Format("{0}", transform.name));
        }
    }

    public class SplineKeyPointList
    {
        public Vector3 Position;
        public float Location;
    }
}