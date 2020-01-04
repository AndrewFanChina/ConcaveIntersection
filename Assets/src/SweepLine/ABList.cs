using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
///  动态数组
///  @author AndrewFan
/// </summary>
/// <typeparam name="T">任意类型</typeparam>
public class AB_List<T> : IEnumerable<T> where T:IComparable<T>
{
	protected int m_capacity = 10;  // 容量
	protected T[] m_items;// 内部数组
	protected int m_length;// 存放的单元个数
	protected int m_mayIdleID;// 可能空闲的单元下标

	protected IEnumerator<T>[] m_enumerators; //枚举器组
	protected bool[] m_enumStates;//枚举器组当前占用状态
	public AB_List()
	{
		init(5);
	}
	public AB_List(int capacity, int enumCount = 5)
	{
		init(enumCount);
	}
	protected void init(int enumCount)
	{
		m_capacity = m_capacity < 10 ? 10 : m_capacity;
		enumCount = enumCount < 5 ? 5 : enumCount;
		m_items = new T[m_capacity];
		if(m_enumerators == null)
		{
			m_enumerators = new IEnumerator<T>[enumCount];
			m_enumStates = new bool[enumCount];
			for(int i = 0; i < m_enumerators.Length; i++)
			{
				m_enumerators[i] = new ABEnumerator<T>(this, i);
			}
		}
	}
	/// <summary>
	/// 增加单元
	/// </summary>
	/// <param name="element">添加的单元</param>
	public virtual void Add(T element)
	{
		increaseCapacity();
		// 赋值
		m_items[m_length] = element;
		m_length++;
	}

	/// <summary>
	/// 插入单元
	/// </summary>
	/// <param name="index">插入位置</param>
	/// <param name="element">单元</param>
	/// <returns>操作是否成功</returns>
	public virtual bool Insert(int index, T element)
	{
		if(index < 0)
		{
			return false;
		}
		if(index >= m_length)
		{
			Add(element);
			return true;
		}
		increaseCapacity();
		// 向后拷贝
		// for(int i=length;i>index;i--)
		// {
		// datas[i]=datas[i-1];
		// }
		System.Array.Copy(m_items, index, m_items, index + 1, m_length - index);

		m_items[index] = element;

		m_length++;
		return true;
	}

	public virtual T this[int index]
	{
		get
		{
			//取位于某个位置的单元
			if(index < 0 || index >= m_length)
			{
				throw new InvalidOperationException();
			}
			return m_items[index];
		}
		set
		{
			//设置位于某个位置的单元
			if(index < 0 || index >= m_length)
			{
				throw new InvalidOperationException();
			}
			m_items[index] = value;
		}
	}

	/// <summary>
	/// 增长容量
	/// </summary>
	protected void increaseCapacity()
	{
		if(m_length >= m_capacity)
		{
			int newCapacity = m_capacity;
			if(newCapacity == 0)
			{
				newCapacity++;
			}
			newCapacity *= 2;
			T[] datasNew = new T[newCapacity];
			System.Array.Copy(m_items, 0, datasNew, 0, m_length);
			m_items = datasNew;
			m_capacity = newCapacity;
		}
	}
	/// <summary>
	/// 清空单元数组
	/// </summary>
	public virtual void Clear()
	{
		for(int i = 0; i < m_length; i++)
		{
			m_items[i] = default(T);
		}
		m_length = 0;
	}


	/// <summary>
	/// 是否包含某个单元
	/// </summary>
	/// <param name="element">单元</param>
	/// <returns>是否包含</returns>
	public bool Contains(T element)
	{
		for(int i = 0; i < m_length; i++)
		{
			if(m_items[i].Equals(element))
			{
				return true;
			}
		}
		return false;
	}


	/// <summary>
	/// 获取指定单元在当前列表中的位置，从前向后查找
	/// </summary>
	/// <param name="element">单元</param>
	/// <returns>位置</returns>
	public int IndexOf(T element)
	{
		for(int i = 0; i < m_length; i++)
		{
			if(m_items[i].Equals(element))
			{
				return i;
			}
		}
		return -1;
	}
	/// <summary>
	/// 获取指定单元在当前列表中的位置，从后先前查找
	/// </summary>
	/// <param name="element">单元</param>
	/// <returns>位置</returns>
	public int LastIndexOf(T element)
	{
		for(int i = m_length - 1; i >= 0; i--)
		{
			if(m_items[i].Equals(element))
			{
				return i;
			}
		}
		return -1;
	}

