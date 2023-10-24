using UnityEngine;
using System.Collections;
using UnityEngine.TextCore.Text;
using System.Threading;

public class Monsters : MonoBehaviour, IDamageable
{
    //// HSJ_ 231019
    //// { 기획 Scale 변경용 변수
    //[Header("졸개 Scale 변경용 변수")]
    //[Space]
    //[Range(10f, 30f)]
    //public float scaleX= 10f;
    //[Range(10f, 30f)]
    //public float scaleY = 10f;    
    //[Range(10f, 30f)]
    //public float scaleZ = 10f;
    //// } 기획 Scale 변경용 변수

    public MonsterPoolObjType monsterType;

    public bool isTest = false;

    //애니메이터 관련
    private Animator anim;

    //public bool isNoTurret = false;
    //public bool isAttackturret = false;
    //public bool isDied = false;

    // 몬스터
    public GameObject monsterLevel = default;

    // 공격 대상으로 삼을 타겟
    public GameObject target = default;

    // 사정거리
    public float attackdistance = 5f;

    // 터렛과의 거리
    public float distanceToTurret = default;

    // 감지 콜라이더
    public Collider detectCollider = default;

    // 터렛 레이어
    public LayerMask turretLayer;

    // 불러올 터렛 스크립트
    public TurretUnit turretUnit = default;

    // 터렛 탐지 거리
    public float detectionRadius = 10f;

    // 터렛을 타겟중인지 체크
    public bool isFindTurret = false;

    // 터렛을 공격중인지 체크
    public bool isAttackTurret = false;

    // 이동 속도
    [SerializeField]
    private float moveSpeed = 3f;

    // 몬스터의 공격속도
    [SerializeField]
    private float AttackSpeed = default;

    // 몬스터의 체력
    [SerializeField]
    private float hp;

    // 몬스터의 데미지
    [SerializeField]
    private int dmg;

    // 몬스터 자폭 데미지
    [SerializeField]
    private float bombdmg;

    // 컴포넌트들
    private Rigidbody rb; // 괴수의 Rigidbody를 사용하여 이동 처리
    private BoxCollider boxCollider;

    // 공격 타이머
    private float attackTimer = 0f;

    public int Lv1hp { get; private set; }
    public int Lv1atk { get; private set; }
    public float Lv1atkspeed { get; private set; }
    public int Lv1BombDmg { get; private set; }
    public int Lv2hp { get; private set; }
    public int Lv2atk { get; private set; }
    public float Lv2atkspeed { get; private set; }
    public int Lv2BombDmg { get; private set; }
    public int Lv3hp { get; private set; }

    public int Lv3atk { get; private set; }
    public float Lv3atkspeed { get; private set; }
    public int Lv3BombDmg { get; private set; }

    // TEST 
    // HSJ_ 2310119
    private SkinnedMeshRenderer meshRender;

    private void OnEnable()
    {
        // 오브젝트 풀에서 반환될때 초기값 재셋팅
        SetMonsterStat();
    }

    void Start()
    {
        // tu = GetComponent<TurretUnit>();

        // 초기 몬스터 셋팅 하는 함수
        SetMonsterStat();

        rb = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        anim = GetComponent<Animator>();

        // TEST : 
        // HSJ_ 2310119
        meshRender = this.gameObject.GetChildObj("AnkleBiter").GetComponent<SkinnedMeshRenderer>();
    }

