using HoloToolkit.Unity;
using System.Collections.Generic;
using UnityEngine;

public class ArrowObjectPool : Singleton<ArrowObjectPool>
{
	public Indicator pooledObject;
	public int pooledAmount = 1;
	public bool willGrow = true;

	List<Indicator> pooledObjects;


	void Start()
	{
		pooledObjects = new List<Indicator>();

		for (int i = 0; i < pooledAmount; i++)
		{
			Indicator arrow = Instantiate(pooledObject);
            arrow.DefaultRotation = arrow.transform.rotation;
            arrow.transform.SetParent(transform, false);
            arrow.Activate(false);
			pooledObjects.Add(arrow);
		}
	}

	public Indicator GetPooledObject()
	{

        //TODO --->>只取第一个
        Indicator _indicator = pooledObjects[0];
        _indicator.Activate(true);
        return _indicator;

        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].Active)
            {
                return pooledObjects[i];
            }
        }
        if (willGrow)
        {
            Indicator arrow = Instantiate(pooledObject);
            arrow.DefaultRotation = arrow.transform.rotation;
            arrow.transform.SetParent(transform, false);
            arrow.Activate(false);
            pooledObjects.Add(arrow);
            return arrow;
        }
        return null;
	}

	public void DeactivateAllPooledObjects()
	{
		foreach (Indicator arrow in pooledObjects)
		{
			arrow.Activate(false);
		}
	}   
}
