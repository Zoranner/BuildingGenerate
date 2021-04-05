using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingBlock)), CanEditMultipleObjects]
public class BuildingBlockEditor : Editor
{
    private BuildingBlock _Target;
    private Transform _TargetTransform;

    private SceneView _SceneView;

    private Vector3 _TempPosition;
    private Quaternion _TempRotation;

    private Vector3 _RulerRangeVector;
    private Vector3 _RulerValueVector;
    private Vector3 _ThisRulerRangePoint;
    private Vector3 _NextRulerRangePoint;
    private Vector3 _ThisRulerValuePoint;
    private Vector3 _NextRulerValuePoint;
    private Vector3 _RulerValueLabelPoint;
    private Vector3 _RulerValueLabelOffset;

    private void OnEnable()
    {
        _Target = (BuildingBlock)target;
        _TargetTransform = _Target.transform;
        _SceneView = EditorWindow.GetWindow<SceneView>();
    }

    private void OnSceneGUI()
    {
        var handleSize = HandleUtility.GetHandleSize(_TargetTransform.position);
        Handles.color = Color.white;

        if (_TempPosition != _TargetTransform.position)
        {
            for (var i = 0; i < 4; i++)
            {
                _Target.ParcelPoints[i] = _Target.ParcelPointsLocal[i] + _TargetTransform.position;
            }

            _Target.UpdateParcelPoints();

            _TempPosition = _TargetTransform.position;
        }

        for (var i = 0; i < 4; i++)
        {
            //_Target.SetParcelPointsLocal(i, Handles.PositionHandle(_Target.ParcelPoints[i], Quaternion.identity) - _TargetTransform.position);
            _Target.SetParcelPoints(i, Handles.PositionHandle(_Target.ParcelPoints[i], Quaternion.identity));

            _RulerRangeVector = _Target.VerticalVector3Ns[i] * handleSize * 0.4f;
            _RulerValueVector = _Target.VerticalVector3Ns[i] * handleSize * 0.2f;
            _RulerValueLabelOffset = _Target.NextVector3Ns[i] * handleSize * 0.5f;

            _ThisRulerRangePoint = _Target.ParcelPoints[i] - _RulerRangeVector;
            _NextRulerRangePoint = _Target.ParcelPoints.RepeatGet(i + 1) - _RulerRangeVector;
            _ThisRulerValuePoint = _Target.ParcelPoints[i] - _RulerValueVector;
            _NextRulerValuePoint = _Target.ParcelPoints.RepeatGet(i + 1) - _RulerValueVector;
            _RulerValueLabelPoint = (_ThisRulerValuePoint + _NextRulerValuePoint) / 2;
            Handles.DrawLine(_Target.ParcelPoints[i], _ThisRulerRangePoint);
            Handles.DrawLine(_Target.ParcelPoints.RepeatGet(i + 1), _NextRulerRangePoint);
            Handles.DrawLine(_ThisRulerValuePoint, _RulerValueLabelPoint + _RulerValueLabelOffset);
            Handles.DrawLine(_NextRulerValuePoint, _RulerValueLabelPoint - _RulerValueLabelOffset);
            Handles.Label(_RulerValueLabelPoint, $"[{i}] {_Target.ParcelEdgeLengths[i]:0.00}m", GeneralGUIStyle.Normal);

            if (!_Target.NonCompliance)
            {
                _SceneView.RemoveNotification();
                _ThisRulerRangePoint = _Target.BlockPoints[0][i] - _RulerRangeVector;
                _NextRulerRangePoint = _Target.BlockPoints[0].RepeatGet(i + 1) - _RulerRangeVector;
                _ThisRulerValuePoint = _Target.BlockPoints[0][i] - _RulerValueVector;
                _NextRulerValuePoint = _Target.BlockPoints[0].RepeatGet(i + 1) - _RulerValueVector;
                _RulerValueLabelPoint = (_ThisRulerValuePoint + _NextRulerValuePoint) / 2;
                Handles.DrawLine(_Target.BlockPoints[0][i], _ThisRulerRangePoint);
                Handles.DrawLine(_Target.BlockPoints[0].RepeatGet(i + 1), _NextRulerRangePoint);
                Handles.DrawLine(_ThisRulerValuePoint, _RulerValueLabelPoint + _RulerValueLabelOffset);
                Handles.DrawLine(_NextRulerValuePoint, _RulerValueLabelPoint - _RulerValueLabelOffset);
                Handles.Label(_RulerValueLabelPoint, $"{_Target.BlockEdgeLengths[0][i]:0.00}m", GeneralGUIStyle.Normal);
            }
            else
            {
                _SceneView.ShowNotification(GeneralGUIStyle.Notification);
            }
        }
    }

    public static class GeneralGUIStyle
    {
        private static GUIStyle _Normal;
        public static GUIStyle Normal
        {
            get
            {
                if(_Normal == null)
                {
                    _Normal = new GUIStyle
                    {
                        alignment = TextAnchor.MiddleCenter,
                        fixedWidth = 200,
                        fixedHeight = 100
                    };

                    _Normal.normal.textColor = Color.white;
                }

                return _Normal;
            }
        }


        private static GUIContent _Notification;
        public static GUIContent Notification
        {
            get
            {
                if (_Notification == null)
                {
                    _Notification = new GUIContent
                    {
                        text = "地块尺寸不符合规范"
                    };
                }

                return _Notification;
            }
        }
    }
}
