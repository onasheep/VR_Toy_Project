using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    private SphereCollider blackHoleCollider;


    void Start()
    {
        StartCoroutine(DestroyToDie());
        blackHoleCollider = GetComponent<SphereCollider>();
    }

    private void Update()
    {
          
    }

    IEnumerator DestroyToDie()
    {

        yield return new WaitForSeconds(5f);
        if (transform.localScale.x > 0 || transform.localScale.y > 0 || transform.localScale.z > 0)
        {
            while (transform.localScale.x > 0 || transform.localScale.y > 0 || transform.localScale.z > 0)
            {

                
                gameObject.transform.localScale -= new Vector3(0.01f, 0.01f, 0.01f);
                yield return new WaitForSeconds(0.01f);
            }

        }
        if (transform.localScale.x > -10 || transform.localScale.y > -10 || transform.localScale.z > -10)
        {

            while (transform.localScale.x > -10 || transform.localScale.y > -10 || transform.localScale.z > -10)
            {
                blackHoleCollider.radius += 0.2f;
                gameObject.transform.localScale -= new Vector3(0.2f, 0.2f, 0.2f);

                yield return new WaitForSeconds(0.01f);
            }
        }
        Destroy(gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        //오브젝트 풀링 
        if (other.CompareTag("Monster"))
        {
            GameObject monster = other.GetComponent<GameObject>();
            Destroy(other.gameObject);
        }

        if (other.CompareTag("BossAttackPlayer"))
        {
            GameObject BossAttackPlayer = other.GetComponent<GameObject>();
            Destroy(other.gameObject);
        }

        if (other.CompareTag("BossAttackSpawnMon"))
        {
            GameObject BossAttackSpawnMon = other.GetComponent<GameObject>();
            Destroy(other.gameObject);
        }

        if (other.CompareTag("BossAttackTurret"))
        {
            GameObject BossAttackTurret = other.GetComponent<GameObject>();
            Destroy(other.gameObject);
        }
    }

}