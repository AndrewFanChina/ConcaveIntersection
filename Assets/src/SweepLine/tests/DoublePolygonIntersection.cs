using ConcaveHull;
using UnityEngine;

public class DoublePolygonIntersection : MonoBehaviour
{
	public PolygonGen m_polygonTarget;
	public bool m_cross;
	protected PolygonGen m_polygonGen;
	protected Vector2 m_currentPos;
	void Start()
    {
        m_polygonGen=GetComponent<PolygonGen>();
    }

	private void Update()
	{
		var _newPos = transform.position;
		var _pos2D = new Vector2(_newPos.x, _newPos.y);
		if(m_currentPos != _pos2D)
		{
			m_currentPos = _pos2D;
			if(m_polygonGen != null && m_polygonGen.m_polygon != null&&m_polygonTarget != null && m_polygonTarget.m_polygon != null)
			{
				float _t1 = Time.realtimeSinceStartup;
				m_cross = m_polygonGen.m_polygon.CrossWith(m_polygonTarget.m_polygon);
				float _t2 = Time.realtimeSinceStartup;
				//Debug.Log("used time:"+(_t2-_t1));
			}
		}


	}
}
