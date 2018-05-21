using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground {

    public Transform obj;
    public System.Collections.Generic.Dictionary<Vector3, Ground> links;
    public System.Collections.Generic.Dictionary<Vector3, Vector3> startPointToEndPoint;
    public System.Collections.Generic.Dictionary<Vector3, System.Collections.Generic.Dictionary<Ground,int>> distances;

    public Ground(Transform groundObject)
    {
        this.obj = groundObject;
        this.links = new System.Collections.Generic.Dictionary<Vector3, Ground>();
        this.startPointToEndPoint = new System.Collections.Generic.Dictionary<Vector3, Vector3>();
        this.distances = new System.Collections.Generic.Dictionary<Vector3, System.Collections.Generic.Dictionary<Ground, int>>();
    }
    public Vector3 GetMidPoint()
    {
        return new Vector3(obj.transform.position.x, obj.transform.position.y+obj.transform.localScale.y/2);
    }

    public void GenerateDistanceLists()
    {
        foreach(Vector3 link in links.Keys)
        {
            System.Collections.Generic.Dictionary<Ground, int> currentSearch = new System.Collections.Generic.Dictionary<Ground, int>();
            AddDistance(links[link], 1, currentSearch);
            distances.Add(link, currentSearch);
        }
    }
    private void AddDistance(Ground search, int depth, System.Collections.Generic.Dictionary<Ground, int> currentSearch)
    {
        currentSearch.Add(search, depth);

        foreach (Ground curr in search.links.Values)
        {
            if (!currentSearch.ContainsKey(curr) && curr != this)
            {
                AddDistance(curr, depth + 1, currentSearch);
            }
        }
    }

}
