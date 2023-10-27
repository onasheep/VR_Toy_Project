using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public static BossManager instance;

 
    public float gametime = default;
    public float spawn = default;
    public float skillCoolTime = default;
    //게임이 종료되는 시간
    public float EndGame = 420;
    public float Weaknesstime = default;
    public float MonsterAttackTime = default;
    public GameObject Startx = default;
    public GameObject Endx = default;
    private void Awake()
    {
        instance = this;
    }
    private void Update()
    {
        spawn += Time.deltaTime;
        gametime += Time.deltaTime;
        skillCoolTime += Time.deltaTime;
        Weaknesstime += Time.deltaTime;
        MonsterAttackTime += Time.deltaTime;

    }


}
