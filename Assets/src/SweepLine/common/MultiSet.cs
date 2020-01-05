using System.Collections.Generic;
using System.Diagnostics;


public class MultiSet<TKey, TValue> : Dictionary<TKey, HashSet<TValue>>
{
	public MultiSet()
		: base()
	{
	}

	public void Add(TKey key, TValue value)
	{
		Debug.Assert(key!=null);
		HashSet<TValue> container = null;
		if (!this.TryGetValue(key, out container))
		{
			container = new HashSet<TValue>();
			base.Add(key, container);
		}
		container.Add(value);
	}

	public bool ContainsValue(TKey key, TValue value)
	{
		Debug.Assert(key != null);
		bool toReturn = false;
		HashSet<TValue> values = null;
		if (this.TryGetValue(key, out values))
		{
			toReturn = values.Contains(value);
		}

		return toReturn;
	}

	public void Remove(TKey key, TValue value)
	{
		Debug.Assert(key != null);
		HashSet<TValue> container = null;
		if(this.TryGetValue(key, out container))
		{
			container.Remove(value);
			if(container.Count <= 0)
			{
				this.Remove(key);
			}
		}
	}

	public HashSet<TValue> GetValues(TKey key, bool returnEmptySet)
	{
		HashSet<TValue> toReturn = null;
		if(!base.TryGetValue(key, out toReturn) && returnEmptySet)
		{
			toReturn = new HashSet<TValue>();
		}
		return toReturn;
	}
}