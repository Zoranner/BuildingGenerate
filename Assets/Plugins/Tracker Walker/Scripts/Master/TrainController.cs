using UnityEngine;

namespace Longston.TrackerWalker
{
    [ExecuteInEditMode]
    public class TrainController : SplineController
    {
        public float Speed;
        public float Times;

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
                transform.position = Vector3.Lerp(Spline.KeyPoint[keyPointIndex], Spline.KeyPoint[keyPointIndex + 1], lerpT);
                transform.LookAt(Spline.KeyPoint[keyPointIndex + 1]);
            }
        }

        void FixedUpdate()
        {
            Location += Speed / 180;
            Times = Time.time;
        }
    }
}