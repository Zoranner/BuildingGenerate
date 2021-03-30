using System.Collections.Generic;
using UnityEngine;

namespace Longston.TrackerWalker
{
    [ExecuteInEditMode]
    public class CurvySpline : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        private float _Distance;
        public float Distance
        {
            set => _Distance = value;
            get => _Distance;
        }

        [HideInInspector]
        [SerializeField]
        private List<Vector3> _KeyPoint;
        public List<Vector3> KeyPoint
        {
            set => _KeyPoint = value;
            get => _KeyPoint;
        }

        private void Start()
        {
            if (GameObject.Find("Tracker Walker") == null)
            {
                GameObject trackerWalker = new GameObject
                {
                    name = "Tracker Walker"
                };
                if (trackerWalker.GetComponent<PublicSetting>() == null)
                {
                    trackerWalker.AddComponent<PublicSetting>();
                }
            }
        }

        private void OnDrawGizmos()
        {
            Distance = 0;
            KeyPoint = new List<Vector3>();
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i) != null)
                {
                    if (transform.GetChild(i).GetComponent<CurvySplinePoint>() == null)
                    {
                        transform.GetChild(i).gameObject.AddComponent<CurvySplinePoint>();
                    }

                    transform.GetChild(i).GetComponent<CurvySplinePoint>().Index = i;
                    transform.GetChild(i).transform.name = string.Format("CSP{0}", i.ToString("D5"));
                    Distance += transform.GetChild(i).GetComponent<CurvySplinePoint>().Distance;
                    if (transform.GetChild(i).GetComponent<CurvySplinePoint>().KeyPoint != null)
                    {
                        for (int j = 1; j < transform.GetChild(i).GetComponent<CurvySplinePoint>().KeyPoint.Count; j++)
                        {
                            KeyPoint.Add(transform.GetChild(i).GetComponent<CurvySplinePoint>().KeyPoint[j]);
                        }
                    }
                }
            }
        }
    }
}