    private void Update()
    {
        //// TEST : 
        //// TODO : 기획 Sclae 확정 후 삭제 예정
        //// HSJ_ 231019
        //TestChangeScale();

        // 현재 맵에 터렛이 있는지 체크
        //if(GameManager.Instance.turretLv1_List.Count + GameManager.Instance.turretLv2_List.Count + GameManager.Instance.turretLv3_List.Count + GameManager.Instance.turretLv4_List.Count  > 0)
        //{
        //    isFindTurret = true;
        //}
        //else
        //{
        //    isFindTurret = false;
        //}

        if(target == null)
        { target = GameObject.Find("Player"); }

        // 터렛을 추격중이 아니면,
        if (isFindTurret == false)
        {
            // 플레이어 추격
            MoveTowardsTarget(target.transform.position);

            // 공격 범위 내에 왔다면 플레이어 공격
            if (Vector3.Distance(gameObject.transform.position, target.transform.position) <= attackdistance)
            {
                // 자폭
                hp = 0;
                Died();
            }
        }

        // 터렛을 콜라이더에서 발견하면
        else if (isFindTurret == true)
        {    
            //터렛을 공격중이 아니라면
            if (isAttackTurret == false)
            {
                // 터렛 추격
                Vector3 targetPosition = target.transform.position;

                // 현재 위치에서 타겟 방향과 거리 계산
                Vector3 toTarget = targetPosition - transform.position;

                if (toTarget.magnitude > attackdistance)
                {
                    // 이동
                    MoveTowardsTarget(targetPosition);
                }

                // 터렛과의 거리 비교
                distanceToTurret = Vector3.Distance(transform.position, target.transform.position);

                // 터렛이 사정거리 안에 들어왔다면,
                if (distanceToTurret <= attackdistance)
                {
                    // 터렛 공격
                    isAttackTurret = true;
                }

            }

            // 터렛을 공격 중이라면
            else if (isAttackTurret == true)
            {
                // 공격 타이머
                attackTimer += Time.deltaTime;
                
                // 공격 속도에 타이머가 도달하였다면.
                if(attackTimer > AttackSpeed)
                {
                    // 터렛 공격
                    rb.velocity = Vector3.zero;
                    AttackTurret(dmg);
                    attackTimer = 0f;
                }
            }
        }
    }

    public void OnDisable()
    {
        // 바로 터지는 것 방지
        hp = 1f;
    }

    //! 몬스터가 데미지를 받는 함수
    public void OnDamage(int damage)
    {
        // 체력을 데미지만큼 뺀다.
        hp -= damage;

        // 체력이 0 초과일 땐, 
        if (hp > 0)
        {
            // 피격 시 색상 변경을 한다.
            StartCoroutine(ChangeColor());
        }

        // 체력이 0 이하이면,
        if (hp <= 0)
        {
            // 사망한다.
            Died();
        }

    }

    //! 목표 지점으로 괴수를 이동시키는 함수
    private void MoveTowardsTarget(Vector3 targetPosition)
    {
        gameObject.transform.forward = (targetPosition - gameObject.transform.position).normalized;

        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        //rb.velocity = moveDirection * moveSpeed;
        rb.velocity = new Vector3(moveDirection.x, 0f, moveDirection.z) * moveSpeed;
    }


    //! 터렛을 공격하는 함수
    private void AttackTurret(int damage)
    {
        if (turretUnit == null) { return; }

        Debug.Log("공격함");
        rb.velocity = Vector3.zero;
        anim.SetBool("isAttackturret", true);
        turretUnit.DamageSelf(damage);
        // 멈춤
    }

    //! 플레이어를 공격하는 함수
    private void AttackUser(float damage)
    {
        rb.velocity = Vector3.zero;
        Bomb(damage);
    }

    //! 몬스터가 자폭 공격을 하는 함수
    private void Bomb(float damage)
    {
        // TODO : Effect 효과 넣어서 실행시켜야함
        //MonsterBomb.instance.PlayEffect();

        Died();
    }

    //! 몬스터가 사망하는 함수
    private void Died()
    {
        if (hp <= 0)
        {
            // TEST : 
            // HSJ_ 231019
            GameManager.Instance.GetGold_Monster();

            // 상태 초기화
            turretUnit = null;
            target = GameObject.Find("Player");
            isFindTurret = false;
            isAttackTurret = false;

            // 몬스터 사망 시 이펙트 생성. BSJ_231020
            GameObject deathEffect = VFXObjectPool.instance.GetPoolObj(VFXPoolObjType.MonsterDeathVFX);
            deathEffect.SetActive(true);
            deathEffect.transform.position = transform.position;

            // 폭발 트리거 생성
            GameObject deathBomb = MonsterObjectPool.instance.GetPoolObj(MonsterPoolObjType.DeathBomb);
            deathBomb.SetActive(true);
            deathBomb.transform.position = transform.position;
            deathBomb.GetComponent<MonsterDeathBomb>()._Damage = bombdmg * 100f;    // 100f는 테스트 용 데미지 업 _BSJ

            // 색상 원래대로 변경
            meshRender.material.color = Color.white;

            // 몬스터 사망
            MonsterObjectPool.instance.CoolObj(gameObject, monsterType);

            // LEGACY: 오브젝트 풀로 반환하기로 하였음. BSJ_231023
            // Destroy(gameObject);
            // LEGACY: 죽으면 이펙트 생성 후 바로 Destroy 처리하기로 했음. BSJ_231020
            // StartCoroutine(inActive());
        }
    }

