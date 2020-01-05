using System.Collections.Generic;
public class LittlePool<T> where T : new()
{
	private List<T> m_Pools = new List<T>();
	public T Take
	{
		get
		{
			T _result;
			int _count=m_Pools.Count;
			if ( _count > 0)
			{
				_result = m_Pools[_count-1];
				m_Pools.RemoveAt(_count-1);
			}
			else
			{
				_result = new T();
			}
			return _result;
		}
	}

	public void GiveBack(T value)
	{
		m_Pools.Add(value);
	}
}


