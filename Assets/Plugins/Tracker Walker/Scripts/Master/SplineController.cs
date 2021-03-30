using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Longston.TrackerWalker
{
    [ExecuteInEditMode]
    public class SplineController : MonoBehaviour
    {
        public CurvySpline Spline;
        public float Location;

        void Update()
        {
            if (Spline != null)
            {
                if (Location < 0)
                    Location = 0;
                else if (Location > Spline.Distance)
                    Location = Spline.Distance;

                int keyPointIndex = BinarySearchIndex(Location, Spline.KeyPoint);
                float lerpDis = TWKernel.PolyPointDistance(Spline.KeyPoint, keyPointIndex + 1) - TWKernel.PolyPointDistance(Spline.KeyPoint, keyPointIndex);
                float lerpT = (Location - TWKernel.PolyPointDistance(Spline.KeyPoint, keyPointIndex)) / lerpDis;

                if (keyPointIndex < Spline.KeyPoint.Count - 1)
                {
                    transform.position = Vector3.Lerp(Spline.KeyPoint[keyPointIndex], Spline.KeyPoint[keyPointIndex + 1], lerpT);
                    transform.LookAt(Spline.KeyPoint[keyPointIndex + 1]);
                }
                else
                {
                    transform.position = Spline.KeyPoint[Spline.KeyPoint.Count - 1];
                }
            }
        }

        public int BinarySearchIndex(float location, List<Vector3> keyPoint)
        {
            int start = 0;
            int end = keyPoint.Count;
            do
            {
                int middle = (start + end) / 2;
                float middleDis = TWKernel.PolyPointDistance(keyPoint, middle);
                if(middleDis > location) end = middle - 1; else start = middle + 1;
                //Debug.LogError(string.Format("{0}|{1}", start, end));
            }
            while (start <= end);
            return start - 1;
        }
    }
}