using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

// 불렛의 타입을 구별하기 위한 Enum
public enum BossAttackPoolObjType
{
    BossAttackPlayer, BossAttackSpawnMon, BossAttackTurret
}

// 외부 인스펙터창에서 클래스 정보에 접근할 수 있게 해주는 [Serializable]
[Serializable]
public class BossAttackPoolInfo
{
    // 인스펙터 창에서 보여줄 정보들
    public BossAttackPoolObjType Type;        // 오브젝트 이름 (타입)
    public int objAmount = 0;           // 생성할 풀링 오브젝트 갯수
    public GameObject prefab;           // 생성할 풀링 오브젝트 프리팹
    public GameObject container;        // 생성한 풀링오브젝트를 담을 컨테이너
    public Stack<GameObject> bossAttackPoolObj = new Stack<GameObject>();
}

public class BossAttackObjectPool : MonoBehaviour
{
    public static BossAttackObjectPool instance;

    private void Awake()
    {
        instance = this;
    }

    // 상단에 선언한 PoolInfo 클래스를 인스펙터창에서 접근하기 위한 [SerializeField]
    // 인스펙터창에서 설정한 갯수만큼 PoolInfo의 클래스를 가진 List가 생성 될 것
    // == 각 리스트에 대응하는 인덱스 안에 개별적 리스트가 생성될 것
    [SerializeField]
    List<BossAttackPoolInfo> BossAttackPoolList;

    private void Start()
    {
        // 생성할 오브젝트 풀 갯수만큼 반복
        for (int i = 0; i < BossAttackPoolList.Count; i++)
        {
            // PoolInfo 클래스에 담아둔 정보를 각 poolList에 담는다.
            FillPool(BossAttackPoolList[i]);
        }
    }


    //! 생성한 풀링오브젝트를 호출할 메소드
    public GameObject GetPoolObj(BossAttackPoolObjType type)
    {
        // GetPoolByType() 메소드로 검출하고 type 값을 BossAttackPoolInfo 클래스와 대조
        BossAttackPoolInfo select = GetPoolByType(type);

        // 해당하는 타입의 스택
        Stack<GameObject> pool = select.bossAttackPoolObj;

        // 담아둘 게임오브젝트 초기화
        GameObject objInstance;

        // 호출하는 오브젝트 수가 셋팅해둔 오브젝트로 충분하다면
        if (pool.Count > 0)
        {
            // 해당 오브젝트를 게임오브젝트에 담고
            objInstance = pool.Peek();
            // Stack 메모리에서 빼준다.
            pool.Pop();
        }

        // 호출하는 오브젝트 수가 셋팅 값 보다 많다면
        else
        {
            // 풀링오브젝트를 새로 생성해준다.
            objInstance = Instantiate(select.prefab, select.container.transform);
            // TEST : SJ_231013 
            pool.Push(objInstance);
        }

        // 담긴 오브젝트 반환
        return objInstance;
    }       // GetPoolObj()

    //! 호출된 풀링오브젝트를 풀에 다시 반환하는 메소드
    public void CoolObj(GameObject obj, BossAttackPoolObjType type)
    {
        BossAttackPoolInfo select = GetPoolByType(type);
        obj.SetActive(false);
        Stack<GameObject> pool = select.bossAttackPoolObj;

        if (pool.Contains(obj) == false)
        {
            pool.Push(obj);
        }
    }


    // PoolInfo 클래스 셋팅 값 기반으로 풀링오브젝트 생성
    private void FillPool(BossAttackPoolInfo bossAttackPoolInfo)
    {
        // PoolInfo 클래스에서 셋팅한 objAmount 만큼 반복
        for (int i = 0; i < bossAttackPoolInfo.objAmount; i++)
        {
            // 풀링할 오브젝트를 담을 변수 초기화
            GameObject tempObj = default;

            // 인스턴스화 시킨 오브젝트 담아넣기
            tempObj = Instantiate(bossAttackPoolInfo.prefab, bossAttackPoolInfo.container.transform);
            bossAttackPoolInfo.bossAttackPoolObj.Push(tempObj);
            // 생성한 오브젝트 비활성화, 위치 초기화, 메모리 할당하기
            tempObj.SetActive(false);
            tempObj.transform.position = Vector3.zero;
        }
    }

    //! 호출하는 오브젝트 종류를 검출하는 반복문
    private BossAttackPoolInfo GetPoolByType(BossAttackPoolObjType type)
    {
        // 오브젝트 풀 갯수만큼 반복문
        for (int i = 0; i < BossAttackPoolList.Count; i++)
        {
            // 호출하는 오브젝트의 타입과 일치한다면
            if (type == BossAttackPoolList[i].Type)
            {
                // 해당하는 오브젝트 풀의 인덱스를 반환.
                return BossAttackPoolList[i];
            }
        }

        // 일치하는 타입이 없다면 null 리턴
        return null;
    }

}

