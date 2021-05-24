using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Change_Height : MonoBehaviour
{
    //[SerializeField]
    Mesh mesh;
    Connectors connectors;
    Road_Generation road_Generation;

    Detect_Height overlap;
    GameObject Height_Detector;

    Vector3[] vertices;

    bool Ready;

    public float Height;

    MeshCollider Mesh_Collider;

    // Start is called before the first frame update
    void Start()
    {
        overlap = GetComponentInChildren<Detect_Height>();
        connectors = GetComponent<Connectors>();
        mesh = GetComponent<MeshFilter>().mesh;
        road_Generation = FindObjectOfType<Road_Generation>();
        if (connectors.Connector.Count != 0)
        {
            Height_Detector = connectors.Connector[0];
            overlap = Height_Detector.GetComponent<Detect_Height>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!Ready)
        {
            Set_Height();
        }

        if (connectors.Connector.Count == 1)
        {
            if (!connectors.Ready)
            {
                connectors.Connector[0].transform.localPosition = new Vector3(connectors.Connector[0].transform.localPosition.x, connectors.Connector[0].transform.localPosition.y - Height, connectors.Connector[0].transform.localPosition.z);
                connectors.Ready = true;
                Mesh_Collider = gameObject.AddComponent<MeshCollider>();
            }
        }
        else
        {
            connectors.Ready = true;
        }
    }

    void Set_Height()
    {
        vertices = mesh.vertices;
        if (overlap.Hitpoint.position.y != transform.position.y)
        {
            Height = transform.position.y - overlap.Hitpoint.position.y - 4f;
            if (Height >= -5 && Height <= 5)
            {
                for (int i = 0; i < vertices.Length; i++)
                {
                    if (i == 0 || i == 1 || i == 2 || i == 3 || i == 4 || i == 7 || i == 13 || i == 14 || i == 16 || i == 17 || i == 22 || i == 23)
                    {
                        vertices[i] = new Vector3(mesh.vertices[i].x, mesh.vertices[i].y - Height, mesh.vertices[i].z);
                    }
                }
            }
            else
            {
                Connectors con = this.GetComponent<Connectors>();
                if (con.Connector.Count == 1)
                {
                    con.Connector.Remove(con.Connector[0]);
                }
            }

            /*for (int i = 0; i < vertices.Length; i++)
            {
                if (i == 0 || i == 1 || i == 2 || i == 3 || i == 4 || i == 7 || i == 13 || i == 14 || i == 16 || i == 17 || i == 22 || i == 23)
                {
                    vertices[i] = new Vector3(mesh.vertices[i].x, mesh.vertices[i].y - Height, mesh.vertices[i].z);
                }
            }*/
        }
        mesh.vertices = vertices;
        Ready = true;
    }
}
