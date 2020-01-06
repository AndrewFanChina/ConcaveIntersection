using ConcaveHull;
using SweepLine;
using UnityEngine;

public class PolygonLoopDir : MonoBehaviour
{
    private SimplePolygon m_polygon;
	public TextMesh m_showText;
	void Start()
    {
        m_polygon=new SimplePolygon();
    }

	private void Update()
	{
		m_polygon.Clear();
		var _newPos = transform.position;
		var _childCount=transform.childCount;
		for (int i = 0; i < _childCount; i++)
		{
			var _child = transform.GetChild(i);
			var _posI=_child.transform.position;
			m_polygon.AddPoint(new Vector2(_posI.x,_posI.y));
		}
		var _clockwise = m_polygon.IsClockwise();
		if(m_showText!=null)
		{
			m_showText.text=_clockwise?"Clockwise":"CouterClockwise";
		}


	}
	void OnDrawGizmos()
	{

		if (m_polygon == null || m_polygon.Points == null)
		{
			return;
		}
		Gizmos.color = Color.yellow;
		var _points = m_polygon.Points;
		int _count = _points.Count;
		float _timeP=Time.realtimeSinceStartup;
		_timeP-=(int)_timeP;
		for (int i = 0; i < _count; i++)
		{
			Gizmos.color = Color.white;
			Vector2 left = _points[i].getValue();
			Vector2 right = _points[(i + 1) % _count].getValue();
			Gizmos.DrawLine(left, right);
			Gizmos.DrawSphere(new Vector3(left.x, left.y, 0), 3.5f);
			Gizmos.color = Color.green;
			var _lerp=left+ (right-left)*_timeP;
			Gizmos.DrawSphere(new Vector3(_lerp.x, _lerp.y, 0), 1.5f);
		}
	}
}
