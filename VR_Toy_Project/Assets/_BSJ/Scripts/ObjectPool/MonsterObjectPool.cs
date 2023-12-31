using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 몬스터의 타입을 구별하기 위한 Enum
public enum MonsterPoolObjType { Monster_Lv1, Monster_Lv2, Monster_Lv3, DeathBomb }

// 외부 인스펙터창에서 클래스 정보에 접근할 수 있게 해주는 [Serializable]
[Serializable]
public class MonsterPoolInfo
{
    // 인스펙터 창에서 보여줄 정보들
    public MonsterPoolObjType Type;        // 오브젝트 이름 (타입)
    public int objAmount = 0;       // 생성할 풀링 오브젝트 갯수
    public GameObject prefab;       // 생성할 풀링 오브젝트 프리팹
    public GameObject container;    // 생성한 풀링오브젝트를 담을 컨테이너
    public Stack<GameObject> monsterPoolObj = new Stack<GameObject>();
}

public class MonsterObjectPool : MonoBehaviour
{
    public static MonsterObjectPool instance;

    private void Awake()
    {
        instance = this;
    }

    // 상단에 선언한 VFXPoolInfo 클래스를 인스펙터창에서 접근하기 위한 [SerializeField]
    // 인스펙터창에서 설정한 갯수만큼 VFXPoolInfo의 클래스를 가진 List가 생성 될 것
    // == 각 리스트에 대응하는 인덱스 안에 개별적 리스트가 생성될 것
    [SerializeField]
    List<MonsterPoolInfo> monsterPoolList;

    private void Start()
    {
        // 생성할 오브젝트 풀 갯수만큼 반복
        for (int i = 0; i < monsterPoolList.Count; i++)
        {
            // VFXPoolInfo 클래스에 담아둔 정보를 각 poolList에 담는다.
            FillPool(monsterPoolList[i]);
        }
    }


    //! 생성한 풀링오브젝트를 호출할 메소드
    public GameObject GetPoolObj(MonsterPoolObjType type)
    {
        // GetPoolByType() 메소드로 검출하고 type 값을 VFXPoolInfo 클래스와 대조
        MonsterPoolInfo select = GetPoolByType(type);

        // 해당하는 타입의 스택
        Stack<GameObject> pool = select.monsterPoolObj;

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
        }

        // 담긴 오브젝트 반환
        return objInstance;
    }       // GetPoolObj()

    //! 호출된 풀링오브젝트를 풀에 다시 반환하는 메소드
    public void CoolObj(GameObject obj, MonsterPoolObjType type)
    {
        MonsterPoolInfo select = GetPoolByType(type);
        obj.SetActive(false);
        obj.transform.position = select.container.transform.position;
        Stack<GameObject> pool = select.monsterPoolObj;

        if (pool.Contains(obj) == false)
        {
            pool.Push(obj);
        }
    }


    // MonsterPoolInfo 클래스 셋팅 값 기반으로 풀링오브젝트 생성
    private void FillPool(MonsterPoolInfo MonsterPoolInfo)
    {
        // MonsterPoolInfo 클래스에서 셋팅한 objAmount 만큼 반복
        for (int i = 0; i < MonsterPoolInfo.objAmount; i++)
        {
            // 풀링할 오브젝트를 담을 변수 초기화
            GameObject tempObj = default;

            // 인스턴스화 시킨 오브젝트 담아넣기
            tempObj = Instantiate(MonsterPoolInfo.prefab, MonsterPoolInfo.container.transform);
            MonsterPoolInfo.monsterPoolObj.Push(tempObj);
            // 생성한 오브젝트 비활성화, 위치 초기화, 메모리 할당하기
            tempObj.SetActive(false);
            tempObj.transform.position = MonsterPoolInfo.container.transform.position;
        }
    }

    //! 호출하는 오브젝트 종류를 검출하는 반복문
    private MonsterPoolInfo GetPoolByType(MonsterPoolObjType type)
    {
        // 오브젝트 풀 갯수만큼 반복문
        for (int i = 0; i < monsterPoolList.Count; i++)
        {
            // 호출하는 오브젝트의 타입과 일치한다면
            if (type == monsterPoolList[i].Type)
            {
                // 해당하는 오브젝트 풀의 인덱스를 반환.
                return monsterPoolList[i];
            }
        }

        // 일치하는 타입이 없다면 null 리턴
        return null;
    }

}

