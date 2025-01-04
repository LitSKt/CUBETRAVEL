//控制第一关旁白的显示，挂载在第一关旁白字幕对象上
using TMPro;
using UnityEngine;

public class S1 : MonoBehaviour
{
    public float displayMultiplier;
    public float destroyTime;

    float sustainTime = 0;
    //用于记录旁白生命周期阶段的trigger
    int trigger;

    Transform scene;
    TextMeshProUGUI tmp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scene = GameObject.Find("ZeroPoint").transform.Find("Scenes");
        tmp = transform.GetComponent<TextMeshProUGUI>();
        tmp.color = new Color(1, 1, 1, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (scene.GetComponent<ScenesChanging>().IsFinishThisScene)
        {
            if (scene.GetComponent<ScenesChanging>().CurrentScene == 0 && trigger == 0)
            {
                DisplayNarratageCodeBlock();
                if (Mathf.Abs(tmp.color.a - 1) <= 0.01f)
                {
                    trigger++;
                }
            }
        }

        DestroyNarratageCodeBlock();
    }

    //渐入显示旁白
    void DisplayNarratageCodeBlock()
    {
        tmp.color = new Color(1, 1, 1, Mathf.Lerp(tmp.color.a, 1, displayMultiplier * Time.deltaTime));
    }

    //当旁白渐出后销毁
    void DestroyNarratageCodeBlock()
    {
        if (trigger == 1)
        {
            sustainTime += Time.deltaTime;
        }
        if (sustainTime >= destroyTime)
        {
            tmp.color = new Color(1, 1, 1, Mathf.Lerp(tmp.color.a, 0, displayMultiplier * Time.deltaTime));
            if (Mathf.Abs(tmp.color.a) <= 0.01f)
            {
                Destroy(transform.gameObject);
            }
        }
    }
}
