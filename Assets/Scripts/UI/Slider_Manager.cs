using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Slider_Manager : MonoBehaviour
{
    [SerializeField]
    Text text;

    [SerializeField]
    Slider slider;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        text.text = slider.value.ToString();
    }
}
