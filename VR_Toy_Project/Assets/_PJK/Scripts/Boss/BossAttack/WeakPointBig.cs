using System;
using System.Collections;
using UnityEngine;

public class WeakPointBig : MonoBehaviour, IDamageable
{
    public Boss boss;
    private Renderer myMat;
    public Material weakOn;
    public Material weakOut;
    public bool isLive = true;
   

    public void Start()
    {
        myMat = GetComponent<Renderer>();
    }


    private void Update()
    {
        if (isLive == true)
        {
            myMat.material = weakOn;
            transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        }
        else 
        {
            myMat.material = weakOut;
            transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        }
    }

    public void OnDamage(int damage)
    {
        boss.OnDamage(damage);

        if (isLive ==true)
        {

            StartCoroutine(ActiveWeakpoint());
        }

    }
   
    IEnumerator ActiveWeakpoint()
    {
        isLive = false;
        int reviveTime=UnityEngine.Random.Range(3, 7);

        yield return new WaitForSeconds(reviveTime);
        isLive = true;
        
    }
}
