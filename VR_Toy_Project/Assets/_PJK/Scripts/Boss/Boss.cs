using System.Collections;
using UnityEngine;

public class Boss : MonoBehaviour, IDamageable
{
    public GameObject player = default;
    public GameObject[] Turret = default;
    public GameObject bossDiePrefab = default;
    private GameObject bossDie = default;
    private Animator ba = default;
    // 터렛을 타겟중인지 체크
    public bool isFindTurret = false;
    // 터렛을 공격중인지 체크
    public bool isAttackTurret = false;
    // 약점포인트 공격당했는지 체크
    public bool isAttackedWeakPoint = false;
    // 죽었는지 체크
    public bool isDead = false;
    public BossManager bm = default;
    private Monsters m = default;
    // 약점 포인트 프리팹들
    public GameObject[] weakpoints;

    // 몇개가 활성화 되어있는지 체크
    public int weakActiveCount = 0;


    // HSJ_ 231019
    // 프로퍼티로 변경
    // { GameManger에서 가져가서 사용할 변수
    public int MaxHp { get; private set; }
    public int CurHP { get; private set; }
    private int lastGoldHP = default;
    // { GameManger에서 가져가서 사용할 변수

    void Start()
    {
        // 보스 초기 값 셋팅
        MaxHp = JsonData.Instance.bossDatas.Boss_Data[0].Hp;
        lastGoldHP = MaxHp;
        CurHP = MaxHp;

        StartCoroutine(BossMove());

        // 약점 프리팹 모두 비활성화
        m = GetComponent<Monsters>();

        SkinnedMeshRenderer skin = GetComponentInChildren<SkinnedMeshRenderer>();
        skin.BakeMesh(GetComponent<MeshFilter>().mesh);

        ba = GetComponent<Animator>();
    }

    private IEnumerator BossMove()
    {
        Vector3 startLocation = transform.position;
        Vector3 targetLocation = player.transform.position;

        float yPosition = startLocation.y;

        // HSJ_ 231023
        // BossManager에 존재하는 타이머 중 일부를 지역변수로 변경
        float elapsedRate = default;
        float currentTime = default;

        //시작하는시간
        currentTime = 0f;
        //도착하는데 도달하는 시간(초)
        float finishTime = BossManager.instance.EndGame;
        // 경과율
        elapsedRate = currentTime / finishTime;
        while (elapsedRate < 1)
        {
            currentTime += Time.deltaTime;
            elapsedRate = currentTime / finishTime;
            // TEST : * 5f
            transform.position = Vector3.Lerp(startLocation, targetLocation, elapsedRate * 5f);
            // Y 위치를 고정
            Vector3 newPosition = new Vector3(
                Mathf.Lerp(startLocation.x, targetLocation.x, elapsedRate),
                yPosition, // 고정된 Y 위치
                Mathf.Lerp(startLocation.z, targetLocation.z, elapsedRate)
            );
            transform.position = newPosition;
            yield return null;
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            OnDamage(500000);
        }


    }

    private void AttackedWeakPoint()
    {
        isAttackedWeakPoint = true;
    }





    public void OnDamage(int damage)
    {
        if(isDead == true) { return; }

        CurHP -= damage;
        CalculateHp();

        if (CurHP <= 0)
        {
            isDead = true;
            ba.SetTrigger("Dead");
            bossDie = Instantiate(bossDiePrefab, transform.position, Quaternion.identity);


            
            StartCoroutine(Dead());

            if (transform.localScale.x < 0 && transform.localScale.y < 0 && transform.localScale.z < 0)
            {

                Destroy(gameObject);
                // TEST : HSJ_ 231024
                GameManager.Instance.WinGame();
            }

        }
    }

    IEnumerator Dead()
    {
        while (transform.localScale.x > 0 || transform.localScale.y > 0 || transform.localScale.z > 0)
        {
            Debug.Log(transform.localScale);
            transform.localScale -= new Vector3(0.02f, 0.02f, 0.02f);
            yield return new WaitForSeconds(0.01f);
        }
    }

    // ! 보스 체력 퍼센트를 계산하여 돈을 얻는 함수
    private void CalculateHp()
    {
        int rateHp = (int)(MaxHp * 0.1f);

        if (CurHP <= lastGoldHP - rateHp)
        {
            GameManager.Instance.GetGold_Boss();
            lastGoldHP = lastGoldHP - rateHp;
        }
    }       // alculateHp()
}
