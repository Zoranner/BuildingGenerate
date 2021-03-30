using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Longston.TrackerWalker
{
    [ExecuteInEditMode]
    public class RoadSideController : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        CurvySpline mSpline;
        public CurvySpline Spline
        {
            set
            {
                mSpline = value;
                for (int i = 0; i < transform.childCount; i++)
                    transform.GetChild(i).GetComponent<RoadSideExecutor>().Spline = value;
            }
            get { return mSpline; }
        }


        [HideInInspector]
        [SerializeField]
        List<GameObject> mSideObject;
        public List<GameObject> SideObject
        {
            set { mSideObject = value; }
            get { return mSideObject; }
        }

        [HideInInspector]
        [SerializeField]
        GameObject mGetInObject;
        public GameObject GetInObject
        {
            set
            {
                mGetInObject = value;
                UpdateSignal(RoadSideExecutor.SignalType.GetIn, value);
            }
            get { return mGetInObject; }
        }

        [HideInInspector]
        [SerializeField]
        GameObject mGetOutObject;
        public GameObject GetOutObject
        {
            set
            {
                mGetOutObject = value;
                UpdateSignal(RoadSideExecutor.SignalType.GetOut, value);
            }
            get { return mGetOutObject; }
        }

        [HideInInspector]
        [SerializeField]
        GameObject mNoticeObject;
        public GameObject NoticeObject
        {
            set
            {
                mNoticeObject = value;
                UpdateSignal(RoadSideExecutor.SignalType.Notice, value);
            }
            get { return mNoticeObject; }
        }

        void OnDrawGizmos()
        {
            for(int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).name = string.Format("SideObject_{0}", i.ToString("D4"));
                if(transform.GetChild(i).GetComponent<RoadSideExecutor>() == null)
                    transform.GetChild(i).gameObject.AddComponent<RoadSideExecutor>();
            }
        }

        public void AddSignal()
        {
            GameObject newSignal = new GameObject();
            newSignal.transform.parent = transform;
            newSignal.AddComponent<RoadSideExecutor>();
        }

        public void UpdateSignal(RoadSideExecutor.SignalType type, GameObject go)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                RoadSideExecutor signExecutor = transform.GetChild(i).GetComponent<RoadSideExecutor>();
                if (signExecutor.Type == type)
                {
                    while (signExecutor.transform.childCount > 0)
                    {
                        GameObject se = signExecutor.transform.GetChild(0).gameObject;
                        if (Application.isEditor) DestroyImmediate(se);
                        else Destroy(se);
                    }
                    if (go != null)
                    {
                        GameObject newSignal = Instantiate(go) as GameObject;
                        newSignal.transform.parent = signExecutor.transform;
                        newSignal.transform.position = signExecutor.transform.position;
                        newSignal.transform.rotation = signExecutor.transform.rotation;
                    }
                }
            }
        }

        public void ClearAllSignals()
        {
            // 销毁所有子物体
            while (transform.childCount > 0)
            {
                GameObject se = transform.GetChild(0).gameObject;
                if (Application.isEditor) DestroyImmediate(se);
                else Destroy(se);
            }
        }
    }
}