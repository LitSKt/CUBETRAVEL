//控制开始玩家与开始字母互动的脚本，直接挂载在StartLetters上
using UnityEngine;

public class StartLetters : MonoBehaviour
{
    public float forceMultiplier;

    //只会对字母施加一次力的作用，故用此变量限制
    bool haveAddForce;

    Transform trans;
    Transform player;
    Camera cam;
    Transform[] letters;
    Vector3[] lettersOriginalPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trans = transform;
        player = GameObject.Find("ZeroPoint").transform.Find("Player");
        cam = GameObject.Find("ZeroPoint").transform.Find("Main Camera").GetComponent<Camera>();
        letters = new Transform[trans.childCount];
        lettersOriginalPos = new Vector3[letters.Length];

        GetLettersCodeBlock();
    }

    //Rigidbody
    void FixedUpdate()
    {
        if (JudgeGameStartCodeBlock())
        {
            player.GetComponent<PBeforeStart>().IsStarted = true;
        }
        if (player.GetComponent<PBeforeStart>().IsStarted)
        {
            if (!haveAddForce)
            {
                GenerateForceToLettersCodeBlock();
                haveAddForce = true;
            }
            DestoryLettersCodeBlock();
        }
    }

    //获取所有字母对象及其初始位置，存入letters和lettersOriginalPos中
    void GetLettersCodeBlock()
    {
        for (int i = 0; i < letters.Length; ++i)
        {
            letters[i] = trans.GetChild(i);
            lettersOriginalPos[i] = letters[i].localPosition;
        }
    }

    //当所有字母都产生位移后开始游戏
    bool JudgeGameStartCodeBlock()
    {
        int haveOffsetLetterNum = 0;
        for (int i = 0; i < letters.Length; ++i)
        {
            //这里不能用Equals，小数会有误判
            if ((letters[i].localPosition - lettersOriginalPos[i]).magnitude > 0.001f)
            {
                haveOffsetLetterNum++;
            }
        }
        if (haveOffsetLetterNum == letters.Length)
        {
            return true;
        }
        return false;
    }

    //在游戏开始后立马向字母施加一个方向随机的力(不向下)，且只执行一次，为让字母有自然的旋转，施力点有向上偏移1
    void GenerateForceToLettersCodeBlock()
    {
        for (int i = 0; i < letters.Length; ++i)
        {
            Vector3 forceDir = new(Random.Range(-1f, 1f), Random.Range(0f, 1f), Random.Range(-1f, 1f));
            letters[i].GetComponent<Rigidbody>().AddForceAtPosition(forceMultiplier * forceDir.normalized, letters[i].position + Vector3.up, ForceMode.Force);
        }
    }

    //当所有字母都在视锥外时消除StartLetters游戏对象，减少刚体运动的运算量，优化作用
    void DestoryLettersCodeBlock()
    {
        int outCamNum = 0;
        for (int i = 0; i < letters.Length; ++i)
        {
            if (!JudgeObjectVisibleCodeBlock(cam, letters[i].gameObject))
            {
                outCamNum++;
            }
        }
        if (outCamNum == letters.Length)
        {
            Destroy(trans.gameObject);
        }
    }

    //判断物体(在当前脚本即为字母)是否在视锥内
    bool JudgeObjectVisibleCodeBlock(Camera cam, GameObject obj)
    {
        Bounds bounds = obj.GetComponent<Renderer>().bounds;
        return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(cam), bounds);
    }
}
