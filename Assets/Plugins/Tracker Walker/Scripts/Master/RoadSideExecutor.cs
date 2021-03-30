using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Longston.TrackerWalker
{
    public class RoadSideExecutor : SplineController
    {
        [HideInInspector]
        [SerializeField]
        int mIdentifier;
        public int Identifier
        {
            set { mIdentifier = value; }
            get { return mIdentifier; }
        }

        [HideInInspector]
        [SerializeField]
        SignalType mType;
        public SignalType Type
        {
            set
            {
                mType = value;
            }
            get { return mType; }
        }

        public enum SignalType { GetIn, GetOut, Notice }
    }
}
