using ConcaveHull;
using SweepLine;
using UnityEngine;

public class SegmentsCross : MonoBehaviour
{
	public TextMesh m_showText;
	void Start()
    {

    }
	private Vector2 GetChildPos(int _id)
	{
		var _child = transform.GetChild(_id);
		var _posI=_child.transform.position;
		return new Vector2(_posI.x,_posI.y);
	}
	private void Update()
	{
		var _childCount=transform.childCount;
		if(_childCount>=4)
		{
			var _p0=GetChildPos(0);
			var _p1=GetChildPos(1);
			var _p2=GetChildPos(2);
			var _p3=GetChildPos(3);
			var _crossed = SimplePolygon.SegmenetCross(_p0,_p1,_p2,_p3);
			if(m_showText!=null)
			{
				m_showText.text=_crossed?"Cross":"Not Cross";
			}
		}

	}
	void OnDrawGizmos()
	{


		Gizmos.color = Color.yellow;
		var _childCount=transform.childCount;
		if(_childCount>=4)
		{
			var _p0=GetChildPos(0);
			var _p1=GetChildPos(1);
			var _p2=GetChildPos(2);
			var _p3=GetChildPos(3);
			Gizmos.color = Color.white;
			Gizmos.DrawLine(_p0, _p1);
			Gizmos.DrawLine(_p2, _p3);
			Gizmos.DrawSphere(new Vector3(_p0.x, _p0.y, 0), 3.5f);
			Gizmos.DrawSphere(new Vector3(_p1.x, _p1.y, 0), 3.5f);
			Gizmos.DrawSphere(new Vector3(_p2.x, _p2.y, 0), 3.5f);
			Gizmos.DrawSphere(new Vector3(_p3.x, _p3.y, 0), 3.5f);
		}

	}
}
