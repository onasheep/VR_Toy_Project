using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretButton04 : MonoBehaviour
{
    // 터렛 인덱스
    private int turretIDX = default;
    // PlaceUnit class
    private PlaceUnit placeUnit = default;
    // PlayerStatus class
    private PlayerStatus playerStatus = default;


    private void Awake()
    {
        // 인덱스: PlaceUnit 참조
        turretIDX = 3;

        // PlaceUnit 가져오기
        placeUnit = FindObjectOfType<PlaceUnit>();

        // PlayerStatus 가져오기
        playerStatus = FindObjectOfType<PlayerStatus>();
    }

    //! ID SET 후 유닛 배치 실행
    public void PlaceTurret()
    {
        // 인덱스 설정
        placeUnit.SetID(turretIDX);

        // 배치 모드로 변경
        playerStatus.mode = Mode.PlaceMode;
        playerStatus.ModeSwap();
    }
}
