using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Longston.TrackerWalker
{
    [ExecuteInEditMode]
    public class CurvyCross : MonoBehaviour
    {
        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i) != null)
                {
                    if (transform.GetChild(i).GetComponent<CurvyCrossPoint>() == null)
                        transform.GetChild(i).gameObject.AddComponent<CurvyCrossPoint>();
                    transform.GetChild(i).GetComponent<CurvyCrossPoint>().PointIndex = i;
                    transform.GetChild(i).transform.name = string.Format("CCP{0}", i.ToString("D5"));
                }
            }
        }
    }
}
