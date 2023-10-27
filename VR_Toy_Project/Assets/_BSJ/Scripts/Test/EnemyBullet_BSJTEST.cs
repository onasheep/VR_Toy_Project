using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet_BSJTEST : MonoBehaviour
{
    PlayerStatus p_Stat;

    private void Update()
    {
        // 총알이 계속 앞으로 날아감.
        transform.Translate(Vector3.back * 5f * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            p_Stat = other.GetComponent<PlayerStatus>();
            p_Stat.HitDamage_Player();
            gameObject.SetActive(false);
        }
    }
}
