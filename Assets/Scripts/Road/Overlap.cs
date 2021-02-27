using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overlap : MonoBehaviour
{
    float Activ_Trigger = 0;
    BoxCollider collider;

    [SerializeField]
    Material[] materials;

    bool Active = true;

    private void Start()
    {
        collider = GetComponent<BoxCollider>();
    }

    private void Update()
    {
        Activ_Trigger += Time.deltaTime;

        if(Activ_Trigger>=1f)
        {
            collider.isTrigger = false; // Deactivates the trigger so when a new connector will be spawend in the same place, it will have contact
            Active = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Road")
        {
            Connectors con = this.GetComponentInParent<Connectors>();
            con.Connector.Remove(this.gameObject);  // Removes the connector from the list
            if (Active == false)
            {
                MeshRenderer mesh = other.GetComponent<MeshRenderer>();
                mesh.material = materials[0];
                Debug.Log("Yep");
            }
        }
        if (other.tag == "Connector")
        {
            if (Active == false)
            {
                Connectors con = this.GetComponentInParent<Connectors>();
                con.Connector.Remove(this.gameObject);  // Removes the connector from the list
            }
        }
    }
}