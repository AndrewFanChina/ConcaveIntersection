using System.Collections;
using System.Collections.Generic;
using SweepLine;
using UnityEngine;

public class GCTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		for(int i = 0; i < 100; i++)
		{
			Point2 _p = new Point2(Random.Range(0, 10), Random.Range(0, 10));
			if(!m_testTable.ContainsKey(_p))
			{
				m_testTable.Add(_p, _p);
			}
		}
	}
    Dictionary<Point2, Point2> m_testTable=new Dictionary<Point2, Point2>();//new Point2.Comparer()
																			// Update is called once per frame
	void Update()
    {
	    for (int i = 0; i < 100; i++)
	    {
			Point2 _p = new Point2(Random.Range(0, 10), Random.Range(0, 10));
			if(!m_testTable.ContainsKey(_p))
			{
				m_testTable.Add(_p, _p);
			}
			else
			{
				m_testTable.Remove(_p);
			}
		}

    }
}