	/// <summary>
	/// 获得长度
	/// </summary>
	public virtual int Count
	{
		get
		{
			return m_length;
		}
	}
	/// <summary>
	/// 移除指定位置的单元，如果单元归属权属于当前列表，则会将其卸载
	/// </summary>
	/// <param name="index">位置索引</param>
	/// <returns>移除掉的单元</returns>
	public virtual void RemoveAt(int index)
	{
		if(index < 0 || index >= m_length)
		{
			return;
		}
		for(int i = index; i <= m_length - 2; i++)
		{
			m_items[i] = m_items[i + 1];
		}
		m_length--;
	}
	/// <summary>
	/// 移除指定尾部单元
	/// </summary>
	/// <returns>移除掉的单元</returns>
	public virtual T RemoveEnd()
	{
		if(m_length <= 0)
		{
			return default(T);
		}
		T temp = m_items[m_length - 1];
		m_items[m_length - 1] = default(T);
		m_length--;
		return temp;
	}
	/// <summary>
	/// 从指定位置开始(包括当前)，移除后续单元，如果单元归属权属于当前列表，则会将其卸载
	/// </summary>
	/// <param name="index">要移除的位置</param>
	/// <param name="innerMove">是否是内部移动</param>
	/// <returns>被移除的个数，如果index越界，则返回-1</returns>
	public virtual int RemoveAllFrom(int index)
	{
		if(index < 0 || index >= m_length)
		{
			return -1;
		}
		int removedNum = 0;
		for(int i = m_length - 1; i >= index; i--)
		{
			m_items[i] = default(T);
			m_length--;
			removedNum++;
		}
		return removedNum;
	}
	/// <summary>
	/// 移除指定单元，如果单元归属权属于当前列表，则会将其卸载
	/// </summary>
	/// <param name="element">单元</param>
	/// <returns>是否操作成功</returns>
	public virtual bool Remove(T element)
	{
		int index = IndexOf(element);
		if(index < 0)
		{
			return false;
		}
		RemoveAt(index);
		return true;
	}
	/// <summary>
	/// 获取所有数据，注意这里的数据可能包含了很多冗余空数据，长度>=当前数组长度。
	/// </summary>
	/// <returns>所有数据数组</returns>
	public T[] GetAllItems()
	{
		return m_items;
	}
	/// <summary>
	/// 转换成定长数组，伴随着内容拷贝。
	/// 如果是值类型数组，将与本动态数组失去关联；
	/// 如果是引用类型数组，将与本动态数组保存相同的引用。
	/// </summary>
	/// <returns>数组</returns>
	public virtual Array ToArray()
	{
		T[] array = new T[m_length];
		for(int i = 0; i < m_length; i++)
		{
			array[i] = m_items[i];
		}
		return array;
	}
	/// <summary>
	/// 显示此数组，每个单元之间以逗号分隔
	/// </summary>
	public void Show()
	{
		string text = "";
		for(int i = 0; i < m_length; i++)
		{
			T obj = m_items[i];
			text += (obj.ToString() + ",");
		}
		Debug.Log(text);
	}

	/// <summary>
	/// 显示此数组，每个单元一行
	/// </summary>
	public void ShowByLines()
	{
		string text = "";
		for(int i = 0; i < m_length; i++)
		{
			T obj = m_items[i];
			text += (obj.ToString());
		}
		Debug.Log(text);
	}


	public IEnumerator<T> GetEnumerator()
	{
		//搜索可用的枚举器
		int idleEnumID = -1;
		for(int i = 0; i < m_enumStates.Length; i++)
		{
			int tryID = i + m_mayIdleID;
			if(!m_enumStates[tryID])
			{
				idleEnumID = tryID;
				break;
			}
		}
		if(idleEnumID < 0)
		{
			Debug.LogError("use too much enumerators");
		}
		//标记为已经使用状态
		m_enumStates[idleEnumID] = true;
		m_enumerators[idleEnumID].Reset();
		//向前迁移空闲坐标
		m_mayIdleID = (m_mayIdleID + 1) % m_enumStates.Length;
		return m_enumerators[idleEnumID];
	}
	IEnumerator IEnumerable.GetEnumerator()
	{
		return null;
	}

	public void QuickSort()
	{
		QuickSort(0, m_length - 1);
	}
	private void QuickSort(int left, int right)
	{
		if(right - left <= 0) // if size <= 1,
		{
			return; // already sorted
		}
		else
		// size is 2 or larger
		{
			var pivot = m_items[right]; // rightmost item
										// partition range
			int partition = partitionIt1(left, right, pivot);
			QuickSort(left, partition - 1); // sort left side
			QuickSort(partition + 1, right); // sort right side
		}
	} // end recQuickSort()
	private int partitionIt1(int left, int right, T pivot)
	{
		int leftPtr = left - 1; // left (after ++)
		int rightPtr = right; // right-1 (after --)
		var pivotCode = pivot;
		while(true)
		{	// find bigger item
			while(m_items[++leftPtr].CompareTo(pivotCode)<0); // (nop) find smaller item
			while(rightPtr > 0 && m_items[--rightPtr].CompareTo(pivotCode)>0); // (nop)
			if(leftPtr >= rightPtr) // if pointers cross,
			{
				break; // partition done
			}
			else // not crossed, so
			{
				swap(leftPtr, rightPtr); // swap elements
			}
		} // end while(true)
		swap(leftPtr, right); // restore pivot
		return leftPtr; // return pivot location
	} // end partitionIt()
	private void swap(int dex1, int dex2) // swap two elements
	{
		var temp = m_items[dex1]; // A into temp
		m_items[dex1] = m_items[dex2]; // B into A
		m_items[dex2] = temp; // temp into B
	}
	struct ABEnumerator<T> : IDisposable, IEnumerator<T> where T : IComparable<T>
	{
		private AB_List<T> m_list;
		private int m_idNext;
		private T m_current;
		private int m_id;
		public object Current
		{
			get
			{
				if(m_idNext <= 0)
				{
					throw new InvalidOperationException();
				}
				return m_current;
			}
		}
		T IEnumerator<T>.Current
		{
			get
			{
				return m_current;
			}
		}

		internal ABEnumerator(AB_List<T> list, int id)
		{
			m_list = list;
			m_idNext = 0;
			m_id = id;
			m_current = default(T);
		}

		void IEnumerator.Reset()
		{
			m_idNext = 0;
		}

		public void Dispose()
		{
			//m_list = null;
			//清除使用标记
			m_list.m_enumStates[m_id] = false;
			m_list.m_mayIdleID = m_id;
		}


		public bool MoveNext()
		{
			if(m_list == null)
			{
				throw new ObjectDisposedException(base.GetType().FullName);
			}
			if(m_idNext < 0)
			{
				return false;
			}
			if(m_idNext < m_list.Count)
			{
				m_current = m_list.m_items[m_idNext++];
				return true;
			}
			m_idNext = -1;
			return false;
		}
	}
}
