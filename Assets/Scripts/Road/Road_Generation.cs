using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Road_Generation : MonoBehaviour
{
    [SerializeField]
    GameObject[] Roads;     // The prefabs of the raods
    int Road_Type;          // The value of the active road
    int New_Road_Type;      // The value of the road that is getting create

    public GameObject[] Road_Type_Game;    // The road type in the game world

    [SerializeField]
    GameObject Turtle;      // Visual representation of the process
    [SerializeField]
    Camera Cam;             // Camera

    [SerializeField]
    Transform Camera_Position;
    
    [SerializeField]
    int Max_Lenght;

    public List<GameObject> Sections = new List<GameObject>();
    public int Active_Road = 0;      

    public int Current_Lenght = 1;
    GameObject Current_Section; // The prefab that is going to be created

    GameObject[] GO;        // The active road section in the game world

    [SerializeField]
    bool Step_By_Step;      // The way update goes, manual or automatic

    [SerializeField]
    GameObject Building_Prefab;

    [SerializeField]
    public Transform Hitpoint;

    public bool Ready;

    [SerializeField]
    GameObject Select_Terrain_UI;

    [SerializeField]
    GameObject Terrain_Preview;
    [SerializeField]
    GameObject Actual_Terrain;

    [SerializeField]
    Slider Road_Sections_Slider;

    float Start_Button_Timer;

    Connectors Con;

    // Start is called before the first frame update
    void Start()
    {
        Ready = false;
        Select_Terrain_UI.SetActive(true);
        Terrain_Preview.SetActive(true);
        Actual_Terrain.SetActive(false);
        Start_Button_Timer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Road_Type_Game.Length == 0 && Ready)
        {
            Start_Button_Timer += Time.deltaTime;
            if (Input.GetMouseButtonDown(0) && Start_Button_Timer >= 1)
            {
                int layer_mask = LayerMask.GetMask("Terrain");

                RaycastHit hit;
                Ray ray = Cam.ScreenPointToRay(Input.mousePosition);
                // Does the ray intersect any objects excluding the player layer
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layer_mask))
                {
                    Hitpoint.position = hit.point;
                }


                Camera_Position.position = new Vector3(Hitpoint.position.x, 500f, Hitpoint.position.z - 300);
                Cam.transform.localEulerAngles = new Vector3(45, 0, 0);

                Max_Lenght = (int)Road_Sections_Slider.value;

                Road_Type_Game = new GameObject[4];
                Road_Type = 0;
                Current_Section = Roads[Road_Type];
                Sections.Add(Current_Section);
                GO = new GameObject[Max_Lenght];
                GO[0] = Instantiate(Current_Section, Hitpoint.position + new Vector3(0, 1f, 0), Quaternion.identity);
                Road_Type_Game[0] = GO[0];
                Turtle.transform.position = Current_Section.transform.position;
                Turtle.transform.rotation = Current_Section.transform.rotation;
                Get_Connectors(GO[Active_Road]);
                Active_Road++;
            }
        }

        if (Road_Type_Game.Length != 0)
        {
            if (Step_By_Step)       // Manual process
            {
                if (Input.GetKeyDown(KeyCode.F) && Current_Lenght < Max_Lenght)
                {
                    Get_Connectors(GO[Active_Road]);

                    if (GO[Active_Road].name != Road_Type_Game[0].name)
                    {
                        Buildings(GO[Active_Road]);
                    }

                    Active_Road++;
                }
            }
            else              // Automatic process
            {
                if (Current_Lenght < Max_Lenght)
                {
                    if (GO[Active_Road] == null)
                    {
                        //Active_Road++;
                        Current_Lenght = Max_Lenght;
                        return;
                    }

                    Get_Connectors(GO[Active_Road]);

                    if (GO[Active_Road].name != Road_Type_Game[0].name)
                    {
                        Buildings(GO[Active_Road]);
                    }

                    Active_Road++;
                }
            }
        }
    }

    void Get_Connectors(GameObject Sect)
    {
        if (Sect.GetComponent<Connectors>() != null)
        {
            Con = Sect.GetComponent<Connectors>();
            if (!Con.Ready)
            {
                Active_Road--;
            }
            if (Con.Ready && Con.Connector.Count >= 1)
            {
                foreach (GameObject a in Con.Connector)  // Goes through all the connectors of the road section
                {
                    if (Current_Lenght < Max_Lenght)
                    {
                        Turtle.transform.position = a.transform.position;
                        Turtle.transform.rotation = a.transform.rotation;
                        Extend_Road();
                    }
                }
            }
            Road_Type = 0;
        }
    }

    void Buildings(GameObject Sect)
    {
        Spawn_Buildings Buildings = Sect.GetComponent<Spawn_Buildings>();
        
        if(Buildings.Spawning_Points.Count != 0)
        {
            foreach(GameObject a in Buildings.Spawning_Points)
            {
                GameObject GO = Instantiate(Building_Prefab, a.transform.position - new Vector3(0, 1f, 0), Quaternion.identity);
                Renderer renderer = GO.GetComponent<Renderer>();
                renderer.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            }
        }
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
                Change_Height Height_Detector = GO[Active_Road].GetComponent<Change_Height>();
                if (Height_Detector.Is_On_Water)
                {
                    New_Road_Type = 1;
                    Spawn_Buildings Buildings = GO[Active_Road].GetComponent<Spawn_Buildings>();
                    Buildings.Spawning_Points.Clear();
                }
                else
                {
                    New_Road_Type = Random.Range(0, 15);
                    if (New_Road_Type >= 4)
                    {
                        New_Road_Type = 1;
                    }
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
            if (Road_Type_Game[Road_Type] == null)
            {
                New_Road_Type = 1;
                Current_Section = Roads[New_Road_Type];
                Build_Section(Current_Section);
            }
            else
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
            GO[Current_Lenght] = Instantiate(Section, Turtle.transform.position, Quaternion.Euler(0, (Turtle.transform.eulerAngles.y + 270f), 0));
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
        //Sections[Current_Lenght] = Section;
        Sections.Add(Section);
        Current_Lenght++;
    }

    public void Is_Ready()
    {
        Select_Terrain_UI.SetActive(false);
        Terrain_Preview.SetActive(false);
        Actual_Terrain.SetActive(true);
        Ready = true;
    }
}