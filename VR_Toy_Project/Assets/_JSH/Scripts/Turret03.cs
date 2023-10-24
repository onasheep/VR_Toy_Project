using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret03 : TurretUnit
{
    // head와 muzzle 인스펙터창에서 할당 필요

    private void Awake()
    {
        // 스탯 초기화
        Init(
            JsonData.Instance.unitDatas.Unit[2].Name,
            JsonData.Instance.unitDatas.Unit[2].ID,
            JsonData.Instance.unitDatas.Unit[2].HP,
            JsonData.Instance.unitDatas.Unit[2].Cost,
            JsonData.Instance.unitDatas.Unit[2].Install_Limit,
            JsonData.Instance.unitDatas.Unit[2].Range,
            JsonData.Instance.unitDatas.Unit[2].Firing_Interval,
            JsonData.Instance.unitDatas.Unit[2].Bullet_Table_ID
            );
    }

    private void Start()
    {
        // 터렛의 존재 유무와 갯수를 세기 위해 추가
        GameManager.Instance.turretLv1_List.Add(transform);
    }

    private void OnEnable()
    {
        DetectTarget();
    }
}
