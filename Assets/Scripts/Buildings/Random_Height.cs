using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Random_Height : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int r = Random.Range(15, 40);

        transform.localScale = new Vector3(transform.localScale.x, r, transform.localScale.z);
    }
}
