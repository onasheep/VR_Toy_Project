using UnityEngine;

public class BossAttack : MonoBehaviour
{
    private Boss boss;
    public BossManager bm;
    public GameObject player = default;
    public GameObject playerAttack = default;
    public GameObject TowerAttack = default;
    public GameObject monsterAttack = default;
    public LayerMask Turret = default;
    private GameObject target = default;
    private Monsters m = default;
    private int skill = default;

    private void Awake()
    {
        m = GetComponent<Monsters>();
        boss = GetComponent<Boss>();

    }

    // Update is called once per frame
    void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(boss.transform.position, 100f, Turret);

        

        if (bm.skillCoolTime > 20f)
        {
            attack();
            bm.skillCoolTime = 0;

        }
    }

    void attack()
    {
        float zrange = Vector3.Distance(gameObject.transform.position, player.transform.position);

        if (GameManager.Instance.turretLv1_List.Count + GameManager.Instance.turretLv2_List.Count + GameManager.Instance.turretLv3_List.Count + GameManager.Instance.turretLv4_List.Count > 0)
        {
            skill = Random.Range(0, 3);
        }
        else if (GameManager.Instance.turretLv1_List.Count + GameManager.Instance.turretLv2_List.Count + GameManager.Instance.turretLv3_List.Count + GameManager.Instance.turretLv4_List.Count < 1)
        {
            skill = Random.Range(0, 2);
            skill = 1;
        }


        if (skill == 0)
        {
            int howmanyAttack = Random.Range(2, 6);
            Debug.LogFormat("플레이어공격 몇개? {0}", howmanyAttack);
            for (int i = 0; i < howmanyAttack; i++)
            {
                GameObject atktoplayer = Instantiate(playerAttack,new Vector3(boss.transform.position.x, boss.transform.position.y+2f, boss.transform.position.z),Quaternion.identity);
            }

        }

        else if (skill == 1)
        {
            int howmanyAttack = Random.Range(1, 4);
            for (int i = 0; i < howmanyAttack; i++)
            {

                Debug.LogFormat("몬스터소환 몇개? {0}", howmanyAttack);
                GameObject spawnmon = Instantiate(monsterAttack, new Vector3(boss.transform.position.x, boss.transform.position.y + 2f, boss.transform.position.z), Quaternion.identity);
            }
        }

        else if (skill == 2)
        {
            int howmanyAttack = Random.Range(1, 4);
            for (int i = 0; i < howmanyAttack; i++)
            {
                Debug.LogFormat("타워공격 몇개? {0}", howmanyAttack);
                GameObject atktotower = Instantiate(TowerAttack, new Vector3(boss.transform.position.x, boss.transform.position.y + 2f, boss.transform.position.z), Quaternion.identity);
            }
        }
    }
}
