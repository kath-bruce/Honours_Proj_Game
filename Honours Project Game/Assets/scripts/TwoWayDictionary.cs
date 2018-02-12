using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using UnityEngine;

public class TwoWayDictionary<F>
{
    private Dictionary<F, GameObject> FToGO;
    private Dictionary<GameObject, F> GOToF;

    public TwoWayDictionary()
    {
        FToGO = new Dictionary<F, GameObject>();
        GOToF = new Dictionary<GameObject, F>();
    }

    public void Add(F fType, GameObject go)
    {
        FToGO.Add(fType, go);
        GOToF.Add(go, fType);
    }

    public void Clear()
    {
        FToGO.Clear();
        GOToF.Clear();
    }

    public F GetfType(GameObject go)
    {
        F fType;

        //try get is used as using [] will create entry leading to imbalance in both dictionaries
        if (GOToF.TryGetValue(go, out fType))
        {
            return fType;
        }
        else
        {
            return default(F);
        }
    }

    public GameObject GetGO(F fType)
    {
        GameObject go;

        //try get is used as using [] will create entry leading to imbalance in both dictionaries
        if (FToGO.TryGetValue(fType, out go))
        {
            return go;
        }
        else
        {
            return null;
        }
    }

    public void RemovefTypeAndGO(F fType, GameObject go)
    {
        FToGO.Remove(fType);
        GOToF.Remove(go);
    }

    public void RemovefType(F fType)
    {
        GameObject go;
        FToGO.TryGetValue(fType, out go);

        RemovefTypeAndGO(fType, go);
    }

    public void RemoveGO(GameObject go)
    {
        F fType;
        GOToF.TryGetValue(go, out fType);

        RemovefTypeAndGO(fType, go);
    }

    public F[] GetFs()
    {
        return FToGO.Keys.ToArray();
    }

    public GameObject[] GetGOs()
    {
        return GOToF.Keys.ToArray();
    }

    public int GetCount()
    {
        if (GOToF.Count != FToGO.Count)
        {
            Debug.LogError("TwoWayDictionary dictionaries have differing counts!!!!");
            return -1;
        }
        else
        {
            return FToGO.Count;
        }

    }

    public bool ContainsF(F f)
    {
        return FToGO.ContainsKey(f) && GOToF.ContainsValue(f);
    }

    public bool ContainsGO(GameObject go)
    {
        return GOToF.ContainsKey(go) && FToGO.ContainsValue(go);
    }

    public bool ContainsFandGo(F f, GameObject go)
    {
        return ContainsF(f) && ContainsGO(go);
    }
}
