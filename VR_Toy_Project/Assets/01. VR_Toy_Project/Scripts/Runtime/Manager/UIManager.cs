using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Variables
    private GameObject HUDCanvas = default;
    private GameObject StartCanvas = default;
    private GameObject startBtn = default; 
    private GameObject quitBtn = default;

    private Slider slider = default;
    private GameObject timerTxt = default;
    private GameObject goldTxt = default;
    private GameObject hpTxt = default;

    private Boss boss = default;
    private PlayerStatus playerStat = default;
    
    // UI에 표시할 텍스트 
    private string timeFormat = default;
    private string hpFormat = default;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }
    
    // Update is called once per frame
    void Update()
    {

    }

    // ! UI 요소 캐싱 및 초기화
    public void Init()
    {
        // { 컴퍼넌트 
        boss = GFunc.GetRootObj("Boss").GetComponent<Boss>();
        playerStat = GFunc.GetRootObj("Player").GetComponent<PlayerStatus>();
        // } 컴퍼넌트 
        
        // { HUD Canvas 
        HUDCanvas = GFunc.GetRootObj("PlayerHUDCanvas");
        StartCanvas = GFunc.GetRootObj("GameStartCanvas");        
        timerTxt = HUDCanvas.GetChildObj("TimerTxt");
        slider = HUDCanvas.GetChildObj("BossHPSlider").GetComponent<Slider>();
        goldTxt = HUDCanvas.GetChildObj("GoldTxt");
        hpTxt = HUDCanvas.GetChildObj("UI_PlayerHP");
        // } HUD Canvas 

        // { Start Canvas
        startBtn = StartCanvas.GetChildObj("StartButton");
        quitBtn = StartCanvas.GetChildObj("QuitButton");

        startBtn.GetComponent<Button>().onClick.AddListener(OnStartButton);
        quitBtn.GetComponent<Button>().onClick.AddListener(OnQuitButtion);
        // } Start Canvas

    }       // Init()

    // ! Player HUD 업데이트 
    public void Update_HUD(float curTime_, int gold_)
    {

        // { 시간을 분 / 초 로 나눔
        int min = (((int)curTime_ / 60));
        int sec = ((int)curTime_ % 60);
        // } 시간을 분 / 초 로 나눔

        // { HUD Text 변경
        timeFormat = string.Format("{0} : {1}", min.ToString("D2"), sec.ToString("D2"));
        timerTxt.SetTmpText(timeFormat);
        goldTxt.SetTmpText(gold_.ToString());

        hpFormat = string.Format("HP " + "{0} / {1}", playerStat.curHp.ToString("D3"), playerStat.maxHp.ToString("D3"));
        hpTxt.SetTmpText(hpFormat);
        // } HUD Text 변경

        slider.value = ((float)boss.CurHP / (float)boss.MaxHp);

    }   // UpdateHUD()

    // ! 게임 시작 버튼 클릭시 동작
    public void OnStartButton()
    {
        bool isStart_ = GameManager.Instance.GameStart();       
        StartCanvas.SetActive(!isStart_);
    }       // OnStartButton()

    // ! 게임 종료 버튼 클릭시 동작
    public void OnQuitButtion()
    {        
        GameManager.Instance.GameQuit();
    }       // OnQuitButtion()

}
