using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Longston.TrackerWalker
{
    public class PublicSetting : MonoBehaviour
    {
        [HideInInspector]
        [SerializeField]
        bool mSelectChanged;
        public bool SelectChanged
        {
            set { mSelectChanged = value; }
            get { return mSelectChanged; }
        }

        [HideInInspector]
        [SerializeField]
        GameObject mSelectedObject;
        public GameObject SelectedObject
        {
            set { mSelectedObject = value; }
            get { return mSelectedObject; }
        }
    }
}
