using UnityEngine;

public class MonsterMoving : MonoBehaviour
{
    public float speed = 20f; // 몬스터 이동 속도
    public float detectionRadius = 10f; // 타겟을 감지할 범위
    public LayerMask turretLayer; // 포탑 레이어
    public LayerMask playerLayer; // 플레이어 레이어
    public Boss boss = default;

    private Transform target; // 현재 목표 위치

    private void Start()
    {
        boss = GameObject.Find("Boss").GetComponent<Boss>();
    }

    void Update()
    {
        if (boss.CurHP > 0)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius);
            Transform nearestTurret = null;

            foreach (var collider in colliders)
            {
                if (collider.gameObject.layer == turretLayer)
                {
                    if (nearestTurret == null || Vector3.Distance(transform.position, collider.transform.position) < Vector3.Distance(transform.position, nearestTurret.position))
                    {
                        nearestTurret = collider.transform;
                    }
                }
            }

            if (nearestTurret != null)
            {
                // 가장 가까운 포탑이 감지됨 - 포탑 방향으로 이동
                target = nearestTurret;
            }
            else
            {
                // 포탑이 없을 때 - 플레이어 방향으로 이동
                Collider[] playerColliders = Physics.OverlapSphere(transform.position, detectionRadius, playerLayer);
                if (playerColliders.Length > 0)
                {
                    target = playerColliders[0].transform;
                }
                else
                {

                }
            }

            if (target != null)
            {
                // 목표 방향으로 이동
                Vector3 moveDirection = (target.position - transform.position).normalized;
                transform.Translate(moveDirection * speed * Time.deltaTime);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
