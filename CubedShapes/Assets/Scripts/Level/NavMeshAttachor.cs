using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAttachor : MonoBehaviour {

    //Public
    public static System.Collections.Generic.Dictionary<Transform, Ground> generated = new System.Collections.Generic.Dictionary<Transform, Ground>();


    //Internal
    //private System.Collections.ArrayList children;
    private System.Collections.Generic.SortedDictionary<float, System.Collections.Generic.Dictionary<Transform, System.Collections.Generic.Dictionary<string, GameObject>>> heightSortedLinks;
    private System.Collections.Generic.Dictionary<Transform,Transform> linkToGround;
    private System.Collections.Generic.Dictionary<Transform, Transform> alreadyLinked;


    private NavMeshSurface nav;

    private static string RIGHT                  = "Right";
    private static string LEFT                   = "Left";
    private static string LINK                   = " - Link - ";
    private static string DROP                   = "Drop";
    private static string LINK_LEFT              = LINK + LEFT;
    private static string LINK_RIGHT             = LINK + RIGHT;
    private static float  LINK_EDGE_DISTANCE     = 0.7f;
    private static float  LINK_JUMP_DISTANCE_X   = 5;

    // Use this for initialization
    void Start () {
        if (DevelopmentSettings.ACTIVATE_NAVMESH)
        {
            nav = transform.GetComponent<NavMeshSurface>();
            //children = new System.Collections.ArrayList();
            heightSortedLinks = new System.Collections.Generic.SortedDictionary<float, System.Collections.Generic.Dictionary<Transform, System.Collections.Generic.Dictionary<string,GameObject>>>();
            linkToGround = new System.Collections.Generic.Dictionary<Transform, Transform>();
            alreadyLinked = new System.Collections.Generic.Dictionary<Transform, Transform>();

            ListChildren(transform);
           
            CreateLinkGameObjects();
            CreateNavMeshLinksAndGrounds();
            GenerateDistancesBetweenGrounds();

            if (nav != null)
            {
                nav.BuildNavMesh();
            }
        }
	}
    private void GenerateDistancesBetweenGrounds()
    {
        foreach(Ground g in generated.Values)
        {
            g.GenerateDistanceLists();
        }
    }

    private void CreateNavMeshLinksAndGrounds()
    {
        System.Collections.Generic.SortedDictionary<string, GameObject> added = new System.Collections.Generic.SortedDictionary<string, GameObject>();
        foreach (float f in heightSortedLinks.Keys)
        {
            foreach (Transform t in heightSortedLinks[f].Keys)
            {
                //bool leftLinkFound = false;
                //bool rightLinkFound = false;
                GameObject leftLink = heightSortedLinks[f][t][LINK_LEFT];
                GameObject rightLink = heightSortedLinks[f][t][LINK_RIGHT];
                float yPos = -f;
                foreach (float comp in heightSortedLinks.Keys)
                {
                    float yPosComp = -comp;
                    if (yPosComp <= yPos)
                    {
                        foreach (Transform c in heightSortedLinks[comp].Keys)
                        {
                            GameObject compLeftLink = heightSortedLinks[comp][c][LINK_LEFT];
                            GameObject compRightLink = heightSortedLinks[comp][c][LINK_RIGHT];
                            string leftLinkName = leftLink.name + " to " + compRightLink.name;
                            string rightLinkName = rightLink.name + " to " + compLeftLink.name;

                            if (//!leftLinkFound //!added.ContainsKey(leftLinkName)
                                    !(alreadyLinked.ContainsKey(leftLink.transform) || alreadyLinked.ContainsKey(compRightLink.transform))
                                    &&
                                    compLeftLink.transform.position.x < leftLink.transform.position.x
                                    &&
                                    compRightLink.transform.position.x + LINK_JUMP_DISTANCE_X > leftLink.transform.position.x
                                )
                            {
                                if (compRightLink.transform.position.x + LINK_JUMP_DISTANCE_X > leftLink.transform.position.x
                                    &&
                                    compRightLink.transform.position.x < leftLink.transform.position.x)
                                {
                                    GameObject mid = Link(rightLinkName, leftLink.transform, compRightLink.transform);
                                    added.Add(leftLinkName, mid);
                                    alreadyLinked.Add(leftLink.transform, leftLink.transform);
                                    alreadyLinked.Add(compRightLink.transform, compRightLink.transform);

                                    //leftLinkFound = true;
                                }
                            }
                            if (//!rightLinkFound //!added.ContainsKey(rightLinkName)
                                    !(alreadyLinked.ContainsKey(rightLink.transform) || alreadyLinked.ContainsKey(compLeftLink.transform))
                                    &&
                                    compLeftLink.transform.position.x - LINK_JUMP_DISTANCE_X < rightLink.transform.position.x
                                    &&
                                    compRightLink.transform.position.x  > rightLink.transform.position.x
                                    &&
                                    compLeftLink.transform.position.x > rightLink.transform.position.x
                                )
                            {
                                    GameObject mid = Link(rightLinkName, rightLink.transform, compLeftLink.transform);
                                    added.Add(rightLinkName, mid);
                                    alreadyLinked.Add(rightLink.transform, rightLink.transform);
                                    alreadyLinked.Add(compLeftLink.transform, compLeftLink.transform);
                                    //rightLinkFound = true;
                            }
                        }

                    }
                    //Debug.Log(yPos);
                }
                if (!alreadyLinked.ContainsKey(leftLink.transform))//!leftLinkFound)
                {
                    GameObject go = RayHit(leftLink, -LINK_EDGE_DISTANCE * 2);
                    if(go != null)
                    {
                        
                        Link(leftLink.name + DROP + LINK + LEFT + DROP, leftLink.transform, go.transform);
                    }
                }
                if (!alreadyLinked.ContainsKey(rightLink.transform))//!rightLinkFound)
                {
                    GameObject go = RayHit(rightLink, LINK_EDGE_DISTANCE * 2);
                    if (go != null)
                    {
                        Link(rightLink.name + DROP + LINK + RIGHT + DROP, rightLink.transform, go.transform);
                    }
                }

            }
            //Input.OrderBy(key => key.Key); links.Keys
        }
        //Debug.Log("end");
    }


    private GameObject RayHit(GameObject from, float xShift)
    {
        GameObject go = null;
        RaycastHit hit;
        int layer_mask = LayerMask.GetMask(Organizer.LAYER_GROUND);
        if (Physics.Raycast(new Vector3(from.transform.position.x + xShift, from.transform.position.y, from.transform.position.z)
                            , transform.TransformDirection(Vector3.down)
                            , out hit
                            , Mathf.Infinity
                            , layer_mask
                            ))
        {
            go = new GameObject(hit.transform.gameObject.name + DROP + from.name);
            go.transform.parent = from.transform;
            go.transform.position = new Vector3(from.transform.position.x + xShift,
                                                from.transform.position.y - hit.distance,
                                                from.transform.position.z);
            
            linkToGround.Add(go.transform, hit.transform);
        }
        return go;
        
    }

    private GameObject Link(string name, Transform from, Transform to)
    {
        GameObject mid = new GameObject(name);
        mid.transform.parent = from.transform;
        mid.transform.position = new Vector3(to.position.x + (from.position.x - to.position.x) / 2,
                                             to.position.y + (from.position.y - to.position.y) / 2,
                                             from.position.z);
        NavMeshLink navLink = mid.AddComponent<NavMeshLink>();
        navLink.startPoint = new Vector3((from.position.x - to.position.x) / 2, (from.position.y - to.position.y) / 2, 0); //leftLink.transform.position;
        navLink.endPoint = new Vector3(-(from.position.x - to.position.x) / 2, -(from.position.y - to.position.y) / 2, 0);
        navLink.width = Mathf.Min(from.localScale.z, to.localScale.z);

        // Add generated links to list
        if(generated.ContainsKey(linkToGround[from])  && generated.ContainsKey(linkToGround[to])) {
            Vector3 fromPoint = new Vector3(from.position.x, from.position.y, 0);
            Vector3 toPoint = new Vector3(to.position.x, to.position.y, 0);


            generated[linkToGround[from]].links.Add(fromPoint, generated[linkToGround[to]]);
            generated[linkToGround[to]].links.Add(toPoint, generated[linkToGround[from]]);
            generated[linkToGround[from]].startPointToEndPoint.Add(fromPoint, toPoint);
            generated[linkToGround[to]].startPointToEndPoint.Add(toPoint, fromPoint);

        }
        else
        {
            Debug.Log("Unable to link: " + from.gameObject.name +" to "+ to.gameObject.name);
        }

        return mid;
    }
 
    
    private void CreateLinkGameObjects()
    {
        foreach (Transform child in generated.Keys)//children)
        {
            System.Collections.Generic.Dictionary<string, GameObject> linkList = new System.Collections.Generic.Dictionary<string, GameObject>();
            float yPos = child.position.y + child.localScale.y / 2;
            float linkName = -yPos;

            GameObject linkLeft = new GameObject(child.gameObject.name + LINK + LEFT);
            linkLeft.transform.parent = GameObject.Find(DevelopmentSettings.LINKS_NODE).transform;
            linkLeft.transform.position = new Vector3(child.position.x - child.localScale.x / 2 + LINK_EDGE_DISTANCE, yPos, child.position.z);
            GameObject linkRight = new GameObject(child.gameObject.name + LINK + RIGHT);
            linkRight.transform.parent = GameObject.Find(DevelopmentSettings.LINKS_NODE).transform;
            linkRight.transform.position = new Vector3(child.position.x + child.localScale.x / 2 - LINK_EDGE_DISTANCE, yPos, child.position.z);

            linkList.Add(LINK_LEFT, linkLeft);
            linkList.Add(LINK_RIGHT, linkRight);

            linkToGround.Add(linkLeft.transform, child);
            linkToGround.Add(linkRight.transform, child);

            if (heightSortedLinks.ContainsKey(linkName))
            {
                heightSortedLinks[linkName].Add(child, linkList);
            }
            else
            {
                System.Collections.Generic.Dictionary<Transform, System.Collections.Generic.Dictionary<string, GameObject>> pos = new System.Collections.Generic.Dictionary<Transform, System.Collections.Generic.Dictionary<string, GameObject>>();
                pos.Add(child, linkList);
                heightSortedLinks.Add(linkName, pos);
            }
        }
    }

    private void ListChildren(Transform t)
    {
        foreach(Transform child in t)
        {
            if (child.GetComponent<BoxCollider>() != null)
            {
                generated.Add(child, new Ground(child));
            }
            ListChildren(child);
        }
    }


    // Update is called once per frame
    void Update () {
		
	}
}
