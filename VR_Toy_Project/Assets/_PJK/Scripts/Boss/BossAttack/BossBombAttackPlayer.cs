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
    public GameObject attakPrefab = default;
    public GameObject effect = default;
    private Boss boss = default;

    // HSJ_ 231024
    private SphereCollider sphereCollider = default;
    //BSJ_
    private Vector3 yOffset = new(0f, 2f, 0f);

    private void Awake()
    {
        boss = FindObjectOfType<Boss>();
        bm = GameObject.Find("BossManager").GetComponent<BossManager>();
        // HSJ_
        sphereCollider = GetComponent<SphereCollider>();

        // 체력 셋팅
        BossBombAttackPlayerHp = JsonData.Instance.bossSkillDatas.Boss_Skill[0].Hp;
        BossBombAttackPlayerAtt = JsonData.Instance.bossSkillDatas.Boss_Skill[0].Att;
        rb = GetComponent<Rigidbody>();
        target = GameObject.Find("Player");
    }

    // BSJ_ 오브젝트 풀에서 호출되서 활성화 되면 코루틴 시작
    private void OnEnable()
    {
        // 체력 재 셋팅
        BossBombAttackPlayerHp = JsonData.Instance.bossSkillDatas.Boss_Skill[0].Hp;
        StartCoroutine(Firsttime());
    }

    private void Update()
    {
        if (BossBombAttackPlayerHp <= 0)
        {
            // 파괴 이펙트 오브젝트 풀에서 생성
            GameObject DieMotion = VFXObjectPool.instance.GetPoolObj(VFXPoolObjType.BossAttackdiedVFX);

            DieMotion.SetActive(true);
            DieMotion.transform.position = transform.position;

            // BSJ_오브젝트 풀로 반환
            BossAttackObjectPool.instance.CoolObj(gameObject, BossAttackPoolObjType.BossAttackPlayer);



            //LEGACY : 오브젝트 풀로 반환
            //GameObject DieMotion = Instantiate(diedPrefab, transform.position, Quaternion.identity);
            //Destroy(gameObject);
        }
        if (boss.CurHP < 0 && gameObject.activeSelf)
        {
            GameObject DieMotion = VFXObjectPool.instance.GetPoolObj(VFXPoolObjType.BossAttackdiedVFX);
            DieMotion.SetActive(true);
            DieMotion.transform.position = transform.position;

            // BSJ_오브젝트 풀로 반환
            BossAttackObjectPool.instance.CoolObj(gameObject, BossAttackPoolObjType.BossAttackPlayer);

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sphereCollider.enabled = false;

            ps = other.GetComponent<PlayerStatus>();

            ps.OnDamage(BossBombAttackPlayerAtt);

            //LEGACY:오브젝트 풀에서 생성하기 위해 변경
            //GameObject AttackMotion = Instantiate(attakPrefab, new Vector3(transform.position.x, transform.position.y-2f, transform.position.z), Quaternion.identity);

            // 플레이어 공격 이펙트 오브젝트 풀에서 생성
            GameObject AttackMotion = VFXObjectPool.instance.GetPoolObj(VFXPoolObjType.BossAttackPlayerVFX);
            AttackMotion.SetActive(true);
            AttackMotion.transform.position = other.transform.position;

            // BSJ_오브젝트 풀로 반환
            BossAttackObjectPool.instance.CoolObj(gameObject, BossAttackPoolObjType.BossAttackPlayer);
            // HSJ
            sphereCollider.enabled = true;
        }
    }
    public void OnDamage(int damage)
    {
        BossBombAttackPlayerHp -= damage;
    }

    IEnumerator Firsttime()
    {
        randomX = Random.Range(-10, 10);
        rb.useGravity = false;
        Vector3 velocity = new Vector3(randomX, 5, 0);
        rb.velocity = velocity;

        yield return new WaitForSeconds(1.5f);

        rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(3f);
        StartCoroutine(AttackEffect());

        rb.useGravity = true;
        //포물선 운동
        velocity = GetVelocity(transform.position, target.transform.position, initialAngle);

        rb.velocity = velocity;


    }

    IEnumerator AttackEffect()
    {
        GameObject attackeffect = Instantiate(effect, transform.position, Quaternion.identity);



        yield return new WaitForSeconds(3f);

        Destroy(attackeffect);
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