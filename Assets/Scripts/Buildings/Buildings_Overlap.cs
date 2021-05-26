using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buildings_Overlap : MonoBehaviour
{
    float Activ_Trigger = 0;
    BoxCollider collider;

    bool Active = true;
    
    private void Start()
    {
        collider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        Activ_Trigger += Time.deltaTime;

        if (Activ_Trigger >= 1f)
        {
            collider.isTrigger = false; // Deactivates the trigger so when a new connector will be spawend in the same place, it will have contact
            Active = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Building_Connection")
        {
            if (Active == false)
            {
                Spawn_Buildings con = this.GetComponentInParent<Spawn_Buildings>();
                con.Spawning_Points.Remove(this.gameObject);  // Removes the connector from the list
            }
        }
        if (other.tag == "Building")
        {
            Spawn_Buildings con = this.GetComponentInParent<Spawn_Buildings>();
            con.Spawning_Points.Remove(this.gameObject);  // Removes the connector from the list
        }
        if (other.tag == "Road")
        {
            Spawn_Buildings con = this.GetComponentInParent<Spawn_Buildings>();
            con.Spawning_Points.Remove(this.gameObject);  // Removes the connector from the list
        }
    }
}
