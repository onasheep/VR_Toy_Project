using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceUnit : MonoBehaviour
{
    // 배치할 유닛 ID
    private int turretID = default;
    // 배치할 유닛 프리팹들
    public GameObject[] turretPrefabs = default;
    // 유닛 배치 UI
    public Transform[] placeUnitUI = default;
    // 선을 그릴 라인 렌더러
    private LineRenderer lineRenderer = default;
    // 버튼 클릭용 클래스
    PlayerStatus playerStatus = default;

    // 배치 불가능 여부
    public bool isPlacable = true;
    // UI 마테리얼
    public Material placeMaterial = default;
    // 배치 가능 RGB
    private Color canPlace = default;
    // 배치 불가능 RGB
    private Color cantPlace = default;

    private void Awake()
    {
        // 시작할 때 비활성화
        placeUnitUI[0].gameObject.SetActive(false);
        placeUnitUI[1].gameObject.SetActive(false);
        placeUnitUI[2].gameObject.SetActive(false);
        placeUnitUI[3].gameObject.SetActive(false);

        // 설정한 Color값
        canPlace = new Color(191 / 255f, 191 / 255f, 191 / 255f, 127 / 255f);
        // 설정한 Color값
        cantPlace = new Color(1.0f, 127 / 255f, 127 / 255f, 127 / 255f);

        // 라인 렌더러 컴포넌트 얻어오기
        lineRenderer = GetComponent<LineRenderer>();

        // 라인 렌더러 비활성화
        lineRenderer.enabled = false;

        // 버튼 클릭 클래스 받아오기
        playerStatus = FindObjectOfType<PlayerStatus>();
    }

    private void OnEnable()
    {
        // 라인 렌더러 컴포넌트 활성화
        lineRenderer.enabled = true;

        // 배치 가능 여부 리셋
        isPlacable = true;
        SetUIEnable();
        // 유닛 배치 UI 활성화
        placeUnitUI[turretID].gameObject.SetActive(true);

        Debug.Log("켜졌나");
    }

    private void Update()
    {
        // 오른쪽 컨트롤러의 One 버튼에서 손을 떼면
        if (BSJVRInput.GetUp(BSJVRInput.Button.One, BSJVRInput.Controller.RTouch))
        {
            // 라인 렌더러 컴포넌트 비활성화
            lineRenderer.enabled = false;

            // 재화가 충분한지 체크, 배치 가능한 장소인지 체크
            if (GameManager.Instance.Gold >= JsonData.Instance.unitDatas.Unit[turretID].Cost && isPlacable == true)
            {
                // 설치 가능
                SetUIEnable();
            }
            // 충분하지 않음
            else if (GameManager.Instance.Gold < JsonData.Instance.unitDatas.Unit[turretID].Cost)
            {
                // 설치 불가능
                SetUIDisable();
            }

            // 배치가 가능할 때만 설치
            if (isPlacable)
            {
                // 유닛 배치 UI 위치에 유닛 생성
                SpawnUnit();
            }

            // 유닛 배치 UI의 좌표 맵 아래로 이동
            placeUnitUI[turretID].transform.position = Vector3.up * -100;

            // 유닛 배치 UI 비활성화
            placeUnitUI[turretID].gameObject.SetActive(false);

            // 배치가 끝났으니 모드 전환
            playerStatus.mode = Mode.ShotMode;
            playerStatus.ModeSwap();
        }

        // 오른쪽 컨트롤러를 기준으로 Ray를 만든다
        Ray ray_ = new Ray(BSJVRInput.RHandPosition, BSJVRInput.RHandDirection);
        RaycastHit hitInfo_ = default;
        int layer_ = 1 << LayerMask.NameToLayer("Terrain");

        // Terrain만 Ray 충돌 검출
        if (Physics.Raycast(ray_, out hitInfo_, 200f, layer_))
        {
            // Ray가 부딪힌 지점에 라인 그리기
            lineRenderer.SetPosition(0, ray_.origin);
            lineRenderer.SetPosition(1, hitInfo_.point);

            // Ray가 부딪힌 지점에 유닛 배치 UI 표시
            placeUnitUI[turretID].position = hitInfo_.point;
            // 유닛 배치 UI의 Head가 위로 향하도록 방향 설정
            placeUnitUI[turretID].up = hitInfo_.normal;
            // 유닛 배치 UI가 앞을 보도록 설정
            placeUnitUI[turretID].right = hitInfo_.transform.forward;


            // 영역 안의 Turret레이어 오브젝트들
            Collider[] hitObjects_ = Physics.OverlapSphere(placeUnitUI[turretID].transform.position, 0.4f, 1 << LayerMask.NameToLayer("Turret"));

            // 영역 안에 탐지된 것이 존재
            if (hitObjects_.Length > 0)
            {
                // 설치 불가능
                SetUIDisable();
            }
            // 영역 안에 탐지된 것이 존재하지 않음
            else if (hitObjects_.Length <= 0)
            {
                // 설치 가능
                SetUIEnable();
            }
        }
        else
        {
            // Ray 충돌이 발생하지 않으면 선이 Ray 방향으로 그려지도록 처리
            lineRenderer.SetPosition(0, ray_.origin);
            lineRenderer.SetPosition(1, ray_.origin + BSJVRInput.RHandDirection * 200f);

            // 유닛 배치 UI를 맵 아래의 지정된 좌표로 이동
            placeUnitUI[turretID].transform.position = Vector3.right * 1.2f * turretID + Vector3.up * -100;
        }
    }       // Update()

    //! 터렛 배치 UI 위치에 터렛을 생성하는 함수
    private void SpawnUnit()
    {
        // 재화 차감
        GameManager.Instance.UseGold(JsonData.Instance.unitDatas.Unit[turretID].Cost);

        // ID에 맞는 프리팹 찾아서 소환
        Transform unit_ = Instantiate(turretPrefabs[turretID]).transform;

        unit_.position = placeUnitUI[turretID].position;
        unit_.forward = placeUnitUI[turretID].forward;
        unit_.localScale = placeUnitUI[turretID].localScale;
    }

    //! 터렛 ID SET
    public void SetID(int idNum_)
    {
        turretID = idNum_;
    }

    //! 터렛 배치 UI를 불가능상태로 변경하는 함수
    private void SetUIDisable()
    {
        // 배치 불가능 상태로 설정
        isPlacable = false;
        placeMaterial.color = cantPlace;
    }

    //! 터렛 배치 UI를 가능상태로 변경하는 함수
    private void SetUIEnable()
    {
        // 배치 가능 상태로 설정
        isPlacable = true;
        placeMaterial.color = canPlace;
    }
}
