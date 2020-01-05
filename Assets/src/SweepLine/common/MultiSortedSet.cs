using System.Collections.Generic;
using System.Diagnostics;


public class MultiSortedSet<TKey, TValue> : Dictionary<TKey, SortedSet<TValue>>
{
	public MultiSortedSet()
		: base()
	{
	}

	public void Add(TKey key, TValue value)
	{
		Debug.Assert(key!=null);
		SortedSet<TValue> container = null;
		if (!this.TryGetValue(key, out container))
		{
			container = new SortedSet<TValue>();
			base.Add(key, container);
		}
		container.Add(value);
	}

	public bool ContainsValue(TKey key, TValue value)
	{
		Debug.Assert(key != null);
		bool toReturn = false;
		SortedSet<TValue> values = null;
		if (this.TryGetValue(key, out values))
		{
			toReturn = values.Contains(value);
		}

		return toReturn;
	}

	public void Remove(TKey key, TValue value)
	{
		Debug.Assert(key != null);
		SortedSet<TValue> container = null;
		if(this.TryGetValue(key, out container))
		{
			container.Remove(value);
			if(container.Count <= 0)
			{
				this.Remove(key);
			}
		}
	}

	public SortedSet<TValue> GetValues(TKey key, bool returnEmptySet)
	{
		SortedSet<TValue> toReturn = null;
		if(!base.TryGetValue(key, out toReturn) && returnEmptySet)
		{
			toReturn = new SortedSet<TValue>();
		}
		return toReturn;
	}
}