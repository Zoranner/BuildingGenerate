﻿using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class BuildingBlock : MonoBehaviour
{
    private float _TempMaxEdgeLength;
    private int _TempEdgeLengthSort;
    private int _TempAngleValueSort;
    private float[] _ContractDistances;
    private int _SelfChildCount;
    private Transform _ChildTransform;
    private GameObject _ChildGameObject;
    private MeshFilter _ChildMeshFilter;
    private MeshRenderer _ChildMeshRenderer;
    private CombineInstance[] _CombineInstances;
    private Material[] _ChildMaterials;
    private Matrix4x4 _SelfMatrix4x4;
    private Mesh _CombinedBlock;
    //private Vector3[] _CurrentBlockPoints;
    private int _AngleValueIndexDifference;
    private Vector3 _MaxAngleValuePoint;
    private Vector3 _TempFootPoint;
    private Vector3 _TempVerticalLine;
    private Vector3 _TempIntersectPoint;

    private const float PARCEL_MARGIN = 22.5f;
    private const float TOWER_MARGIN = 5f;

    public Material[] DefaultMaterials;

    [SerializeField]
    [HideInInspector]
    public bool NonCompliance { get; private set; }

    private MeshFilter _MeshFilter;
    private MeshFilter MeshFilter
    {
        get
        {
            if (_MeshFilter == null)
            {
                _MeshFilter = gameObject.GetOrAddComponent<MeshFilter>();
            }

            return _MeshFilter;
        }
    }

    private MeshRenderer _MeshRenderer;
    private MeshRenderer MeshRenderer
    {
        get
        {
            if (_MeshRenderer == null)
            {
                _MeshRenderer = gameObject.GetOrAddComponent<MeshRenderer>();
            }

            return _MeshRenderer;
        }
    }

    [SerializeField]
    [HideInInspector]
    private Vector3[] _ParcelPointsLocal;
    public Vector3[] ParcelPointsLocal
    {
        get
        {
            if (_ParcelPointsLocal == null || _ParcelPointsLocal.Length != 4)
            {
                _ParcelPointsLocal = new Vector3[] {
                    new Vector3(-50, 0, -50),
                    new Vector3(-50, 0, 50),
                    new Vector3(50, 0, 50),
                    new Vector3(50, 0, -50)
                };
            }
            return _ParcelPointsLocal;
        }
        private set => _ParcelPointsLocal = value;
    }

    [SerializeField]
    [HideInInspector]
    private Vector3[] _ParcelPoints;
    public Vector3[] ParcelPoints
    {
        get
        {
            if (_ParcelPoints == null || _ParcelPoints.Length != 4)
            {
                _ParcelPoints = new Vector3[4];
            }
            return _ParcelPoints;
        }
        private set => _ParcelPoints = value;
    }

    [SerializeField]
    [HideInInspector]
    private Vector3[] _LastVector3Ns;
    public Vector3[] LastVector3Ns
    {
        get
        {
            if (_LastVector3Ns == null || _LastVector3Ns.Length != 4)
            {
                _LastVector3Ns = new Vector3[4];
            }
            return _LastVector3Ns;
        }
        private set => _LastVector3Ns = value;
    }

    [SerializeField]
    [HideInInspector]
    private Vector3[] _NextVector3Ns;
    public Vector3[] NextVector3Ns
    {
        get
        {
            if (_NextVector3Ns == null || _NextVector3Ns.Length != 4)
            {
                _NextVector3Ns = new Vector3[4];
            }
            return _NextVector3Ns;
        }
        private set => _ParcelPoints = value;
    }

    [SerializeField]
    [HideInInspector]
    private Vector3[] _VerticalVector3Ns;
    public Vector3[] VerticalVector3Ns
    {
        get
        {
            if (_VerticalVector3Ns == null || _VerticalVector3Ns.Length != 4)
            {
                _VerticalVector3Ns = new Vector3[4];
            }
            return _VerticalVector3Ns;
        }
        private set => _VerticalVector3Ns = value;
    }

    [SerializeField]
    [HideInInspector]
    private float[] _ParcelEdgeLengths;
    public float[] ParcelEdgeLengths
    {
        get
        {
            if (_ParcelEdgeLengths == null || _ParcelEdgeLengths.Length != 4)
            {
                _ParcelEdgeLengths = new float[4];
            }
            return _ParcelEdgeLengths;
        }
        private set => _ParcelEdgeLengths = value;
    }

    [SerializeField]
    //[HideInInspector]
    private float[] _AngleValues;
    public float[] AngleValues
    {
        get
        {
            if (_AngleValues == null || _AngleValues.Length != 4)
            {
                _AngleValues = new float[4];
            }
            return _AngleValues;
        }
        private set => _AngleValues = value;
    }

    [SerializeField]
    [HideInInspector]
    private int[] _AngleValuesSort;
    public int[] AngleValuesSort
    {
        get
        {
            if (_AngleValuesSort == null || _AngleValuesSort.Length != 4)
            {
                _AngleValuesSort = new int[] { 0, 1, 2, 3 };
            }
            return _AngleValuesSort;
        }
        private set => _AngleValuesSort = value;
    }

    [SerializeField]
    [HideInInspector]
    private float[] _AngleSinValues;
    public float[] AngleSinValues
    {
        get
        {
            if (_AngleSinValues == null || _AngleSinValues.Length != 4)
            {
                _AngleSinValues = new float[4];
            }
            return _AngleSinValues;
        }
        private set => _AngleSinValues = value;
    }

    [SerializeField]
    [HideInInspector]
    private Vector3[][] _BlockPointsLocal;
    public Vector3[][] BlockPointsLocal
    {
        get
        {
            if (_BlockPointsLocal == null || _BlockPointsLocal.Length != 4)
            {
                _BlockPointsLocal = new Vector3[4][];

                for (var i = 0; i < _BlockPointsLocal.Length; i++)
                {
                    _BlockPointsLocal[i] = new Vector3[4];
                }
            }
            return _BlockPointsLocal;
        }
        private set => _BlockPointsLocal = value;
    }

    [SerializeField]
    [HideInInspector]
    private Vector3[][] _BlockPoints;
    public Vector3[][] BlockPoints
    {
        get
        {
            if (_BlockPoints == null || _BlockPoints.Length != 4)
            {
                _BlockPoints = new Vector3[4][];

                for (var i = 0; i < _BlockPoints.Length; i++)
                {
                    _BlockPoints[i] = new Vector3[4];
                }
            }
            return _BlockPoints;
        }
        private set => _BlockPoints = value;
    }

    [SerializeField]
    [HideInInspector]
    private float[][] _BlockEdgeLengths;
    public float[][] BlockEdgeLengths
    {
        get
        {
            if (_BlockEdgeLengths == null || _BlockEdgeLengths.Length != 4)
            {
                _BlockEdgeLengths = new float[4][];

                for (var i = 0; i < _BlockEdgeLengths.Length; i++)
                {
                    _BlockEdgeLengths[i] = new float[4];
                }
            }
            return _BlockEdgeLengths;
        }
        private set => _BlockEdgeLengths = value;
    }

    [SerializeField]
    [HideInInspector]
    private float[][] _BlockMaxEdgeLengths;
    public float[][] BlockMaxEdgeLengths
    {
        get
        {
            if (_BlockMaxEdgeLengths == null || _BlockMaxEdgeLengths.Length != 4)
            {
                _BlockMaxEdgeLengths = new float[4][];

                for (var i = 0; i < _BlockMaxEdgeLengths.Length; i++)
                {
                    _BlockMaxEdgeLengths[i] = new float[4];
                }
            }
            return _BlockMaxEdgeLengths;
        }
        private set => _BlockMaxEdgeLengths = value;
    }

    [SerializeField]
    [HideInInspector]
    private int[][] _BlockMaxEdgeLengthsSort;
    public int[][] BlockMaxEdgeLengthsSort
    {
        get
        {
            if (_BlockMaxEdgeLengthsSort == null || _BlockMaxEdgeLengthsSort.Length != 4)
            {
                _BlockMaxEdgeLengthsSort = new int[4][];

                for (var i = 0; i < _BlockMaxEdgeLengthsSort.Length; i++)
                {
                    _BlockMaxEdgeLengthsSort[i] = new int[] { 0, 1, 2, 3 };
                }
            }
            return _BlockMaxEdgeLengthsSort;
        }
        private set => _BlockMaxEdgeLengthsSort = value;
    }

    [SerializeField]
    [HideInInspector]
    private Mesh[] _BlockStageMeshes;
    private Mesh[] BlockStageMeshes
    {
        get
        {
            if (_BlockStageMeshes == null || _BlockStageMeshes.Length != 4)
            {
                _BlockStageMeshes = new Mesh[4];
            }
            return _BlockStageMeshes;
        }
        set => _BlockStageMeshes = value;
    }

    private void OnEnable()
    {
        transform.hideFlags = HideFlags.None;
    }

    private void OnDrawGizmos()
    {
        var handleSize = HandleUtility.GetHandleSize(transform.position);
        Handles.zTest = CompareFunction.LessEqual;
        Handles.color = Color.gray;
        Handles.DrawAAConvexPolygon(ParcelPoints);
        for (var i = 0; i < 4; i++)
        {
            Handles.CubeHandleCap(i, ParcelPoints[i], Quaternion.identity, handleSize * 0.2F, EventType.Repaint);
        }
    }

    private void OnDestroy()
    {
        MeshRenderer.enabled = false;
    }

    public void SetParcelPoints(int index, Vector3 value)
    {
        if (ParcelPoints[index] == value)
        {
            return;
        }

        //ParcelPointsLocal[index] = value;

        ParcelPoints[index] = value;
        UpdateParcelPoints();

        EditorUtility.SetDirty(this);
    }

    public void UpdateParcelPoints()
    {
        for (var i = 0; i < 4; i++)
        {
            ParcelPointsLocal[i] = ParcelPoints[i] - transform.position;

            LastVector3Ns[i] = (ParcelPoints[i] - ParcelPoints.RepeatGet(i - 1)).normalized;

            NextVector3Ns[i] = (ParcelPoints[i] - ParcelPoints.RepeatGet(i + 1)).normalized;

            VerticalVector3Ns[i] = Vector3.Cross(NextVector3Ns[i], Vector3.up).normalized;

            ParcelEdgeLengths[i] = Vector3.Distance(ParcelPoints[i], ParcelPoints.RepeatGet(i + 1));

            AngleValues[i] = Vector3.Angle(LastVector3Ns[i], NextVector3Ns[i]);

            AngleSinValues[i] = Vector3.Cross(NextVector3Ns[i], LastVector3Ns[i]).magnitude;
        }

        GenerateBuilding();
    }

    public void GenerateBuilding()
    {
        NonCompliance = false;

        for (var i = 0; i < 4; i++)
        {
            if (ParcelEdgeLengths[i] > 200 && ParcelEdgeLengths.RepeatGet(i + 1) > 200 || AngleValues[i] < 60)
            {
                // 不满足生成规则直接返回
                MeshRenderer.enabled = false;
                NonCompliance = true;
                return;
            }
        }

        // 留出过道统一收缩22.5边距
        for (var i = 0; i < 4; i++)
        {
            BlockPointsLocal[0][i] = ParcelPointsLocal[i] - (NextVector3Ns[i] + LastVector3Ns[i]) * (PARCEL_MARGIN / AngleSinValues[i]);
        }

        // 计算最大建筑区域四条边边长
        for (var i = 0; i < 4; i++)
        {
            _TempMaxEdgeLength = Vector3.Distance(BlockPointsLocal[0][i], BlockPointsLocal[0].RepeatGet(i + 1));
            if (_TempMaxEdgeLength < 55)
            {
                // 不满足生成规则直接返回
                MeshRenderer.enabled = false;
                NonCompliance = true;
                return;
            }
            BlockMaxEdgeLengths[0][i] = _TempMaxEdgeLength;
        }

        // 对最大建筑区域四条边长排序（从小到大）
        BlockMaxEdgeLengthsSort[0] = new int[] { 0, 1, 2, 3 };
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3 - i; j++)
            {
                if (BlockMaxEdgeLengths[0][BlockMaxEdgeLengthsSort[0][j]] > BlockMaxEdgeLengths[0][BlockMaxEdgeLengthsSort[0][j + 1]])
                {
                    _TempEdgeLengthSort = BlockMaxEdgeLengthsSort[0][j];
                    BlockMaxEdgeLengthsSort[0][j] = BlockMaxEdgeLengthsSort[0][j + 1];
                    BlockMaxEdgeLengthsSort[0][j + 1] = _TempEdgeLengthSort;
                }
            }
        }

        // 计算符合规则的最终收缩距离
        _ContractDistances = new float[4];
        var minEdgeLengthIndex = BlockMaxEdgeLengthsSort[0][0];
        _ContractDistances[minEdgeLengthIndex] = 22.5f;
        // 较短的一边贴边处理
        var shorterEdgeLengthIndex = BlockMaxEdgeLengths[0].RepeatGet(minEdgeLengthIndex + 1) <= BlockMaxEdgeLengths[0].RepeatGet(minEdgeLengthIndex - 1) ? minEdgeLengthIndex + 1 : minEdgeLengthIndex - 1;
        _ContractDistances.RepeatSet(shorterEdgeLengthIndex, 22.5f);
        var longerEdgeRestDistance = Mathf.Max(0, BlockMaxEdgeLengths[0][minEdgeLengthIndex] - 105) * AngleSinValues.RepeatGet(shorterEdgeLengthIndex + 2);
        _ContractDistances.RepeatSet(shorterEdgeLengthIndex + 2, longerEdgeRestDistance + 22.5f);
        var faceEdgeRestDistance = Mathf.Max(0, BlockMaxEdgeLengths[0].RepeatGet(shorterEdgeLengthIndex) - 105) * AngleSinValues.RepeatGet(minEdgeLengthIndex + 2);
        _ContractDistances.RepeatSet(minEdgeLengthIndex + 2, faceEdgeRestDistance + 22.5f);

        // 重新计算最终收缩后的顶点
        for (var i = 0; i < 4; i++)
        {
            BlockPointsLocal[0][i] = ParcelPointsLocal[i] - (LastVector3Ns[i] * _ContractDistances[i] + NextVector3Ns[i] * _ContractDistances.RepeatGet(i - 1)) / AngleSinValues[i];
            BlockPoints[0][i] = transform.position + BlockPointsLocal[0][i];
        }

        for (var i = 0; i < 4; i++)
        {
            BlockEdgeLengths[0][i] = Vector3.Distance(BlockPoints[0][i], BlockPoints[0].RepeatGet(i + 1));
        }

        // 生成建筑区域第一阶层的多边形网格
        BlockStageMeshes[0] = CreatePrismMesh(BlockPointsLocal[0], 0.2f, 4f);
        for (var i = 0; i < 5; i++)
        {
            _ChildTransform = transform.Find($"Floor-0-{i:D2}");
            if (!_ChildTransform)
            {
                _ChildGameObject = new GameObject($"Floor-0-{i:D2}");
                _ChildTransform = _ChildGameObject.transform;
                _ChildTransform.SetParent(transform);
                _ChildGameObject.hideFlags = HideFlags.HideAndDontSave;
            }
            _ChildTransform.localPosition = new Vector3(0, i * 4f, 0);
            _ChildTransform.localRotation = Quaternion.identity;
            _ChildTransform.localScale = Vector3.one;
            _ChildMeshFilter = _ChildTransform.GetOrAddComponent<MeshFilter>();
            _ChildMeshRenderer = _ChildTransform.GetOrAddComponent<MeshRenderer>();
            _ChildMeshFilter.mesh = BlockStageMeshes[0];
            _ChildMeshRenderer.material = DefaultMaterials[0];
        };

        // 对建筑区域四个角角度进行排序（从大到小）
        AngleValuesSort = new int[] { 0, 1, 2, 3 };
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3 - i; j++)
            {
                if (AngleValues[AngleValuesSort[j]] < AngleValues[AngleValuesSort[j + 1]])
                {
                    _TempAngleValueSort = AngleValuesSort[j];
                    AngleValuesSort[j] = AngleValuesSort[j + 1];
                    AngleValuesSort[j + 1] = _TempAngleValueSort;
                }
            }
        }

        // 统一收缩5米边距留出第二阶层过道
        for (var i = 0; i < 4; i++)
        {
            BlockPointsLocal[1][i] = BlockPointsLocal[0][i] - (NextVector3Ns[i] + LastVector3Ns[i]) * (TOWER_MARGIN / AngleSinValues[i]);
            BlockPoints[1][i] = transform.position + BlockPointsLocal[1][i];
        }

        // 计算第二阶层最大建筑区域四条边边长
        for (var i = 0; i < 4; i++)
        {
            _TempMaxEdgeLength = Vector3.Distance(BlockPointsLocal[1][i], BlockPointsLocal[1].RepeatGet(i + 1));
            if (_TempMaxEdgeLength < 45)
            {
                // 不满足生成规则直接返回
                MeshRenderer.enabled = false;
                NonCompliance = true;
                return;
            }
            BlockMaxEdgeLengths[1][i] = _TempMaxEdgeLength;
        }

        // 对最大建筑区域四条边长排序（从小到大）
        BlockMaxEdgeLengthsSort[1] = new int[] { 0, 1, 2, 3 };
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3 - i; j++)
            {
                if (BlockMaxEdgeLengths[1][BlockMaxEdgeLengthsSort[1][j]] > BlockMaxEdgeLengths[1][BlockMaxEdgeLengthsSort[1][j + 1]])
                {
                    _TempEdgeLengthSort = BlockMaxEdgeLengthsSort[1][j];
                    BlockMaxEdgeLengthsSort[1][j] = BlockMaxEdgeLengthsSort[1][j + 1];
                    BlockMaxEdgeLengthsSort[1][j + 1] = _TempEdgeLengthSort;
                }
            }
        }

        // 计算建筑区域第二阶层的数据
        var maxAngleValueIndex = AngleValuesSort[0];
        var secondAngleValueIndex = AngleValuesSort[1];
        var thirdAngleValueIndex = AngleValuesSort[2];
        var minAngleValueIndex = AngleValuesSort[3];
        var secondAngleValue = AngleValues[secondAngleValueIndex];
        var thirdAngleValue = AngleValues[thirdAngleValueIndex];
        //var maxEdgeLengthIndex = BlockMaxEdgeLengthsSort[1][3];

        //_CurrentBlockPoints = BlockPoints[1];

        // 排序第一的角一定为钝角
        // 排序第二的角为钝角
        if (secondAngleValue > 90)
        {
            // 排序第三的角为直角
            if (thirdAngleValue == 90)
            {
                // 直角前后两角的角度
                var nextBlockPoint = BlockPoints[1].RepeatGet(thirdAngleValueIndex + 1, out var nextAngleValueIndex);
                var lastBlockPoint = BlockPoints[1].RepeatGet(thirdAngleValueIndex - 1, out var lastAngleValueIndex);

                // 作垂线求交点
                _TempVerticalLine = Vector3.Cross(NextVector3Ns[thirdAngleValueIndex], Vector3.up).normalized;
                _TempIntersectPoint = TwoLinesIntersectPoint(nextBlockPoint, nextBlockPoint + _TempVerticalLine,
                                                             lastBlockPoint, BlockPoints[1][minAngleValueIndex]);
                BlockPoints[1][minAngleValueIndex] = _TempIntersectPoint;

                // 再次作垂线求交点
                _TempVerticalLine = Vector3.Cross(LastVector3Ns[thirdAngleValueIndex], Vector3.up).normalized;
                _TempIntersectPoint = TwoLinesIntersectPoint(lastBlockPoint, lastBlockPoint + _TempVerticalLine,
                                                             nextBlockPoint, BlockPoints[1][minAngleValueIndex]);
                BlockPoints[1][minAngleValueIndex] = _TempIntersectPoint;
            }
            // 排序第三的角为锐角
            else if (thirdAngleValue < 90)
            {
                // 两个钝角序号的差值
                _AngleValueIndexDifference = Mathf.Abs(secondAngleValueIndex - thirdAngleValueIndex) % 2;

                // 类平行四边形
                if (_AngleValueIndexDifference == 0)
                {

                }
                // 类常规梯形
                else if (_AngleValueIndexDifference == 1)
                {

                }
            }

        }
        // 排序第二的角为直角
        else if (secondAngleValue == 90)
        {
            // 排序第三的角为直角
            if (thirdAngleValue == 90)
            {
                // 最大角前后两角的角度
                var nextAngleValue = AngleValues.RepeatGet(maxAngleValueIndex + 1, out var nextAngleValueIndex);
                var lastAngleValue = AngleValues.RepeatGet(maxAngleValueIndex - 1, out var lastAngleValueIndex);

                _MaxAngleValuePoint = BlockPoints[1][maxAngleValueIndex];

                // 右直角梯形
                if (nextAngleValue > lastAngleValue)
                {
                    // 作垂线求交点
                    _TempVerticalLine = Vector3.Cross(NextVector3Ns[maxAngleValueIndex], Vector3.up).normalized;
                    _TempIntersectPoint = TwoLinesIntersectPoint(_MaxAngleValuePoint, _MaxAngleValuePoint + _TempVerticalLine,
                                                                 BlockPoints[1].RepeatGet(maxAngleValueIndex + 2), BlockPoints[1].RepeatGet(maxAngleValueIndex + 3));
                    BlockPointsLocal[1][lastAngleValueIndex] = _TempIntersectPoint - transform.position;
                }
                // 左直角梯形
                else if (nextAngleValue < lastAngleValue)
                {
                    // 作垂线求交点
                    _TempVerticalLine = Vector3.Cross(LastVector3Ns[maxAngleValueIndex], Vector3.up).normalized;
                    _TempIntersectPoint = TwoLinesIntersectPoint(_MaxAngleValuePoint, _MaxAngleValuePoint + _TempVerticalLine,
                                                                 BlockPoints[1].RepeatGet(maxAngleValueIndex + 1), BlockPoints[1].RepeatGet(maxAngleValueIndex + 2));
                    BlockPointsLocal[1][nextAngleValueIndex] = _TempIntersectPoint - transform.position;
                }
                // 特殊情况
                else if (nextAngleValue == lastAngleValue)
                {
                    var faceAngleValueIndex = AngleValues.RepeatIndex(maxAngleValueIndex + 2);

                    // 作垂线求交点
                    _TempVerticalLine = Vector3.Cross(NextVector3Ns[maxAngleValueIndex], Vector3.up).normalized;
                    _TempIntersectPoint = TwoLinesIntersectPoint(_MaxAngleValuePoint, _MaxAngleValuePoint + _TempVerticalLine,
                                                                 BlockPoints[1].RepeatGet(maxAngleValueIndex + 2), BlockPoints[1].RepeatGet(maxAngleValueIndex + 3));
                    BlockPointsLocal[1][lastAngleValueIndex] = _TempIntersectPoint - transform.position;

                    // 再次作垂线求交点
                    _TempVerticalLine = Vector3.Cross(_TempVerticalLine, Vector3.up).normalized;
                    _TempIntersectPoint = TwoLinesIntersectPoint(_TempIntersectPoint, _TempIntersectPoint + _TempVerticalLine,
                                                                 BlockPoints[1].RepeatGet(maxAngleValueIndex + 1), BlockPoints[1].RepeatGet(maxAngleValueIndex + 2));
                    BlockPointsLocal[1][faceAngleValueIndex] = _TempIntersectPoint - transform.position;
                }
            }
            // 排序第三的角为锐角
            else if (secondAngleValue < 90)
            {
                // 钝角与直角序号的差值
                _AngleValueIndexDifference = Mathf.Abs(maxAngleValueIndex - secondAngleValueIndex) % 2;

                if (_AngleValueIndexDifference == 0)
                {

                }
                else if (_AngleValueIndexDifference == 1)
                {

                }
            }
        }
        // 排序第二的角为锐角
        else if (secondAngleValue < 90)
        {

        }

        // 重新计算最终收缩后的顶点
        for (var i = 0; i < 4; i++)
        {
            BlockPoints[1][i] = transform.position + BlockPointsLocal[1][i];
        }

        for (var i = 0; i < 4; i++)
        {
            BlockEdgeLengths[1][i] = Vector3.Distance(BlockPoints[1][i], BlockPoints[1].RepeatGet(i + 1));
        }

        // 生成建筑区域第二阶层的多边形网格
        BlockStageMeshes[1] = CreatePrismMesh(BlockPointsLocal[1], 0.2f, 4f);
        for (var i = 5; i < 46; i++)
        {
            _ChildTransform = transform.Find($"Floor-1-{i:D2}");
            if (!_ChildTransform)
            {
                _ChildGameObject = new GameObject($"Floor-1-{i:D2}");
                _ChildTransform = _ChildGameObject.transform;
                _ChildTransform.SetParent(transform);
                _ChildGameObject.hideFlags = HideFlags.HideAndDontSave;
            }
            _ChildTransform.localPosition = new Vector3(0, i * 4f, 0);
            _ChildTransform.localRotation = Quaternion.identity;
            _ChildTransform.localScale = Vector3.one;
            _ChildMeshFilter = _ChildTransform.GetOrAddComponent<MeshFilter>();
            _ChildMeshRenderer = _ChildTransform.GetOrAddComponent<MeshRenderer>();
            _ChildMeshFilter.mesh = BlockStageMeshes[1];
            _ChildMeshRenderer.material = DefaultMaterials[1];
        };

        // 合并所有已经生成的中间网格
        _SelfChildCount = transform.childCount;
        _CombineInstances = new CombineInstance[_SelfChildCount];
        _ChildMaterials = new Material[_SelfChildCount];
        _SelfMatrix4x4 = transform.worldToLocalMatrix;
        for (var i = 0; i < _SelfChildCount; i++)
        {
            _ChildTransform = transform.GetChild(i);
            _ChildMeshFilter = _ChildTransform.GetComponent<MeshFilter>();
            _ChildMeshRenderer = _ChildTransform.GetComponent<MeshRenderer>();
            if (_ChildMeshFilter == null || _ChildMeshRenderer == null)
            {
                continue;
            }
            _CombineInstances[i].mesh = _ChildMeshFilter.sharedMesh;
            _CombineInstances[i].transform = _SelfMatrix4x4 * _ChildMeshFilter.transform.localToWorldMatrix;
            _ChildMeshRenderer.enabled = false;
            //_ChildMeshRenderer.gameObject.hideFlags = HideFlags.None;
            _ChildMaterials[i] = _ChildMeshRenderer.sharedMaterial;
        }
        _CombinedBlock = new Mesh
        {
            name = "Combined Block"
        };
        _CombinedBlock.CombineMeshes(_CombineInstances, false);
        MeshFilter.mesh = _CombinedBlock;
        MeshRenderer.sharedMaterials = _ChildMaterials;
        MeshRenderer.enabled = true;
    }

    // 生成四棱柱多边形网格
    private static Mesh _PrismMesh;
    private static Mesh CreatePrismMesh(Vector3[] points, float floor, float ceil)
    {
        if (points.Length != 4)
        {
            return null;
        }

        _PrismMesh = new Mesh();

        var vertices = new Vector3[24];
        var triangles = new int[36];

        //forward
        vertices[0] = points[1] + Vector3.up * floor;
        vertices[1] = points[0] + Vector3.up * floor;
        vertices[2] = points[1] + Vector3.up * ceil;
        vertices[3] = points[0] + Vector3.up * ceil;
        //back
        vertices[4] = points[2] + Vector3.up * ceil;
        vertices[5] = points[3] + Vector3.up * ceil;
        vertices[6] = points[2] + Vector3.up * floor;
        vertices[7] = points[3] + Vector3.up * floor;
        //up
        vertices[8] = vertices[2];
        vertices[9] = vertices[3];
        vertices[10] = vertices[4];
        vertices[11] = vertices[5];
        //down
        vertices[12] = vertices[6];
        vertices[13] = vertices[7];
        vertices[14] = vertices[0];
        vertices[15] = vertices[1];
        //right
        vertices[16] = vertices[6];
        vertices[17] = vertices[0];
        vertices[18] = vertices[4];
        vertices[19] = vertices[2];
        //left
        vertices[20] = vertices[5];
        vertices[21] = vertices[3];
        vertices[22] = vertices[7];
        vertices[23] = vertices[1];

        var currentCount = 0;
        for (var i = 0; i < 24; i += 4)
        {
            triangles[currentCount++] = i;
            triangles[currentCount++] = i + 3;
            triangles[currentCount++] = i + 1;

            triangles[currentCount++] = i;
            triangles[currentCount++] = i + 2;
            triangles[currentCount++] = i + 3;
        }

        _PrismMesh.vertices = vertices;
        _PrismMesh.triangles = triangles;
        _PrismMesh.RecalculateNormals();

        return _PrismMesh;

    }

    // 求点到直线的垂足
    private static Vector3 Point2LineFootPoint(Vector3 point, Vector3 linePoint1, Vector3 linePoint2)
    {
        var equationA = linePoint2.z - linePoint1.z;
        var equationB = linePoint1.x - linePoint2.x;
        var equationC = linePoint2.x * linePoint1.z - linePoint1.x * linePoint2.z; //x2 * y1 - x1 * y2
        if (linePoint1 == linePoint2)
        {
            // linePoint1与linePoint2重合
            return linePoint1;
        }
        else if (equationA * point.x + equationB * point.z + equationC == 0)
        {
            // point在line上
            return point;
        }
        else
        {
            var x = (equationB * equationB * point.x - equationA * equationB * point.z - equationA * equationC) / (equationA * equationA + equationB * equationB);
            var z = (-equationA * equationB * point.x + equationA * equationA * point.z - equationB * equationC) / (equationA * equationA + equationB * equationB);
            return new Vector3(x, 0, z);
        }
    }

    // 求两个向量的交点
    private static Vector3 TwoLinesIntersectPoint(Vector3 line1Point1, Vector3 line1Point2, Vector3 line2Point1, Vector3 line2Point2)
    {
        // 叉乘为0则平行或共线不相交
        var vector3N1 = line1Point2 - line1Point1;
        var vector3N2 = line2Point2 - line2Point1;
        var crossProduct = Vector3.Cross(vector3N1, vector3N2).magnitude;
        // 线段所在直线的交点坐标 (x, 0, z)
        var x = ((line1Point2.x - line1Point1.x) * (line2Point2.x - line2Point1.x) * (line2Point1.z - line1Point1.z)
                + (line1Point2.z - line1Point1.z) * (line2Point2.x - line2Point1.x) * line1Point1.x
                - (line2Point2.z - line2Point1.z) * (line1Point2.x - line1Point1.x) * line2Point1.x) / crossProduct;
        var z = -((line1Point2.z - line1Point1.z) * (line2Point2.z - line2Point1.z) * (line2Point1.x - line1Point1.x)
                + (line1Point2.x - line1Point1.x) * (line2Point2.z - line2Point1.z) * line1Point1.z
                - (line2Point2.x - line2Point1.x) * (line1Point2.z - line1Point1.z) * line2Point1.z) / crossProduct;
        return new Vector3(x, 0, z);
    }
}
