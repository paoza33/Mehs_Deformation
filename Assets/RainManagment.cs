using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainManagment : MonoBehaviour
{
    public GameObject[] raindrops;
    private int i = 0;

    IEnumerator WaitRaindrop()
    {
        raindrops[i].GetComponent<Rigidbody>().useGravity = true;
        yield return new WaitForSeconds(0.05f);
        i++;
        if(!(i >= raindrops.Length))
        {
            StartCoroutine(WaitRaindrop());
        }
    }

    private void Start()
    {
        StartCoroutine(WaitRaindrop());
    }
}
