using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Mode
{
    NoShotMode,
    ShotMode,
    PlaceMode,
}

// HSJ_ 231023
// IDamagebale 인터페이스 추가 
public class PlayerStatus : MonoBehaviour,IDamageable
{
    // 플레이어 체력
    /// <summary> HSJ_ 231023
    /// playerHp => maxHp로 변수명 변경
    /// int cutHp 변수 추가 
    /// 체략 초기화 및 값 할당 시점 Awake로 변경
    /// </summary>
    [Header("체력")]
    public int maxHp = default;
    public int curHp = default;

    // 플레이어의 현재 무기
    [Header("현재 무기")]
    public int playerWeapon = 0;

    // 무기 프리팹
    public GameObject[] weapons;

    // 피격 이미지
    public Image hitImage;

    // 무적 상태 인지 확인
    [Header("무적상태 확인")]
    public bool isInvincible = false;

    // { JSH_모드 전환 이식
    // 모드 전환할 Shot 클래스 
    public Shot_BSJ shot = default;
    // 모드 전환할 PlaceUnit 클래스
    public PlaceUnit placeUnit = default;
    // 모드
    public Mode mode = default;
    // } JSH_모드 전환 이식

    // { BSJ 231018
    // 대기 시간
    private float f_waitSeconds = 1.5f;
    private WaitForSeconds waitSeconds = default;

    private void Awake()
    {
        // JSH 기본 모드는 Shot모드
        mode = Mode.ShotMode;
        // 모드 적용
        ModeSwap();

        // BSJ
        // 대기 시간 캐싱
        waitSeconds = new WaitForSeconds(f_waitSeconds);

        // HSJ_ 2310123
        // 체력변수 초기화
        maxHp = 500;
        curHp = maxHp;
    }

    private void Start()
    {
        ChangeWeapon();
    }

    private void Update()
    {
        ClickButton();
    }

    //! JSH: 버튼을 인식해서 사용이 가능하도록 해주는 함수
    private void ClickButton()
    {
        // 왼쪽 컨트롤러를 기준으로 Ray를 만든다
        Ray ray_ = new Ray(BSJVRInput.LHandPosition, BSJVRInput.LHandDirection);
        RaycastHit hitInfo_ = default;
        int layer_ = 1 << LayerMask.NameToLayer("Water");

        // 버튼과 Ray 충돌 검출
        if (Physics.Raycast(ray_, out hitInfo_, 200f, layer_))
        {
            // ShotMode면
            if (mode == Mode.ShotMode)
            {
                // 버튼에는 발사하지 않는다
                mode = Mode.NoShotMode;
                ModeSwap();
            }

            // 오른쪽 One버튼으로 클릭
            if (BSJVRInput.GetUp(BSJVRInput.Button.One, BSJVRInput.Controller.RTouch))
            {
                // 버튼의 기능 실행
                hitInfo_.collider.gameObject.GetComponent<Button>().onClick.Invoke();
            }
        }
        // 버튼과 Ray 충돌 X
        else
        {
            // NoShotMode면
            if (mode == Mode.NoShotMode)
            {
                // 버튼에서 벗어나면 발사 모드로 전환
                mode = Mode.ShotMode;
                ModeSwap();
            }
        }
    }

    // 무기 스왑

    public void ChangeWeapon()
    {
        foreach (GameObject weapon in weapons)
        {
            // 현재 무기라면,
            if (weapon == weapons[playerWeapon])
            {
                // 활성화
                weapon.SetActive(true);
            }
            // 다른 무기라면,
            else
            {
                // 비활성화
                weapon.SetActive(false);
            }
        }
    }

    //! 플레이어 피격 시
    public void HitDamage_Player()
    {
        // 무적 상태 시 리턴
        if (isInvincible)
        {
            return;
        }
        // 무적 상태가 아니라면
        else
        {
            isInvincible = true;
            StartCoroutine(ActiveHitImage());
        }

    }

    //! 플레이어 피격 시 피격 이미지 활성화/비활성화
    private IEnumerator ActiveHitImage()
    {
        hitImage.enabled = true;
        yield return waitSeconds;

        hitImage.enabled = false;

        // 무적 해제
        isInvincible = false;
    }

    //! Mode에 맞춰 스크립트를 키고 끄는 함수
    public void ModeSwap()
    {
        switch (mode)
        {
            // 사격 비활성화
            case Mode.NoShotMode:
                shot.enabled = false;
                break;
            // 사격 활성화, 배치 비활성화
            case Mode.ShotMode:
                shot.enabled = true;
                placeUnit.enabled = false;
                break;
            // 배치 활성화, 사격 비활성화
            case Mode.PlaceMode:
                Debug.Log("배치 모드");
                placeUnit.enabled = true;
                shot.enabled = false;
                break;
            // 이상한 값이 들어갔을 경우 Log 표시
            default:
                Debug.Log("Mode가 이상함");
                break;
        }
    }

    // HSJ_ 231023
    // 현재 Hp가 0보다 작아질떄 0으로 고정
    public void OnDamage(int damage)
    {
        if(curHp >= damage)
        {
            curHp -= damage;
        }       
        else
        {
            curHp = 0;
        }       
    }       // OnDamage()

}
