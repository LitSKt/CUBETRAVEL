//控制开始界面的玩家，直接挂载在Player身上
//因为想做一镜到底，所以只用了一个场景，所以开始界面也做在了地图上方
using UnityEngine;
using UnityEngine.InputSystem;

public class PBeforeStart : MonoBehaviour
{
    bool isStarted;

    Transform trans;
    Transform cam;
    Transform startLetters;
    Rigidbody rig;

    public bool IsStarted { get => isStarted; set => isStarted = value; }

    //在游戏开始之前先禁用大部分对象和组件，等游戏开始以后再启用
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //获得对象和组件
        trans = transform;
        cam = GameObject.Find("ZeroPoint").transform.Find("Main Camera");
        startLetters = GameObject.Find("ZeroPoint").transform.Find("StartLetters");
        rig = trans.GetComponent<Rigidbody>();

        //设置位置
        trans.position = new Vector3(trans.position.x, trans.GetComponent<POUTOFBoundary>().RespawnHeight, trans.position.z);
        cam.position = trans.position + new Vector3(0, 50, -50);
        startLetters.gameObject.SetActive(true);
        startLetters.position = cam.position + new Vector3(-0.1f, -46, 56);

        //禁用对象和组件
        trans.GetComponent<PStrength>().enabled = false;
        GameObject.Find("ZeroPoint").transform.Find("Scenes").gameObject.SetActive(false);
        rig.useGravity = false;
    }

    //Rigidbody
    void FixedUpdate()
    {
        if (isStarted)
        {
            AwakingGameCodeBlock();
        }
    }

    void AwakingGameCodeBlock()
    {
        trans.GetComponent<PStrength>().enabled = true;
        GameObject.Find("ZeroPoint").transform.Find("Scenes").gameObject.SetActive(true);
        rig.useGravity = true;
        trans.GetComponent<PBeforeStart>().enabled = false;
    }

    //按下开始键(Enter)时开始游戏，前提:没有唤出ESC面板
    public void StartGame(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (!cam.GetComponent<ESCMenu>().IsCallingMenu)
            {
                isStarted = true;
            }
        }
    }
}
