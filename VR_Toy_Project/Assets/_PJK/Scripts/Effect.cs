using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect : MonoBehaviour
{

    private void Awake()
    {
        Destroy(gameObject,5f);
    }
}
