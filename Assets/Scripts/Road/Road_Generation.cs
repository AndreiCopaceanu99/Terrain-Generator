using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Road_Generation : MonoBehaviour
{
    [SerializeField]
    GameObject[] Roads;     // The prefabs of the raods
    int Road_Type;          // The value of the active road
    int New_Road_Type;      // The value of the road that is getting create

    GameObject[] Road_Type_Game;    // The road type in the game world

    [SerializeField]
    GameObject Turtle;      // Visual representation of the process
    [SerializeField]
    Camera Cam;             // Camera
    
    [SerializeField]
    int Max_Lenght;

    GameObject[] Sections;  // Stores all the road sections
    int Active_Road=0;      

    int Current_Lenght = 1;
    GameObject Current_Section; // The prefab that is going to be created

    GameObject[] GO;        // The active road section in the game world

    [SerializeField]
    bool Step_By_Step;      // The way update goes, manual or automatic

    // Start is called before the first frame update
    void Start()
    {
        Sections = new GameObject[Max_Lenght];
        Road_Type_Game = new GameObject[4];
        Road_Type = 0;
        Current_Section = Roads[Road_Type];
        Sections[0] = Current_Section;
        GO = new GameObject[Max_Lenght];
        GO[0]=Instantiate(Current_Section, new Vector3(0, 5f, 0), Quaternion.identity);
        Road_Type_Game[0] = GO[0];
        Turtle.transform.position = Current_Section.transform.position;
        Turtle.transform.rotation = Current_Section.transform.rotation;
        Cam.transform.position = new Vector3(Turtle.transform.position.x, 500f, Turtle.transform.position.z);
        Get_Connectors(GO[Active_Road]);
        Active_Road++;
    }

    // Update is called once per frame
    void Update()
    {
        if (Step_By_Step)       // Manual process
        {
            if (Input.GetKeyDown(KeyCode.A) && Current_Lenght < Max_Lenght)
            {
                Get_Connectors(GO[Active_Road]);
                Active_Road++;
            }
        }
        else              // Automatic process
        {
            if (Current_Lenght < Max_Lenght)
            {
                Get_Connectors(GO[Active_Road]);
                Active_Road++;
            }
        }
    }

    void Get_Connectors(GameObject Sect)
    {
        Connectors Con = Sect.GetComponentInChildren<Connectors>();
        Transform Section_Mesh = Sect.gameObject.transform.GetChild(0);
        foreach(GameObject a in Con.Connector)  // Goes through all the connectors of the road section
        {
            if (Current_Lenght < Max_Lenght)
            {
                if (a.transform.position.z > Section_Mesh.position.z)
                {
                    Turtle.transform.position = a.transform.position + new Vector3(0, 0, -7.3f);
                }
                if (a.transform.position.z < Section_Mesh.position.z)
                {
                    Turtle.transform.position = a.transform.position + new Vector3(0, 0, 7.3f);
                }
                if (a.transform.position.x > Section_Mesh.position.x)
                {
                    Turtle.transform.position = a.transform.position + new Vector3(-7.3f, 0, 0);
                }
                if (a.transform.position.x < Section_Mesh.position.x)
                {
                    Turtle.transform.position = a.transform.position + new Vector3(7.3f, 0, 0);
                }

                //Debug.Log(a.transform.position + " / " + Section_Mesh.transform.position);
                if(a.transform.position.x != Section_Mesh.position.x && a.transform.position.z != Section_Mesh.position.z)
                {
                    Debug.Log(a.transform.position + " / " + Section_Mesh.position + " / " + Sect.name);
                }
                Turtle.transform.rotation = a.transform.rotation;
                Extend_Road();
            }
        }
        Road_Type = 0;
    }

    void Extend_Road()  // Checks what type of road is the active section
    {
        if(Road_Type==0)
        {
            if (GO[Active_Road].name == Road_Type_Game[Road_Type].name)
            {
                New_Road_Type = 1;
                Current_Section = Roads[New_Road_Type];
                Build_Section(Current_Section);
            }
            else
            {
                Road_Type++;
            }
        }

        if(Road_Type==1)
        {
            if (GO[Active_Road].name == Road_Type_Game[Road_Type].name)
            {
                New_Road_Type = Random.Range(0, 15);
                if(New_Road_Type>=4)
                {
                    New_Road_Type = 1;
                }
                Current_Section = Roads[New_Road_Type];
                Build_Section(Current_Section);
            }
            else
            {
                Road_Type++;
            }
        }

        if(Road_Type==2)
        {
            if (GO[Active_Road].name == Road_Type_Game[Road_Type].name)
            {
                New_Road_Type = 1;
                Current_Section = Roads[New_Road_Type];
                if (GO[Active_Road] == Road_Type_Game[New_Road_Type])
                {
                    Current_Section = Roads[1];
                }
                Build_Section(Current_Section);
            }
            else
            {
                Road_Type++;
            }
        }

        if(Road_Type==3)
        {
            New_Road_Type = 1;
            Current_Section = Roads[New_Road_Type];
            if (GO[Active_Road] == Road_Type_Game[New_Road_Type])
            {
                Current_Section = Roads[1];
            }
            Build_Section(Current_Section);
        }
    }

    void Build_Section(GameObject Section)
    {
        if (New_Road_Type == 2)
        {
            GO[Current_Lenght] = Instantiate(Section, Turtle.transform.position, Quaternion.Euler(0, (Turtle.transform.eulerAngles.y - 180f), 0));
        }
        else
        {
            GO[Current_Lenght] = Instantiate(Section, Turtle.transform.position, Turtle.transform.rotation);
        }
        foreach(GameObject a in Road_Type_Game)
        {
            if(a==null)
            {
                Road_Type_Game[New_Road_Type] = GO[Current_Lenght]; // Adds a new road section from the game world in the array
            }
        }
        Sections[Current_Lenght] = Section;
        Current_Lenght++;
    }
}