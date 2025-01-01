//控制显示退出字样的脚本，直接挂载在QuitLetters上
using System.Security;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuitLetters : MonoBehaviour
{
    struct Letter
    {
        public Transform letter;
        public bool isForced;
    };

    public float forceMultiplier;

    bool isDestoryLetter;
    int indexOfCurrentLetter;

    Transform trans;
    Transform cam;
    Letter[] letters;

    public bool IsDestoryLetter { get => isDestoryLetter; set => isDestoryLetter = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        indexOfCurrentLetter = 0;

        trans = transform;
        cam = GameObject.Find("ZeroPoint").transform.Find("Main Camera");
        letters = new Letter[trans.childCount];

        GetLettersCodeBlock();
    }

    // Rigidbody
    void FixedUpdate()
    {
        DisActiveLettersCodeBlock();

        DestroyLettersCodeBlock();

        FollowCamCodeBlock();
    }

    //获取所有字母
    void GetLettersCodeBlock()
    {
        for (int i = 0; i < letters.Length; ++i)
        {
            letters[i].letter = trans.GetChild(i);
        }
    }

    //时刻跟随相机位置
    void FollowCamCodeBlock()
    {
        trans.position = cam.position + new Vector3(-1.8f, -46, -56);
    }

    //当所有字母都在视锥外时删除QuitLetters游戏对象，减少刚体运动的运算量，优化作用
    void DisActiveLettersCodeBlock()
    {
        //如果关闭菜单，则不管字母有没有被施力过，都计入超出视锥体的判定
        if (!cam.GetComponent<ESCMenu>().IsCallingMenu)
        {
            int outCamNum = 0;
            for (int i = 0; i < letters.Length; ++i)
            {
                if (!JudgeObjectVisibleCodeBlock(cam.GetComponent<Camera>(), letters[i].letter.gameObject))
                {
                    outCamNum++;
                }
            }
            if (outCamNum == letters.Length)
            {
                Destroy(trans.gameObject);
            }
        }
        //如果还在菜单中，字母超出视锥体有可能是被其他字母撞飞的，所以不计入判定，否则在所有字母都被撞飞后会直接重新生成字母，
        //不但突兀，且DestroyLetterCodeBlock()中记录的摧毁字母数并不会变，所以新生成的字母不会被全部摧毁游戏就会退出
        else
        {
            int outCamNum = 0;
            for (int i = 0; i < letters.Length; ++i)
            {
                if (!JudgeObjectVisibleCodeBlock(cam.GetComponent<Camera>(), letters[i].letter.gameObject) && letters[i].isForced)
                {
                    outCamNum++;
                }
            }
            if (outCamNum == letters.Length)
            {
                Destroy(trans.gameObject);
            }
        }
    }

    //摧毁字母动画代码块(按下按键后对字母施力，让他飞起来),并且检测是否可以退出游戏(字母是不是都飞起来了)
    void DestroyLettersCodeBlock()
    {
        if (isDestoryLetter && indexOfCurrentLetter < letters.Length)
        {
            Vector3 forceDir = new(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f));
            letters[indexOfCurrentLetter].letter.GetComponent<Rigidbody>().AddForceAtPosition(forceMultiplier * forceDir.normalized, letters[indexOfCurrentLetter].letter.position + Vector3.up, ForceMode.Force);
            letters[indexOfCurrentLetter].isForced = true;
            indexOfCurrentLetter++;
            isDestoryLetter = false;
        }
        if (indexOfCurrentLetter == letters.Length)
        {
            cam.GetComponent<ESCMenu>().IsQuit = true;
        }
    }

    //判断物体(在当前脚本即为字母)是否在视锥内
    bool JudgeObjectVisibleCodeBlock(Camera cam, GameObject obj)
    {
        Bounds bounds = obj.GetComponent<Renderer>().bounds;
        return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(cam), bounds);
    }
}
