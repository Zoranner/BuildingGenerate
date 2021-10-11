using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
public class BuildingBlock2 : MonoBehaviour
{
    //private readonly float _TempMaxEdgeLength;
    //private readonly int _TempEdgeLengthSort;
    //private readonly int _TempAngleValueSort;
    //private readonly float[] _ContractDistances;
    //private readonly int _SelfChildCount;
    //private readonly Transform _ChildTransform;
    //private readonly GameObject _ChildGameObject;
    //private readonly MeshFilter _ChildMeshFilter;
    //private readonly MeshRenderer _ChildMeshRenderer;
    //private readonly CombineInstance[] _CombineInstances;
    //private readonly Material[] _ChildMaterials;
    //private Matrix4x4 _SelfMatrix4x4;
    //private readonly Mesh _CombinedBlock;
    ////private Vector3[] _CurrentBlockPoints;
    //private readonly int _AngleValueIndexDifference;
    //private Vector3 _MaxAngleValuePoint;
    //private Vector3 _TempFootPoint;
    //private Vector3 _TempVerticalLine;
    //private Vector3 _TempIntersectPoint;

    private const float PARCEL_MARGIN = 22.5f;
    private const float TOWER_MARGIN = 5f;

    public Material ParcelMaterial;

    public Material[] BlockMaterials;

    public float Far = 3;

    public bool NonCompliance { get; private set; }

    private Polygon _Parcel;
    public Polygon Parcel
    {
        get
        {
            if (_Parcel == null)
            {
                _Parcel = new Polygon();
            }

            return _Parcel;
        }
    }

    private Polygon[] _Blocks;
    public Polygon[] Blocks
    {
        get
        {
            if (_Blocks == null || _Blocks.Length != 4)
            {
                _Blocks = new Polygon[4];
            }

            return _Blocks;
        }
    }

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

    private void OnEnable()
    {
        transform.hideFlags = HideFlags.NotEditable;
    }

    private void Update()
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;

