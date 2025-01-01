//控制地图关卡切换的脚本，直接挂载在Scenes上，地图关卡为Scenes的子物体
using UnityEngine;

public class ScenesChanging : MonoBehaviour
{
    int currentScene;
    bool isFinishThisScene;

    Transform trans;
    Transform player;
    GameObject[] scenes;

    public bool IsFinishThisScene { get => isFinishThisScene; set => isFinishThisScene = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trans = transform;
        player = GameObject.Find("ZeroPoint").transform.Find("Player");

        currentScene = 0;
        scenes = new GameObject[trans.childCount];

        //在游戏开始前先获取所有场景
        GetChildrenCodeBlock();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //游戏开始后执行
        if (player.GetComponent<PBeforeStart>().IsStarted)
        {
            //如果完成当前关卡且玩家已经出界(看不见地面)，则切换关卡
            if (isFinishThisScene && player.GetComponent<POUTOFBoundary>().CanPlayerSeeScene)
            {
                ChangeSceneCodeBlock();
                isFinishThisScene = false;
                player.GetComponent<POUTOFBoundary>().CanPlayerSeeScene = false;
            }
        }
    }

    //获取所有关卡并存入scenes数组
    void GetChildrenCodeBlock()
    {
        for (int i = 0; i < scenes.Length; ++i)
        {
            scenes[i] = trans.GetChild(i).gameObject;
        }
    }

    //如果玩家通过当前关卡，则切换下一关卡
    void ChangeSceneCodeBlock()
    {
        //实现关卡循环，暂定循环
        currentScene = (currentScene + 1) % scenes.Length;

        for (int i = 0; i < scenes.Length; ++i)
        {
            if (i == currentScene)
            {
                scenes[i].SetActive(true);
            }
            else
            {
                scenes[i].SetActive(false);
            }
        }
    }
}
