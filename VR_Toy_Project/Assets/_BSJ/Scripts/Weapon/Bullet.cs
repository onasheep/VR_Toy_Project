using TMPro;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // 총알의 타입
    public PoolObjType bulletType;

    // 총알 타격 이펙트
    private VFXPoolObjType vfxType;

    // 총알 데미지 텍스트
    private TextPoolObjType textType;

    // 총알의 속도
    [SerializeField]
    private float bulletSpeed = default;

    // 총알의 데미지
    [SerializeField]
    private int bulletDamage = default;

    // 치명타 확률
    [SerializeField]
    private float criticalPercent = default;

    // 치명타 배율
    [SerializeField]
    private float criticalDamage = default;

    // 최종 데미지
    [SerializeField]
    private int finalDamage = default;

    private void Awake()
    {
        // { 총알 타입에 따른 총알 조정
        if (bulletType == PoolObjType.Bullet01)
        {
            vfxType = VFXPoolObjType.Bullet01_HitVFX;
            textType = TextPoolObjType.DamageText01;
            bulletSpeed = JsonData.Instance.bulletDatas.Bullet[0].Bullet_Speed;
            bulletDamage = JsonData.Instance.bulletDatas.Bullet[0].Att;
            criticalPercent = JsonData.Instance.bulletDatas.Bullet[0].Cri_Chance;
            criticalDamage = JsonData.Instance.bulletDatas.Bullet[0].Cri_Damege;
        }
        else if (bulletType == PoolObjType.Bullet02)
        {
            vfxType = VFXPoolObjType.Bullet02_HitVFX;
            textType = TextPoolObjType.DamageText01;
            bulletSpeed = JsonData.Instance.bulletDatas.Bullet[1].Bullet_Speed;
            bulletDamage = JsonData.Instance.bulletDatas.Bullet[1].Att;
            criticalPercent = JsonData.Instance.bulletDatas.Bullet[1].Cri_Chance;
            criticalDamage = JsonData.Instance.bulletDatas.Bullet[1].Cri_Damege;
        }
        else if (bulletType == PoolObjType.Bullet03)
        {
            vfxType = VFXPoolObjType.Bullet03_HitVFX;
            textType = TextPoolObjType.DamageText01;
            bulletSpeed = JsonData.Instance.bulletDatas.Bullet[2].Bullet_Speed;
            bulletDamage = JsonData.Instance.bulletDatas.Bullet[2].Att;
            criticalPercent = JsonData.Instance.bulletDatas.Bullet[2].Cri_Chance;
            criticalDamage = JsonData.Instance.bulletDatas.Bullet[2].Cri_Damege;
        }
        else if (bulletType == PoolObjType.Bullet04)
        {
            vfxType = VFXPoolObjType.Bullet04_HitVFX;
            textType = TextPoolObjType.DamageText01;
            bulletSpeed = JsonData.Instance.bulletDatas.Bullet[3].Bullet_Speed;
            bulletDamage = JsonData.Instance.bulletDatas.Bullet[3].Att;
            criticalPercent = JsonData.Instance.bulletDatas.Bullet[3].Cri_Chance;
            criticalDamage = JsonData.Instance.bulletDatas.Bullet[3].Cri_Damege;
        }
        else if (bulletType == PoolObjType.Bullet05)
        {
            vfxType = VFXPoolObjType.Bullet05_HitVFX;
            textType = TextPoolObjType.DamageText01;
            bulletSpeed = JsonData.Instance.bulletDatas.Bullet[4].Bullet_Speed;
            bulletDamage = JsonData.Instance.bulletDatas.Bullet[4].Att;
            criticalPercent = JsonData.Instance.bulletDatas.Bullet[4].Cri_Chance;
            criticalDamage = JsonData.Instance.bulletDatas.Bullet[4].Cri_Damege;
        }
        else if (bulletType == PoolObjType.Bullet06)
        {
            vfxType = VFXPoolObjType.Bullet06_HitVFX;
            textType = TextPoolObjType.DamageText01;
            bulletSpeed = JsonData.Instance.bulletDatas.Bullet[5].Bullet_Speed;
            bulletDamage = JsonData.Instance.bulletDatas.Bullet[5].Att;
            criticalPercent = JsonData.Instance.bulletDatas.Bullet[5].Cri_Chance;
            criticalDamage = JsonData.Instance.bulletDatas.Bullet[5].Cri_Damege;
        }
        else if (bulletType == PoolObjType.Bullet07)
        {
            vfxType = VFXPoolObjType.Bullet07_HitVFX;
            textType = TextPoolObjType.DamageText01;
            bulletSpeed = JsonData.Instance.bulletDatas.Bullet[6].Bullet_Speed;
            bulletDamage = JsonData.Instance.bulletDatas.Bullet[6].Att;
            criticalPercent = JsonData.Instance.bulletDatas.Bullet[6].Cri_Chance;
            criticalDamage = JsonData.Instance.bulletDatas.Bullet[6].Cri_Damege;
        }
        else if (bulletType == PoolObjType.Bullet08)
        {
            vfxType = VFXPoolObjType.Bullet08_HitVFX;
            textType = TextPoolObjType.DamageText01;
            bulletSpeed = JsonData.Instance.bulletDatas.Bullet[7].Bullet_Speed;
            bulletDamage = JsonData.Instance.bulletDatas.Bullet[7].Att;
            criticalPercent = JsonData.Instance.bulletDatas.Bullet[7].Cri_Chance;
            criticalDamage = JsonData.Instance.bulletDatas.Bullet[7].Cri_Damege;
        }
        // } 총알 타입에 따른 총알 조정
    }

    private void OnEnable()
    {
        // 치명타 확률 계산하여 치명타 데미지 계산
        float _critCheck = Random.Range(0.0f, 1.0f);

        if (_critCheck < criticalPercent)
        {
            finalDamage = (int)(bulletDamage * criticalDamage);
        }
        else
        {
            finalDamage = bulletDamage;
        }
    }

    void Update()
    {
        // 총알이 계속 앞으로 날아감.
        transform.Translate(Vector3.forward * (bulletSpeed / 5f) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {

        // 약점 || 졸개 || 보스 공격 투사체 || 보스 스폰 알
        if (other.CompareTag("BigWeakPoint") || other.CompareTag("WeakPoint") || other.CompareTag("Monster") || other.CompareTag("BossAttackPlayer") || other.CompareTag("BossAttackSpawnMon") || other.CompareTag("BossAttackTurret") || other.CompareTag("Boss"))
        {
            // 타격 이펙트 콜
            GameObject hitVFX = VFXObjectPool.instance.GetPoolObj(vfxType);
            hitVFX.SetActive(true);
            // LEGACY : 
            //hitVFX.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1f);
            hitVFX.transform.position = other.ClosestPointOnBounds(this.transform.position - new Vector3(0f, 0f, 5f));
            
            // JSH: Bullet08일 때만 스플래시 실행
            if (bulletType == PoolObjType.Bullet08)
            {
                Splash();
            }
            // 나머지는 부딪힌 것에만 데미지
            else
            {
                if (other.CompareTag("BigWeakPoint"))
                {
                    bool isLive = other.GetComponent<WeakPointBig>().isLive;
                    if (isLive == true)
                    {

                        finalDamage = (int)(bulletDamage * criticalDamage * 1.3f);
                    }
                    else
                    {
                        finalDamage = (int)(bulletDamage * criticalDamage);
                    }
                    other.GetComponent<WeakPointBig>().OnDamage(finalDamage);
                }
                
                else if (other.CompareTag("WeakPoint"))
                {
                    bool isLive = other.GetComponent<WeakPointSmall>().isLive;
                    if (isLive == true)
                    {
                        finalDamage = (int)(bulletDamage * criticalDamage * 2f);
                    }
                    else
                    {
                        finalDamage = (int)(bulletDamage * criticalDamage);
                    }

                    other.GetComponent<WeakPointSmall>().OnDamage(finalDamage);
                }
                
                else if (other.CompareTag("Monster"))
                {
                    other.GetComponent<Monsters>().OnDamage(finalDamage);
                }
                
                else if (other.CompareTag("BossAttackPlayer"))
                {
                    other.GetComponent<BossBombAttackPlayer>().OnDamage(finalDamage);
                }
                
                else if (other.CompareTag("BossAttackTurret"))
                {
                    other.GetComponent<BossBombAttackTurret>().OnDamage(finalDamage);
                }
                
                else if (other.CompareTag("BossAttackSpawnMon"))
                {
                    other.GetComponent<BossBombSpawnMon>().OnDamage(finalDamage);
                }
                
                else if (other.CompareTag("Boss"))
                {
                    other.attachedRigidbody.GetComponent<Boss>().OnDamage(finalDamage);
                }
            }
            // } 실제 데미지를 입히는 로직

            // { 타격 데미지 텍스트 콜
            GameObject damageText = TextObjectPool.instance.GetPoolObj(textType);

            // 총알 데미지 텍스트 변경
            damageText.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("{0}", finalDamage);


            float dist = (this.transform.position - Vector3.zero).magnitude;
            
            damageText.SetActive(true);

             
            damageText.transform.position = new Vector3(transform.position.x + Random.Range(-0.25f, 0.25f), transform.position.y + Random.Range(-0.25f, 0.25f), transform.position.z - 1f);


            // TEST :
            float ratio = 10f;

            ratio = dist > 5f ?  10f : 5f;
            damageText.GetComponentInParent<RectTransform>().localScale = new Vector3( dist / ratio, dist / ratio, 1f);

            // } 타격 데미지 텍스트 콜

            // 탄환은 오브젝트 풀로 반환
            BulletObjectPool.instance.CoolObj(gameObject, bulletType);
        }

        // 바닥에 맞으면,
        else if (other.CompareTag("Terrain"))
        {
            // 타격 이펙트 콜
            GameObject hitVFX = VFXObjectPool.instance.GetPoolObj(vfxType);
            hitVFX.SetActive(true);
            hitVFX.transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);

            // 탄환 오브젝트 풀로 반환
            BulletObjectPool.instance.CoolObj(gameObject, bulletType);
        }

        // 터렛에 맞으면,
        else if (other.CompareTag("Turret"))
        {
            // 타격 이펙트 콜
            GameObject hitVFX = VFXObjectPool.instance.GetPoolObj(vfxType);
            hitVFX.SetActive(true);
            hitVFX.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);

            // 탄환 오브젝트 풀로 반환
            BulletObjectPool.instance.CoolObj(gameObject, bulletType);
        }

        // 다른 곳에 맞으면,
        else { /*Do Nothing*/ }
    }       // OnTriggerEnter()

    //! 졸개에게 부딪힌 후 사용할 스플래시 공격 기능
    private void Splash()
    {
        // 몬스터 레이어에 속한 오브젝트만 
        Collider[] hitObjects_ = Physics.OverlapSphere(transform.position, 0.4f, 1 << LayerMask.NameToLayer("Monster"));

        // 검출된 모든 오브젝트에 실행
        foreach (Collider target_ in hitObjects_)
        {
            target_.GetComponent<Monsters>().OnDamage(finalDamage);
        }
    }
}