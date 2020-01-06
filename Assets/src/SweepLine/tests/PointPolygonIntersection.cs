using ConcaveHull;
using UnityEngine;

public class PointPolygonIntersection : MonoBehaviour
{
	public Vector2 m_targetPoint;
	public PolygonGen m_polygonGen;
	public TextMesh m_showText;
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
				var _contains = m_polygonGen.m_polygon.ContainsPoint(m_targetPoint);
				if(m_showText!=null)
                {
                    m_showText.text=_contains?"contains":"not contains";
                }
				float _t2 = Time.realtimeSinceStartup;
				//Debug.Log("used time:"+(_t2-_t1));
			}
		}


	}
}
