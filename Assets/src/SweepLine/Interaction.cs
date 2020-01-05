using ConcaveHull;
using UnityEngine;

public class Interaction : MonoBehaviour
{
	public bool m_contains;
	public Vector2 m_targetPoint;
	public PolygonGen m_polygonGen;
	void Start()
    {
        
    }

	private void Update()
	{
		var _newPos = transform.position;
		var _pos2D = new Vector2(_newPos.x, _newPos.y);
		if(m_targetPoint != _pos2D)
		{
			m_targetPoint = _pos2D;
			if(m_polygonGen != null && m_polygonGen.m_polygon != null)
			{
				float _t1 = Time.realtimeSinceStartup;
				m_contains = m_polygonGen.m_polygon.ContainsPoint(m_targetPoint);
				float _t2 = Time.realtimeSinceStartup;
				//Debug.Log("used time:"+(_t2-_t1));
			}
		}


	}
}
