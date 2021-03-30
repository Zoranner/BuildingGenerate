using System;
using System.Collections.Generic;
using UnityEngine;

namespace Longston.TrackerWalker
{
    public static class TWKernel
    {

        /// <summary>
        /// 两点贝塞尔曲线最优距离
        /// </summary>
        /// <param name="thisCP"></param>
        /// <param name="nextCP"></param>
        /// <returns></returns>
        public static float OptimalDistance(Vector3 P0, Vector3 T0, Vector3 T1, Vector3 P1, out List<Vector3> keyPoint)
        {
            int degree = 0;
            keyPoint = null;
            float optimalDistance = 0;
            float thisDis = 0; float lastDis = 0;
            do
            {
                degree++;
                thisDis = PolyPointDistance(InterPolation(P0, T0, T1, P1, degree));
                lastDis = PolyPointDistance(InterPolation(P0, T0, T1, P1, degree - 1));
            }
            while (Math.Abs(lastDis - thisDis) > 0.00001);
            keyPoint = InterPolation(P0, T0, T1, P1, degree);
            optimalDistance = thisDis;
            return optimalDistance;
        }

        /// <summary>
        /// 多点连线的距离算法
        /// </summary>
        /// <param name="pos"></param>
        /// <returns>PolyPointDistance</returns>
        public static float PolyPointDistance(List<Vector3> pos)
        {
            float polyPointDistance = 0;
            for (int i = 1; i < pos.Count; i++)
                polyPointDistance += Vector3.Distance(pos[i - 1], pos[i]);
            return polyPointDistance;
        }

        public static float PolyPointDistance(List<Vector3> pos, int index)
        {
            index = index <= pos.Count ? index : pos.Count;
            float polyPointDistance = 0;
            for (int i = 1; i < index; i++)
            {
                //Debug.LogError(string.Format("{0}|{1}|{2}", i, pos.Count, index));
                polyPointDistance += Vector3.Distance(pos[i - 1], pos[i]);
            }
            return polyPointDistance;
        }

        /// <summary>
        /// 插值获得两点贝塞尔曲线上的坐标
        /// </summary>
        /// <param name="thisCP"></param>
        /// <param name="nextCP"></param>
        /// <param name="degree"></param>
        /// <returns></returns>
        private static List<Vector3> InterPolation(Vector3 P0, Vector3 T0, Vector3 T1, Vector3 P1, int degree)
        {
            Vector3 p0 = P0;
            Vector3 t0 = T0 + P0;
            Vector3 t1 = T1 + P1;
            Vector3 p1 = P1;
            List<Vector3> interPolation = new List<Vector3>();
            for (int t = 0; t <= degree; t++)
            {
                float f = (float)t / degree;
                interPolation.Add(Bezier(p0, t0, t1, p1, f));
            }
            return interPolation;
        }

        /// <summary>
        /// 贝塞尔曲线上任意点坐标算法
        /// </summary>
        /// <param name="T0"></param>
        /// <param name="P0"></param>
        /// <param name="P1"></param>
        /// <param name="T1"></param>
        /// <param name="f"></param>
        /// <returns>Bezier Position</returns>
        public static Vector3 Bezier(Vector3 P0, Vector3 T0, Vector3 T1, Vector3 P1, float f)
        {
            double Ft2 = 3; double Ft3 = -3;
            double Fu1 = 3; double Fu2 = -6; double Fu3 = 3;
            double Fv1 = -3; double Fv2 = 3;

            double FAX = -P0.x + Ft2 * T0.x + Ft3 * T1.x + P1.x;
            double FBX = Fu1 * P0.x + Fu2 * T0.x + Fu3 * T1.x;
            double FCX = Fv1 * P0.x + Fv2 * T0.x;
            double FDX = P0.x;

            double FAY = -P0.y + Ft2 * T0.y + Ft3 * T1.y + P1.y;
            double FBY = Fu1 * P0.y + Fu2 * T0.y + Fu3 * T1.y;
            double FCY = Fv1 * P0.y + Fv2 * T0.y;
            double FDY = P0.y;

            double FAZ = -P0.z + Ft2 * T0.z + Ft3 * T1.z + P1.z;
            double FBZ = Fu1 * P0.z + Fu2 * T0.z + Fu3 * T1.z;
            double FCZ = Fv1 * P0.z + Fv2 * T0.z;
            double FDZ = P0.z;

            float FX = (float)(((FAX * f + FBX) * f + FCX) * f + FDX);
            float FY = (float)(((FAY * f + FBY) * f + FCY) * f + FDY);
            float FZ = (float)(((FAZ * f + FBZ) * f + FCZ) * f + FDZ);

            return new Vector3(FX, FY, FZ);
        }
    }
}
