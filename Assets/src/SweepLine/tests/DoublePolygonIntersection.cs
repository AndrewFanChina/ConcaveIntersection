using ConcaveHull;
using SweepLine;
using UnityEngine;

public class DoublePolygonIntersection : MonoBehaviour
{
    public PolygonGen m_polygonTarget;
    protected PolygonGen m_polygonGen;
    protected Vector2 m_currentPos;
    public bool m_usePresetPoints;
    public bool m_repeat;
    public TextMesh m_showText;
    void Start()
    {
        m_polygonGen = GetComponent<PolygonGen>();
    }

    private void Update()
    {
        var _newPos = transform.position;
        var _pos2D = new Vector2(_newPos.x, _newPos.y);
        bool _needUpdate = false;
        if (m_polygonGen != null && m_polygonGen.m_polygon != null && m_polygonTarget != null && m_polygonTarget.m_polygon != null)
        {
            if (Time.frameCount >= 1 && m_usePresetPoints)
            {
                m_usePresetPoints = false;
                swapPolygon(m_polygonGen.m_polygon);
                swapPolygon(m_polygonTarget.m_polygon);
                _needUpdate = true;
            }
        }
        if (m_currentPos != _pos2D)
        {
            m_currentPos = _pos2D;
            _needUpdate = true;
        }
        if(m_repeat)
        {
            m_repeat=false;
            _needUpdate=true;
        }
        if (_needUpdate)
        {
            if (m_polygonGen != null && m_polygonGen.m_polygon != null && m_polygonTarget != null && m_polygonTarget.m_polygon != null)
            {
                float _t1 = Time.realtimeSinceStartup;
                var _cross = m_polygonGen.m_polygon.CrossWith(m_polygonTarget.m_polygon);
                if(m_showText!=null)
                {
                    m_showText.text=_cross?"cross":"not cross";
                }
                float _t2 = Time.realtimeSinceStartup;
                Debug.Log("used time:" + (_t2 - _t1));
            }
        }

    }
    protected void swapPolygon(SimplePolygon _polygon)
    {
        _polygon.Clear();
        _polygon.AddPoint(new Vector2(152, 115));
        _polygon.AddPoint(new Vector2(152, 103));
        // _polygon.AddPoint(new Vector2(164, 75));
        // _polygon.AddPoint(new Vector2(136, 60));
        // _polygon.AddPoint(new Vector2(136, 48));
        // _polygon.AddPoint(new Vector2(108, 25));
        _polygon.AddPoint(new Vector2(295, 26));//G
        // _polygon.AddPoint(new Vector2(282, 55));
        // _polygon.AddPoint(new Vector2(260, 60));
        _polygon.AddPoint(new Vector2(242, 70));
        _polygon.AddPoint(new Vector2(264, 88));
        _polygon.AddPoint(new Vector2(264, 114));
        _polygon.AddPoint(new Vector2(197, 127));
        _polygon.MakeCounterClockwise().LinkNeighbors();
    }
}
