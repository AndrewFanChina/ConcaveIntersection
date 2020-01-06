using ConcaveHull;
using SweepLine;
using UnityEngine;

public class DoublePolygonIntersection : MonoBehaviour
{
    public PolygonGen m_polygonTarget;
    public bool m_cross;
    protected PolygonGen m_polygonGen;
    protected Vector2 m_currentPos;
    public bool m_usePresetPoints;
    public bool m_repeat;
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
                m_cross = m_polygonGen.m_polygon.CrossWith(m_polygonTarget.m_polygon);
                float _t2 = Time.realtimeSinceStartup;
                Debug.Log("used time:" + (_t2 - _t1));
            }
        }

    }
    protected void swapPolygon(SimplePolygon _polygon)
    {
        _polygon.Clear();
        _polygon.AddPoint(new Vector2(152, 115));
        _polygon.AddPoint(new Vector2(72, 103));
        _polygon.AddPoint(new Vector2(164, 75));
        _polygon.AddPoint(new Vector2(136, 60));
        _polygon.AddPoint(new Vector2(136, 48));
        _polygon.AddPoint(new Vector2(108, 25));
        _polygon.AddPoint(new Vector2(282, 32));
        _polygon.AddPoint(new Vector2(222, 100));
        _polygon.MakeCounterClockwise().LinkNeighbors();
    }
}
