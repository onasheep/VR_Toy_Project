using System;
using System.Collections;
using UnityEngine;

public class WeakPointSmall : MonoBehaviour, IDamageable
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
        }
        else
        {
            myMat.material = weakOut;
        }
    }

    public void OnDamage(int damage)
    {
            boss.OnDamage(damage);

        if (isLive == true)
        {
            StartCoroutine(ActiveWeakpoint());
        }
    }

    IEnumerator ActiveWeakpoint()
    {
        isLive = false;
        int reviveTime = UnityEngine.Random.Range(1, 5);

        yield return new WaitForSeconds(reviveTime);
        isLive = true;

    }
}
