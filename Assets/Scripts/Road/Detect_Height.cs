using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Detect_Height : MonoBehaviour
{
    [SerializeField]
    public Transform Hitpoint;

    private void FixedUpdate()
    {
        int layer_mask = LayerMask.GetMask("Terrain");

        RaycastHit hit;
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(new Vector3(transform.position.x, 200, transform.position.z), transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, layer_mask))
        {
            Hitpoint.position = hit.point;
        }
    }
}
