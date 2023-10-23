using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectedCollider : MonoBehaviour
{
    private Monsters monsters;

    // 찾은 대상이 있는지 판별
    private bool isFindTarget = false;

    private void Awake()
    {
        monsters = GetComponentInParent<Monsters>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 이미 찾은 대상이 있다면 리턴한다.
        if (isFindTarget == true) { return; }

        // 터렛과 충돌하면 몬스터의 타겟을 터렛으로 변경한다.
        if (other.CompareTag("Turret"))
        {
            monsters.turretUnit = other.GetComponent<TurretUnit>();
            monsters.target = other.gameObject;
            monsters.isFindTurret = true;
            isFindTarget = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 터렛과의 충돌에서 벗어나면,
        if(other.CompareTag("Turret"))
        {
            // 플레이어를 타겟으로 한다.
            monsters.turretUnit = null;
            monsters.target = GameObject.Find("Player");
            monsters.isFindTurret = false;
            monsters.isAttackTurret = false;
            isFindTarget = false;
        }
    }
}