    // 피격시 색상 변경하는 코루틴
    IEnumerator ChangeColor()
    {
        meshRender.material.color = Color.red;

        yield return new WaitForSeconds(0.2f);

        meshRender.material.color = Color.white;
    }


    //! 몬스터 셋팅하는 함수
    public void SetMonsterStat()
    {
        // 공격 타이머
        attackTimer = 0f;

        // 처음 타겟은 플레이어
        target = GameObject.Find("Player");

        // { 몬스터 초기값 셋팅
        Lv1hp = JsonData.Instance.monsterDatas.Monster[0].HP;
        Lv1atk = JsonData.Instance.monsterDatas.Monster[0].Att;
        Lv1BombDmg = JsonData.Instance.monsterDatas.Monster[0].Explosion_Damage;
        Lv1atkspeed = JsonData.Instance.monsterDatas.Monster[0].Att_Speed;

        Lv2hp = JsonData.Instance.monsterDatas.Monster[1].HP;
        Lv2atk = JsonData.Instance.monsterDatas.Monster[1].Att;
        Lv2BombDmg = JsonData.Instance.monsterDatas.Monster[1].Explosion_Damage;
        Lv2atkspeed = JsonData.Instance.monsterDatas.Monster[1].Att_Speed;

        Lv3hp = JsonData.Instance.monsterDatas.Monster[2].HP;
        Lv3atk = JsonData.Instance.monsterDatas.Monster[2].Att;
        Lv3BombDmg = JsonData.Instance.monsterDatas.Monster[2].Explosion_Damage;
        Lv3atkspeed = JsonData.Instance.monsterDatas.Monster[2].Att_Speed;

        if (BossManager.instance.gametime < 300f)
        {
            hp = Lv1hp;
            dmg = Lv1atk;
            bombdmg = Lv1BombDmg;
            AttackSpeed = Lv1atkspeed;
        }
        else if (BossManager.instance.gametime > 300f && BossManager.instance.gametime < 600f)
        {
            hp = Lv2hp;
            dmg = Lv2atk;
            bombdmg = Lv2BombDmg;
            AttackSpeed = Lv2atkspeed;
        }
        else if (BossManager.instance.gametime > 600f)
        {
            hp = Lv3hp;
            dmg = Lv3atk;
            bombdmg = Lv3BombDmg;
            AttackSpeed = Lv3atkspeed;
        }
        // } 몬스터 초기값 셋팅
    }

    // LEGACY: 자식 오브젝트인 감지 콜라이더에서 타겟을 찾음.
    // 타겟을 찾는다.
    //private void FindTarget()
    //{
    //    Collider[] colliders = Physics.OverlapSphere(transform.position, detectionRadius, turretLayer);

    //    if (colliders.Length > 0)
    //    {
    //        // 가장 가까운 터렛을 타겟으로 설정
    //        float closestDistance = float.MaxValue;

    //        foreach (Collider collider in colliders)
    //        {
    //            float distance = Vector3.Distance(transform.position, collider.transform.position);
    //            if (distance < closestDistance)
    //            {
    //                closestDistance = distance;
    //                target = collider.transform;
    //            }
    //        }
    //    }
    //    else
    //    {
    //        target = null;
    //    }
    //}

    // LEGACY: 죽으면 바로 사라지게 하기로 하였음. BSJ_231020
    //IEnumerator inActive()
    //{
    //    yield return new WaitForSeconds(1.5f);
    //    Destroy(gameObject);
    //}

    //// TEST : 기획 분들 Scale 변경 테스트 하기 위한 함수
    //// HSJ_ 231019
    //public void TestChangeScale()
    //{
    //    // TEST : 스케일 변경 위하여 
    //    if(Input.GetMouseButtonDown(1))
    //    {
    //        isTest = true;
    //        moveSpeed = 0f;
    //    }
    //    this.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);

    //}       // TestChangeScale()
}


