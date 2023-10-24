using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class TurretUnit : MonoBehaviour
{
    // { 터렛 유닛이 가지고 있어야 하는 변수들
    // 터렛 이름
    protected string myName = default;
    // ID
    protected int unitID = default;
    // 터렛 최대 체력
    protected int healthMax = default;
    // 터렛 현재 체력
    protected int health = default;
    // 터렛 비용
    public int cost = default;
    // 터렛 상한
    protected int install_Limit = default;
    // 터렛 탐지 범위
    protected int range = default;
    // 터렛 발사 간격
    protected float firing_Interval = default;
    // 사용할 탄환 ID
    protected int bullet_Table_ID = default;
    // 사용할 터렛 모델
    protected string image = default;
    // 가장 가까운 목표
    protected Transform target = default;

    // 탐지 준비 여부
    protected bool isReadyDetect = true;
    // 공격 준비 여부
    protected bool isReadyAttack = false;

    // 터렛의 포탑 편하게 사용하기 위해 직렬화
    [SerializeField]
    protected GameObject head = default;
    // 마찬가지로 포구 직렬화
    [SerializeField]
    protected GameObject muzzle = default;
    // } 터렛 유닛이 가지고 있어야 하는 변수들


    //! 터렛 자신의 정보 초기화
    public virtual void Init(string name_, int unitID_, int health_, int cost_, int limit_, int range_, float interval_, int bulletID_)
    {
        // TODO: 변수 초기화 추가 
        myName = name_;
        unitID = unitID_;
        healthMax = health_;
        health = healthMax;
        cost = cost_;
        install_Limit = limit_;
        range = range_;
        firing_Interval = interval_;
        bullet_Table_ID = bulletID_;

        // 목표와 공격 준비는 하위 클래스에서
    }

    //! 터렛의 탐지 로직
    protected virtual void DetectTarget()
    {
        // 영역 안의 Monster레이어 오브젝트 검출
        Collider[] hitObjects_ = Physics.OverlapSphere(transform.position, range, 1 << LayerMask.NameToLayer("Monster"));

        // 목표가 이미 존재한다면 탐지 실행 X
        foreach (Collider collider in hitObjects_)
        {
            if (collider.gameObject == target) return;
        }

        // 영역 안에 탐지된 것이 존재하지 않음
        if (hitObjects_.Length <= 0)
        {
            // 사이클 재시작
            Invoke("DetectTarget", firing_Interval);
            target = default;

            return;
        }
        else
        {
            isReadyDetect = false;
        }

        // 가장 가까운 목표를 탐색하는 루프
        int closest_ = 0;
        for (int i = closest_ + 1; i < hitObjects_.Length; i++)
        {
            // 가장 가까운 목표와의 거리
            float closestDistance_ = (transform.position - hitObjects_[closest_].transform.position).magnitude;
            // 다음 목표와의 거리
            float nextDistance_ = (transform.position - hitObjects_[i].transform.position).magnitude;

            // 다음 목표가 더 가깝다면
            if (nextDistance_ < closestDistance_)
            {
                // 다음 목표의 인덱스로 교체
                closest_ = i;
            }
            else { /* Do Nothing */ }
        }

        // 탐색한 목표를 저장
        target = hitObjects_[closest_].transform;

        Invoke("AttackTarget", firing_Interval);
    }       // DetectTarget()

    //! 터렛의 공격 로직
    protected virtual void AttackTarget()
    {
        // 목표가 존재하지 않으면 실행하지 않음
        // if (target == null || target == default)
        // 목표가 비활성화 상태라면 탐지 실행
        if (target.gameObject.activeSelf == false)
        {
            Invoke("DetectTarget", firing_Interval);
            isReadyDetect = true;
            return;
        }

        // 적을 향해 포탑 회전
        Vector3 direction = target.position - head.transform.position;

        // 포구가 forward 상태에서 오른쪽을 보고 있음...
        direction.y = 0;
        //direction = new Vector3(0, direction.y - 90, 0);

        // 만들어준 방향으로 회전
        head.transform.right = direction.normalized;

        // 포구에서 총알 발사
        GameObject bullet = BulletObjectPool.instance.GetPoolObj((PoolObjType)bullet_Table_ID);
        bullet.gameObject.SetActive(true);
        bullet.transform.position = muzzle.transform.position;
        bullet.transform.forward = muzzle.transform.right;

        Invoke("AttackTarget", firing_Interval);
    }       // AttackTarget()

    //! 터렛의 체력 변화
    public virtual void DamageSelf(int damage_)
    {
        // 체력 감소
        health -= damage_;

        // TODO: 감소된 체력 UI에 반영

        // 체력이 0이하로 내려가면 죽음
        if (health <= 0)
        {
            health = 0;

            DestroySelf();
        }
    }

    //! 터렛의 파괴
    protected virtual void DestroySelf()
    {
        // TODO: 배치된 터렛 수 감소
        GameManager.Instance.turretLv1_List.Remove(this.transform);

        // 스스로를 파괴
        Destroy(gameObject);
    }
}
