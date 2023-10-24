using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    private GameObject _monsterSpawner = default;
    public GameObject Monsterlv1 = default;
    public GameObject Monsterlv2 = default;
    public GameObject Monsterlv3 = default;
    private Boss boss = default;
    // 스폰 포인트
    public GameObject spawnPoint01;
    public GameObject spawnPoint02;

    // BSJ _ 테스트 용 시간 단축 
    public float testTimeReduce;

    private float spawnTime = 5f;

    // HSJ_ 231023
    // 게임 종료시간 캐싱할 변수
    private float endTime = default;
    private float lv1Time = default;
    private float lv2Time = default;

    void Start()
    {
        _monsterSpawner = gameObject;
        // HSJ_ 231023
        // { time 캐싱
        endTime = GameManager.Instance.EndTime;
        lv1Time = endTime * 0.3f;
        lv2Time = endTime * 0.6f;
        // } time 초기화
    }

    private void Update()
    {
        boss = GetComponentInParent<Boss>();
        if(BossManager.instance.spawn > spawnTime && boss.CurHP >0)
        {
            CreateMonster();
            BossManager.instance.spawn = 0f;
        }
    }

    // TODO : TEST 용 testTimeReduce 변수 제거 할 것     
    private void CreateMonster()
    {
            if (GameManager.Instance.CurTime < lv1Time * testTimeReduce)
            {
                for (int i = 0; i < 3; i++)
                {
                    FirstWave();

                }
            }
            else if (lv1Time * testTimeReduce < GameManager.Instance.CurTime && GameManager.Instance.CurTime < lv2Time * testTimeReduce)
            {
                for (int i = 0; i < 4; i++)
                {
                    SecondWave();

                }
            }
            else if (lv2Time * testTimeReduce < GameManager.Instance.CurTime )
            {
                for (int i = 0; i < 5; i++)
                {
                    ThirdWave();

                }
            }
    }

    void FirstWave()
    {
        int spawnx=Random.Range((int)spawnPoint01.transform.position.x, (int)spawnPoint02.transform.position.x);

        // Test: 오브젝트 풀을 사용하여 몬스터 소환
        GameObject monster01 = MonsterObjectPool.instance.GetPoolObj(MonsterPoolObjType.Monster_Lv1);
        monster01.SetActive(true);
        monster01.transform.position = new Vector3(spawnx, _monsterSpawner.transform.position.y, _monsterSpawner.transform.position.z);

        // 몬스터 생성 LEGACY : 오브젝트 풀에서 생성하기로 하였음. BSJ_231023
        // GameObject Mon1 = Instantiate(Monsterlv1, _monsterSpawner.transform.position, Quaternion.identity);
        // Mon1.transform.position = new Vector3(spawnx, _monsterSpawner.transform.position.y, _monsterSpawner.transform.position.z);
    }

    void SecondWave()
    {
        int spawnx = Random.Range((int)spawnPoint01.transform.position.x, (int)spawnPoint02.transform.position.x);

        // Test: 오브젝트 풀을 사용하여 몬스터 소환
        GameObject monster02 = MonsterObjectPool.instance.GetPoolObj(MonsterPoolObjType.Monster_Lv2);
        monster02.SetActive(true);
        monster02.transform.position = new Vector3(spawnx, _monsterSpawner.transform.position.y, _monsterSpawner.transform.position.z);

        // 몬스터 생성 LEGACY : 오브젝트 풀에서 생성하기로 하였음. BSJ_231023
        // GameObject Mon2 = Instantiate(Monsterlv2, _monsterSpawner.transform.position, Quaternion.identity);
        // Mon2.transform.position = new Vector3(spawnx, _monsterSpawner.transform.position.y, _monsterSpawner.transform.position.z);
    }
    void ThirdWave()
    {
        int spawnx = Random.Range((int)spawnPoint01.transform.position.x, (int)spawnPoint02.transform.position.x);

        // Test: 오브젝트 풀을 사용하여 몬스터 소환
        GameObject monster03 = MonsterObjectPool.instance.GetPoolObj(MonsterPoolObjType.Monster_Lv3);
        monster03.SetActive(true);
        monster03.transform.position = new Vector3(spawnx, _monsterSpawner.transform.position.y, _monsterSpawner.transform.position.z);

        // 몬스터 생성 LEGACY : 오브젝트 풀에서 생성하기로 하였음. BSJ_231023
        // GameObject Mon3 = Instantiate(Monsterlv3, _monsterSpawner.transform.position, Quaternion.identity);
        // Mon3.transform.position = new Vector3(spawnx, _monsterSpawner.transform.position.y, _monsterSpawner.transform.position.z);
    }

    // TODO: 몬스터 소환하는 함수 만들어야함.
}
