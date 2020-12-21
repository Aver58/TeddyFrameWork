using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSetActive : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < 1000; i++)
        {
            gameObject.SetActive(false);
            gameObject.SetActive(true);
            // MaskableGraphic.OnEnable 有gc
        }
    }
}
