
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SweepLine
{
	public class Segements
	{
		protected MultiSet<Vector2, Vector2> m_segements=new MultiSet<Vector2, Vector2>();
		protected MultiSet<Vector2, Vector2> m_fullCast = new MultiSet<Vector2, Vector2>();
		protected Dictionary<Vector2, Vector2> m_oneLinkOne = new Dictionary<Vector2, Vector2>();
		public void Clear()
		{
			m_segements.Clear();
		}

		public void AddSegment(Vector2 _left, Vector2 _right)
		{
			m_segements.Add(_left, _right);
		}

		public SimplePolygon ToPolygon(SimplePolygon _newPolygon=null)
		{
			if (_newPolygon == null)
			{
				_newPolygon = new SimplePolygon();
			}
			else
			{
				_newPolygon.Clear();
			}
			m_fullCast.Clear();
			foreach (var _segement in m_segements)
			{
				var _left = _segement.Key;
				var _rights = _segement.Value;
				foreach (var _right in _rights)
				{
					m_fullCast.Add(_left, _right);
					m_fullCast.Add(_right, _left);
				}
			}
			m_oneLinkOne.Clear();

			if (m_fullCast.Count > 0)
			{
				var _left = m_fullCast.First().Key;
				m_oneLinkOne.Add(_left, _left);
				_newPolygon.AddPoint(_left);
				for (int i = 0; i < m_fullCast.Count; i++)
				{
					var _rights = m_fullCast[_left];
					foreach(var _right in _rights)
					{
						if(!m_oneLinkOne.ContainsKey(_right))
						{
							m_oneLinkOne.Add(_right, _right);
							_newPolygon.AddPoint(_right);
							_left = _right;
							break;
						}
					}

				}
			}
			return _newPolygon;
		}
	}
}
