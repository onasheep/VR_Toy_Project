using System.Collections;
using UnityEngine;
public class BossBombAttackPlayer : MonoBehaviour, IDamageable
{
    // 공격 포탄 Hp
    public int BossBombAttackPlayerHp = default;
    public int BossBombAttackPlayerAtt = default;
    public float initialAngle = 30f;    // 처음 날라가는 각도
    private Rigidbody rb;               // Rigidbody
    private int randomX;
    public float detectionRadius = 1000f;
    private GameObject target;
    private BossManager bm = default;
    private PlayerStatus ps = default;
    public GameObject diedPrefab = default;
    public bool isAttackedBullet { get; private set; } = false;

    private void Awake()
    {
        bm = GameObject.Find("BossManager").GetComponent<BossManager>();

        // 체력 셋팅
        BossBombAttackPlayerHp = JsonData.Instance.bossSkillDatas.Boss_Skill[0].Hp;
        BossBombAttackPlayerAtt = JsonData.Instance.bossSkillDatas.Boss_Skill[0].Att;
        rb = GetComponent<Rigidbody>();
        target = GameObject.Find("Player");
    }


    public void AttackedByBullet()
    {
        isAttackedBullet = true;
    }




    private void Start()
    {
        StartCoroutine(Firsttime());
    }

    private void Update()
    {
        // 체력이 0이되면 비활성화
        if (BossBombAttackPlayerHp <= 0)
        {
            Destroy(gameObject);
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // HSJ_ 231023
            // 중복 트리거 체크 방지 위해서 최초 접촉시 Collider 꺼줌
            // 추후 오브젝트 풀로 사용시 꺼내는 시점에서 켜주고, 끄는 시점에서 꺼주면 될듯함
            this.GetComponent<SphereCollider>().enabled = false;
            ps = other.GetComponent<PlayerStatus>();

            //PlayerStatus의 OnDamage
            
            isAttackedBullet = false;
            ps.OnDamage(BossBombAttackPlayerAtt);
            Destroy(gameObject);

        }
    }
    public void OnDamage(int damage)
    {
        BossBombAttackPlayerHp -= damage;
    }

    private void OnDestroy()
    {
        //if (isAttackedBullet == true)
        //{
        //    GameObject DieMotion = Instantiate(diedPrefab, transform.position, Quaternion.identity);
        //}
        //else
        //{
        //    GameObject AttakMotion = Instantiate(diedPrefab, transform.position, Quaternion.identity);
        //}
    }



    IEnumerator Firsttime()
    {
        randomX = Random.Range(-10, 10);
        rb.useGravity = false;
        Vector3 velocity = new Vector3(randomX, 10, 0);
        rb.velocity = velocity;

        yield return new WaitForSeconds(1.5f);
        rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(1.5f);

        rb.useGravity = true;
        // 포물선 운동
        velocity = GetVelocity(transform.position, target.transform.position, initialAngle);

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





}