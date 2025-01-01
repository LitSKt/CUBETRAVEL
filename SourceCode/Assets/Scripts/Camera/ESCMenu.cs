//控制游戏弹出ESC菜单的脚本，直接挂载在相机身上
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ESCMenu : MonoBehaviour
{
    public float camRotSpeedMultiplier;
    public GameObject quitLettersPerfab;

    bool isCallingMenu;
    bool isQuit;

    Transform trans;
    Transform player;

    public bool IsCallingMenu { get => isCallingMenu; set => isCallingMenu = value; }

    public bool IsQuit { get => isQuit; set => isQuit = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trans = transform;
        player = GameObject.Find("ZeroPoint").transform.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        MenuCodeBlock();

        QuitCodeBlock();
    }

    //显示ESC面板的代码，同样包含相机转向面板和暂停玩家操作
    void MenuCodeBlock()
    {
        if (isCallingMenu)
        {
            trans.rotation = Quaternion.Slerp(trans.rotation, Quaternion.Euler(36, -180, 0), camRotSpeedMultiplier * Time.deltaTime);
            player.GetComponent<PController>().enabled = false;
            player.GetComponent<PStrength>().enabled = false;
            player.GetComponent<PBeforeStart>().enabled = false;
            if (GameObject.Find("QuitLetters(Clone)") == null)
            {
                GameObject quitLetter = Instantiate(quitLettersPerfab);
                quitLetter.transform.position = trans.position + new Vector3(-1.8f, -46, -56);
            }
        }
        else
        {
            trans.rotation = Quaternion.Slerp(trans.rotation, Quaternion.Euler(36, 0, 0), camRotSpeedMultiplier * Time.deltaTime);
            player.GetComponent<PController>().enabled = true;
            player.GetComponent<PStrength>().enabled = true;
            player.GetComponent<PBeforeStart>().enabled = true;
        }
    }

    void QuitCodeBlock()
    {
        if (isQuit)
        {
            Application.Quit();//在可执行程序中结束运行
        }
    }

    //ESC按键代码
    public void ESC(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (isCallingMenu)
            {
                isCallingMenu = false;
            }
            else
            {
                isCallingMenu = true;
            }
        }
    }

    //退出按键代码块(连按4下Enter，消除所有字母便可退出)
    public void Quit(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            if (isCallingMenu)
            {
                GameObject.Find("QuitLetters(Clone)").GetComponent<QuitLetters>().IsDestoryLetter = true;
            }
        }
    }
}
