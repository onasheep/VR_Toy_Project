using System.Collections;
using UnityEngine;
public class BossBombSpawnMon : MonoBehaviour, IDamageable
{
    // 알 Hp
    public int bossBombSpawnMonHp = default;

    public float initialAngle = 30f;    // 처음 날라가는 각도

    public int targetx;
    public int targetz;
    private Vector3 RandomTarget =default;
    private GameObject bossBombSpawnMon = default;
    private float Shottime;
    private Rigidbody rb;               // Rigidbody
    private int randomX;

    public GameObject Monsterlv1 = default;
    public GameObject Monsterlv2 = default;
    public GameObject Monsterlv3 = default;

    private void Awake()
    {
        // 체력 셋팅
        bossBombSpawnMonHp = JsonData.Instance.bossSkillDatas.Boss_Skill[2].Hp;

        rb = GetComponent<Rigidbody>();
        Shottime = 0;
        bossBombSpawnMon = this.gameObject;
        
    }

    private void Start()
    {
        targetx = Random.Range(-500, 480);
        targetz = Random.Range(-500, 480);
        randomX = Random.Range(-50, 50);
        RandomTarget = new Vector3(targetx, 0, targetz);
        StartCoroutine(Firsttime());

    }

    private void Update()
    {
        if (bossBombSpawnMon.transform.position.y < 2)
        {
            spawnMons();
            bossBombSpawnMon.gameObject.SetActive(false);
        }

        // 체력이 0이되면 비활성화
        if (bossBombSpawnMonHp <= 0)
        {
            gameObject.SetActive(false);
        }
    }


    IEnumerator Firsttime()
    {
        randomX = Random.Range(-50, 50);
        rb.useGravity = false;
        Vector3 velocity = new(15, 0, 0);
        rb.velocity = velocity;
        
        yield return new WaitForSeconds(1.5f);
        rb.velocity = Vector3.zero;

        yield return new WaitForSeconds(1.5f);
        rb.useGravity = true;
        // 포물선 운동
        velocity = GetVelocity(transform.position, RandomTarget, initialAngle);
        rb.velocity = velocity;


    }

    private void spawnMons()
    {
        if (BossManager.instance.currentTime < 300f)
        {
            for (int i = 0; i < 3; i++)
            {
                FirstWave();

            }
        }
        else if (300f < BossManager.instance.currentTime && BossManager.instance.currentTime < 600f)
        {
            for (int i = 0; i < 4; i++)
            {
                SecondWave();

            }
        }
        else if (600f < BossManager.instance.currentTime && BossManager.instance.currentTime < 900f)
        {
            for (int i = 0; i < 5; i++)
            {
                ThirdWave();

            }
        }
    }

    void FirstWave()
    {
        
        int randomx = Random.Range(targetx - 50, targetx + 50);
        int randomz = Random.Range(targetz - 50, targetz + 50);

        GameObject Mon1 = Instantiate(Monsterlv1, bossBombSpawnMon.transform.position, Quaternion.identity);
        Mon1.transform.position = new Vector3(randomx, bossBombSpawnMon.transform.position.y, randomz);
    }

    void SecondWave()
    {
        int randomx = Random.Range(targetx - 50, targetx + 50);
        int randomz = Random.Range(targetz - 50, targetz + 50);
        
        GameObject Mon2 = Instantiate(Monsterlv2, bossBombSpawnMon.transform.position, Quaternion.identity);
        Mon2.transform.position = new Vector3(randomx, bossBombSpawnMon.transform.position.y, randomz);
    }
    void ThirdWave()
    {
        int randomx = Random.Range(targetx - 50, targetx + 50);
        int randomz = Random.Range(targetz - 50, targetz + 50);

        GameObject Mon3 = Instantiate(Monsterlv3, bossBombSpawnMon.transform.position, Quaternion.identity);
        Mon3.transform.position = new Vector3(randomx, bossBombSpawnMon.transform.position.y, randomz);
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

        bossBombSpawnMonHp -= damage;

    }
}