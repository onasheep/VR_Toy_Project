using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;
using System.Collections;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager instance_ = default;

    public static GameManager Instance
    {
        get
        {
            if (instance_ == null || instance_ == default)
            {
                instance_ = GFunc.GetRootObj("GameManager").GetComponent<GameManager>();
            }

            return instance_;
        }
    }
    #endregion

    #region Variables
    // { 캐싱을 위한 컴포너트들
    private UIManager uiManager = default;
    private PostProcessVolume postProcessV = default;
    private GameObject objectPoolContainer = default;
    // { 캐싱을 위한 컴포너트들

    [Header("Turret List")]
    [Space]
    // { 터렛을 관리할 리스트 
    public List<Transform> turretLv1_List = default;
    public List<Transform> turretLv2_List = default;
    public List<Transform> turretLv3_List = default;
    public List<Transform> turretLv4_List = default;
    // } 터렛을 관리할 리스트 

    [Header("Gamecycle Boolean")]
    [Space]
    // { 게임 사이클 변수
    public bool isStart = true;
    public bool isEnd = default;
    // } 게임 사이클 변수

    // { HUD 변수
    public float EndTime { get; private set; }
    public float CurTime { get; private set; }
    public int Gold { get; private set; }
    private float goldTime = default;
    // } HUD 변수
    #endregion

    private void Awake()
    {
        Init();
    }


    // Start is called before the first frame update
    void Start()
    {
        //BSJ_ 메인 BGM 시작
        SoundManager.instance.PlayBGM("MainBGM");

        Time.timeScale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if (isStart == default || isStart == false) { return; }


        uiManager.Update_HUD(CurTime, Gold);

        Timer();

    }       // Update()


    private void Init()
    {
        uiManager = GFunc.GetRootObj("UIManager").GetComponent<UIManager>();
        postProcessV = Camera.main.GetComponent<PostProcessVolume>();
        objectPoolContainer = GFunc.GetRootObj("ObjPoolContainer");
        isStart = false;
        isEnd = false;

        EndTime = 420f;
        CurTime = 0f;

        // { JSH 리스트 할당
        turretLv1_List = new List<Transform>();
        turretLv2_List = new List<Transform>();
        turretLv3_List = new List<Transform>();
        turretLv4_List = new List<Transform>();
        // } JSH 리스트 할당

        // 초기 골드 
        Gold = 500;
    }       // Init()

    // ! 글로벌로 사용할 타이머, 시간에 따른 골드 수급
    private void Timer()
    {
        if (CurTime >= EndTime)
        {
            isEnd = true;
            isStart = false;
        }

        CurTime += Time.deltaTime;
        GetGold_Time();
    }       // Timer()

    public IEnumerator EffectCamera()
    {
        postProcessV.enabled = true;
        yield return new WaitForSeconds(1f);
        postProcessV.enabled = false;
    }

    // ! 1초당 획득 골드
    private void GetGold_Time()
    {
        goldTime += Time.deltaTime;
        if (goldTime >= 1f)
        {
            goldTime -= 1f;
            Gold += JsonData.Instance.economyDatas.Economy[1].Gold_Gain;
        }
    }       // GetGold_Time()

    // ! 졸개 처치 시 골드 획득   
    public void GetGold_Monster()
    {
        Gold += JsonData.Instance.economyDatas.Economy[3].Gold_Gain;
    }       // GetGold_Monster();

    // ! 보스 체력 비율에 따른 골드 획득
    public void GetGold_Boss()
    {
        Gold += JsonData.Instance.economyDatas.Economy[2].Gold_Gain;
    }       // GetGold_Boss()

    public bool GameStart()
    {
        isStart = true;
        Time.timeScale = 1f;
        return isStart;
    }       // GameStart()

    public void WinGame()
    {
        BSJVRInput.PlayVibration(0f, 0f, 0, BSJVRInput.Controller.RTouch);
        Time.timeScale = 0f;
        uiManager.ChangeUI_GameWin();
    }

    public void LoseGame()
    {
        BSJVRInput.PlayVibration(0f, 0f, 0, BSJVRInput.Controller.RTouch);
        objectPoolContainer.SetActive(false);
        Time.timeScale = 0f;
        uiManager.ChangeUI_GameOver();
    }

    public void ReStart()
    {
        // LEGACY : 
        //Init();

        // HSJ_ 로드 할 씬
        SceneManager.LoadScene(0);
        
    }       // ReStart()

    public void GameQuit()
    {
#if UNITY_STANDALONE
        Application.Quit();
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }       // QameQuit()

    //! JSH: 재화 사용 함수
    public void UseGold(int cost_)
    {
        // 보유 재화에서 코스트만큼 차감
        Gold -= cost_;
    }

    //! JSH: 재화 사용 가능 여부 반환 함수
    public bool CanUseGold(int cost_)
    {
        // 코스트가 보유 재화와 같거나 작음
        if (cost_ <= Gold) return true;
        // 코스트가 보유 재화보다 큼
        return false;
    }
}
