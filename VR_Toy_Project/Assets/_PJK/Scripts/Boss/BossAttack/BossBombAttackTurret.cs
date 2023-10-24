using System.Collections;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class BossBombAttackTurret : MonoBehaviour, IDamageable
{
    // 공격 포탄 Hp
    public int BossBombAttackHp = default;
    public int BossBombAttackDmg = default;

    public float initialAngle = 30f;    // 처음 날라가는 각도
    public Vector3 targetPos;           // 저장될 타겟 포지션
    // public GameObject target;        // 타겟 : 타겟을 찾기 않고 포지션을 저장해 두기로 하였음. BSJ_231023
    private float Shottime;
    private Rigidbody rb;               // Rigidbody
    private int randomX;

    private Turret01[] turret01;
    private Turret02[] turret02;
    private BossManager bm;
    private float nearDistance;
    private GameObject tempTarget;
    private void Awake()
    {
        // 초기 체력 셋팅
        BossBombAttackHp = JsonData.Instance.bossSkillDatas.Boss_Skill[1].Hp;
        // 초기 데미지 셋팅
        BossBombAttackDmg = JsonData.Instance.bossSkillDatas.Boss_Skill[1].Att;

        rb = GetComponent<Rigidbody>();
        Shottime = 0;
        nearDistance = Mathf.Infinity;

        turret01 = FindObjectsOfType<Turret01>();
        turret02 = FindObjectsOfType<Turret02>();

        if (turret01.Length > 0)
        {
            // 제일 가까운 터렛 찾기
            for (int i = 0; i < turret01.Length; i++)
            {
                float findDistance = Vector3.Distance(transform.position, turret01[i].transform.position);

                // 제일 가깝다면 그 타겟을 저장하고,
                if (nearDistance > findDistance)
                {
                    nearDistance = findDistance;
                    targetPos = turret01[i].gameObject.transform.position;
                }
            }
        }

        if (turret02.Length > 0)
        {
            for (int i = 0; i < turret02.Length ; i++)
            {
                float findDistance = Vector3.Distance(transform.position, turret02[i].transform.position);

                // 제일 가깝다면 그 타겟을 저장하고,
                if (nearDistance > findDistance)
                {
                    nearDistance = findDistance;
                    targetPos = turret02[i].gameObject.transform.position;
                }
            }
        }
    }


    private void Start()
    {
        StartCoroutine(Firsttime());
    }

    private void Update()
    {
        // 체력이 0이되면 비활성화
        if (BossBombAttackHp <= 0 )
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 터렛과 충돌 시 데미지 처리
        if(other.CompareTag("Turret"))
        {
            other.GetComponent<TurretUnit>().DamageSelf(BossBombAttackDmg);
        }
    }


    IEnumerator Firsttime()
    {
        randomX = Random.Range(-10, 10);
        rb.useGravity = false;
        Vector3 velocity = new Vector3(randomX, 5, 0);
        rb.velocity = velocity;

        yield return new WaitForSeconds(1.5f);
        rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(1.5f);

        rb.useGravity = true;
        // 포물선 운동
        velocity = GetVelocity(transform.position, targetPos, initialAngle);

        rb.velocity = velocity;
    }


    public Vector3 GetVelocity(Vector3 startPos, Vector3 target, float initialAngle)
    {
        float gravity = Physics.gravity.magnitude;
        float angle = initialAngle * Mathf.Deg2Rad;

        Vector3 targetPos = new Vector3(target.x, 0, target.z); // y 좌표를 0으로 설정하여 y 값을 무시
        Vector3 shotPos = new Vector3(startPos.x, 0, startPos.z); // y 좌표를 0으로 설정하여 y 값을 무시

        float distance = Vector3.Distance(targetPos, shotPos);
        float yOffset = startPos.y - target.y; // yOffset을 0으로 설정하여 높이를 고려하지 않음


        if (distance <= 0 || yOffset <= 0)
        {
            return Vector3.zero;
        }

        float initialVelocity
           = (1 / Mathf.Cos(angle)) * Mathf.Sqrt((0.5f * gravity * Mathf.Pow(distance, 2)) / (distance * Mathf.Tan(angle) + yOffset));

        Vector3 velocity
             = new Vector3(0f, initialVelocity * Mathf.Sin(angle), initialVelocity * Mathf.Cos(angle));

        float angleBetweenObjects
            = Vector3.Angle(Vector3.forward, targetPos - shotPos) * (target.x > startPos.x ? 1 : -1);
        Vector3 finalVelocity
            = Quaternion.AngleAxis(angleBetweenObjects, Vector3.up) * velocity;

        return finalVelocity;
    }

    public void OnDamage(int damage)
    {
        BossBombAttackHp -= damage;
    }
}