        for (var i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i < Far * 8);
        }

        if (Time.frameCount % 10 == 0)
        {
            GenerateBlock();
        }
    }

    public void GenerateBlock()
    {
        NonCompliance = false;

        // 显示地面多边形网格
        BuildObject(Parcel, "Parcel", -1, ParcelMaterial);

        for (var i = 0; i < 4; i++)
        {
            if (Parcel.EdgeLengths[i] > 200 && Parcel.EdgeLengths.RepeatGet(i + 1) > 200 || Parcel.AngleValues[i] < 60)
            {
                // 不满足生成规则直接返回
                MeshRenderer.enabled = false;
                NonCompliance = true;
                return;
            }
        }

        // 生成第一阶层建筑
        Blocks[0] = new Polygon(Parcel.Points);
        // 计算符合规则的最终收缩距离
        var margins = new float[4];
        var minEdgeLengthIndex = Blocks[0].EdgeLengthsSort[0];
        margins[minEdgeLengthIndex] = PARCEL_MARGIN;
        // 较短的一边贴边处理
        var shorterEdgeLengthIndex = Blocks[0].EdgeLengths.RepeatGet(minEdgeLengthIndex + 1) <= Blocks[0].EdgeLengths.RepeatGet(minEdgeLengthIndex - 1) ? minEdgeLengthIndex + 1 : minEdgeLengthIndex - 1;
        margins.RepeatSet(shorterEdgeLengthIndex, PARCEL_MARGIN);
        var longerEdgeRestDistance = Mathf.Max(0, Blocks[0].EdgeLengths[minEdgeLengthIndex] - 105) * Blocks[0].AngleSinValues.RepeatGet(shorterEdgeLengthIndex + 2);
        margins.RepeatSet(shorterEdgeLengthIndex + 2, longerEdgeRestDistance + PARCEL_MARGIN);
        var faceEdgeRestDistance = Mathf.Max(0, Blocks[0].EdgeLengths.RepeatGet(shorterEdgeLengthIndex) - 105) * Blocks[0].AngleSinValues.RepeatGet(minEdgeLengthIndex + 2);
        margins.RepeatSet(minEdgeLengthIndex + 2, faceEdgeRestDistance + PARCEL_MARGIN);
        Blocks[0].Shrink(margins);

        for (var i = 0; i < 5; i++)
        {
            BuildObject(Blocks[0], $"Blocks-{0}-{i:D2}", i, BlockMaterials[0]);
        }

        // 生成第二阶层建筑
        Blocks[1] = new Polygon(Blocks[0].Points);
        Blocks[1].Shrink(TOWER_MARGIN);

        var maxAngleValueIndex = Blocks[1].AngleValuesSort[0];
        var secondAngleValueIndex = Blocks[1].AngleValuesSort[1];
        var thirdAngleValueIndex = Blocks[1].AngleValuesSort[2];
        var minAngleValueIndex = Blocks[1].AngleValuesSort[3];
        var secondAngleValue = Blocks[1].AngleValues[secondAngleValueIndex];
        var thirdAngleValue = Blocks[1].AngleValues[thirdAngleValueIndex];

        _TempBlockPoints = Blocks[1].Points;

        // 排序第一的角一定为钝角
        // 排序第二的角为钝角
        if (secondAngleValue > 90)
        {
            // 排序第三的角为直角（已完成）
            if (thirdAngleValue == 90)
            {
                // 直角前后两角的角度
                var nextBlockPoint = Blocks[1].Points.RepeatGet(thirdAngleValueIndex + 1, out var nextAngleValueIndex);
                var lastBlockPoint = Blocks[1].Points.RepeatGet(thirdAngleValueIndex - 1, out var lastAngleValueIndex);

                // 作垂线求交点
                _TempVerticalLine = Vector3.Cross(Blocks[1].NextVector3Ns[thirdAngleValueIndex], Vector3.up).normalized;
                _TempIntersectPoint = TwoLinesIntersectPoint(nextBlockPoint, nextBlockPoint + _TempVerticalLine,
                                                             lastBlockPoint, Blocks[1].Points[minAngleValueIndex]);
                Blocks[1].Points[minAngleValueIndex] = _TempIntersectPoint;

                // 再次作垂线求交点
                _TempVerticalLine = Vector3.Cross(Blocks[1].LastVector3Ns[thirdAngleValueIndex], Vector3.up).normalized;
                _TempIntersectPoint = TwoLinesIntersectPoint(lastBlockPoint, lastBlockPoint + _TempVerticalLine,
                                                             nextBlockPoint, Blocks[1].Points[minAngleValueIndex]);
                Blocks[1].Points[minAngleValueIndex] = _TempIntersectPoint;
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
            // 排序第三的角为直角（已完成）
            if (thirdAngleValue == 90)
            {
                // 最大角前后两角的角度
                var nextAngleValue = Blocks[1].AngleValues.RepeatGet(maxAngleValueIndex + 1, out var nextAngleValueIndex);
                var lastAngleValue = Blocks[1].AngleValues.RepeatGet(maxAngleValueIndex - 1, out var lastAngleValueIndex);

                _MaxAngleValuePoint = Blocks[1].Points[maxAngleValueIndex];

                // 右直角梯形（已完成）
                if (nextAngleValue > lastAngleValue)
                {
                    // 作垂线求交点
                    Debug.Log(maxAngleValueIndex);
                    _TempVerticalLine = Vector3.Cross(Blocks[1].NextVector3Ns[maxAngleValueIndex], Vector3.up).normalized;
                    Debug.Log(_TempVerticalLine);
                    Debug.Log($"{_MaxAngleValuePoint} {_MaxAngleValuePoint + _TempVerticalLine}");
                    Debug.Log($"{Blocks[1].Points.RepeatGet(maxAngleValueIndex + 2)} {Blocks[1].Points.RepeatGet(maxAngleValueIndex + 3)}");
                    _TempIntersectPoint = TwoLinesIntersectPoint(_MaxAngleValuePoint, _MaxAngleValuePoint + _TempVerticalLine,
                                                                 Blocks[1].Points.RepeatGet(maxAngleValueIndex + 2), Blocks[1].Points.RepeatGet(maxAngleValueIndex + 3));
                    Debug.Log(_TempIntersectPoint);
                    Debug.Log(nextAngleValueIndex);
                    _TempBlockPoints[lastAngleValueIndex] = _TempIntersectPoint - transform.position;
                }
                // 左直角梯形（已完成）
                else if (nextAngleValue < lastAngleValue)
                {
                    // 作垂线求交点
                    Debug.Log("这种情况");
                    Debug.Log(maxAngleValueIndex);
                    _TempVerticalLine = Vector3.Cross(Blocks[1].LastVector3Ns[maxAngleValueIndex], Vector3.up).normalized;
                    Debug.Log(_TempVerticalLine);
                    Debug.Log($"{_MaxAngleValuePoint} {_MaxAngleValuePoint + _TempVerticalLine}");
                    Debug.Log($"{Blocks[1].Points.RepeatGet(maxAngleValueIndex + 1)} {Blocks[1].Points.RepeatGet(maxAngleValueIndex + 2)}");
                    _TempIntersectPoint = TwoLinesIntersectPoint(_MaxAngleValuePoint + _TempVerticalLine, _MaxAngleValuePoint,
                                                                 Blocks[1].Points.RepeatGet(maxAngleValueIndex + 1), Blocks[1].Points.RepeatGet(maxAngleValueIndex + 2));
                    Debug.Log(_TempIntersectPoint);
                    Debug.Log(nextAngleValueIndex);
                    _TempBlockPoints[nextAngleValueIndex] = _TempIntersectPoint - transform.position;
                }
                // 特殊情况（已完成）
                else if (nextAngleValue == lastAngleValue)
                {
                    var faceAngleValueIndex = Blocks[1].AngleValues.RepeatIndex(maxAngleValueIndex + 2);

                    // 作垂线求交点
                    _TempVerticalLine = Vector3.Cross(Blocks[1].NextVector3Ns[maxAngleValueIndex], Vector3.up).normalized;
                    _TempIntersectPoint = TwoLinesIntersectPoint(_MaxAngleValuePoint, _MaxAngleValuePoint + _TempVerticalLine,
                                                                 Blocks[1].Points.RepeatGet(maxAngleValueIndex + 2), Blocks[1].Points.RepeatGet(maxAngleValueIndex + 3));
                    _TempBlockPoints[lastAngleValueIndex] = _TempIntersectPoint - transform.position;

                    // 再次作垂线求交点
                    _TempVerticalLine = Vector3.Cross(_TempVerticalLine, Vector3.up).normalized;
                    _TempIntersectPoint = TwoLinesIntersectPoint(_TempIntersectPoint, _TempIntersectPoint + _TempVerticalLine,
                                                                 Blocks[1].Points.RepeatGet(maxAngleValueIndex + 1), Blocks[1].Points.RepeatGet(maxAngleValueIndex + 2));
                    _TempBlockPoints[faceAngleValueIndex] = _TempIntersectPoint - transform.position;
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

        //Debug.Log(_TempBlockPoints);
        Blocks[1].Refresh(_TempBlockPoints);

        // 生成第二阶层建筑
        for (var i = 5; i < 46; i++)
        {
            BuildObject(Blocks[1], $"Blocks-{1}-{i:D2}", i, BlockMaterials[1]);
        }

        // 生成第三阶层建筑 
        Blocks[2] = new Polygon(Blocks[1].Points);
        Blocks[2].Shrink(TOWER_MARGIN);

        // 生成第三阶层建筑
        for (var i = 46; i < 61; i++)
        {
            BuildObject(Blocks[2], $"Blocks-{2}-{i:D2}", i, BlockMaterials[2]);
        }

        // 生成第四阶层建筑
        Blocks[3] = new Polygon(Blocks[2].Points);
        Blocks[3].Shrink(TOWER_MARGIN);

        // 生成第四阶层建筑
        for (var i = 61; i < 76; i++)
        {
            BuildObject(Blocks[3], $"Blocks-{3}-{i:D2}", i, BlockMaterials[3]);
        }
    }

    private Transform _ChildTransform;
    private GameObject _ChildGameObject;
    private MeshFilter _ChildMeshFilter;
    private MeshRenderer _ChildMeshRenderer;
    private int _AngleValueIndexDifference;
    private Vector3 _TempVerticalLine;
    private Vector3 _TempIntersectPoint;
    private Vector3 _MaxAngleValuePoint;
    private Vector3[] _TempBlockPoints;

    private void BuildObject(Polygon polygon, string name, int level, Material material)
    {
        _ChildTransform = transform.Find(name);
        if (!_ChildTransform)
        {
            _ChildGameObject = new GameObject(name);
            _ChildTransform = _ChildGameObject.transform;
            _ChildTransform.SetParent(transform);
            //_ChildGameObject.hideFlags = HideFlags.HideAndDontSave;
        }
        _ChildTransform.localPosition = new Vector3(0, level * 4, 0);
        _ChildTransform.localRotation = Quaternion.identity;
        _ChildTransform.localScale = Vector3.one;
        _ChildMeshFilter = _ChildTransform.GetOrAddComponent<MeshFilter>();
        _ChildMeshRenderer = _ChildTransform.GetOrAddComponent<MeshRenderer>();
        _ChildMeshFilter.mesh = polygon.PrismMesh;
        _ChildMeshRenderer.material = material;
    }

    private void OnDrawGizmos()
    {
        var handleSize = HandleUtility.GetHandleSize(transform.position);
        Handles.zTest = CompareFunction.LessEqual;
        Handles.color = Color.gray;
        //Handles.DrawAAConvexPolygon(Parcel.Points);
        for (var i = 0; i < 4; i++)
        {
            Handles.CubeHandleCap(i, Parcel.Points[i], Quaternion.identity, handleSize * 0.2F, EventType.Repaint);
        }
    }

    private void OnDestroy()
    {
        MeshRenderer.enabled = false;
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

    // 判断一个点是否在凸多边形内
    private static bool IsPointInPolygon(Vector3 point, Vector3[] polyPoints)
    {
        var crossProducts = Vector3.Cross(polyPoints[0], polyPoints[1]).magnitude;

        for (var i = 1; i < polyPoints.Length; i++)
        {
            crossProducts *= Vector3.Cross(polyPoints[i], polyPoints.RepeatGet(i + 1)).magnitude;

            if (crossProducts < 0)
            {
                return false;
            }
        }

        return true;
    }
}
