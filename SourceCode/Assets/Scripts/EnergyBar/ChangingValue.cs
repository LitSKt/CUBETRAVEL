//控制能量环数值的脚本，挂载在能量环前景上
using UnityEngine;
using UnityEngine.UI;

public class ChangingValue : MonoBehaviour
{
    public float lerpSpeed;

    Transform trans;
    Transform player;
    Image image;

    float currentImageStrengthValue;

    public float CurrentImageStrengthValue { get => currentImageStrengthValue; set => currentImageStrengthValue = value; }

    // Start is called before the first frame update
    void Start()
    {
        trans = transform;
        image = trans.GetComponent<Image>();
        player = GameObject.Find("ZeroPoint").transform.Find("Player");

        //能量环当前数值为一个百分比，即玩家当前能量与玩家最大能量的比值
        CurrentImageStrengthValue = player.GetComponent<PStrength>().CurrentStrength / player.GetComponent<PStrength>().maxStrength;
    }

    // Update is called once per frame
    void Update()
    {
        ChangingValueCodeBlock();
    }

    void ChangingValueCodeBlock()
    {
        //用线性插值使数值变化更加平滑
        CurrentImageStrengthValue = Mathf.Lerp(CurrentImageStrengthValue, player.GetComponent<PStrength>().CurrentStrength / player.GetComponent<PStrength>().maxStrength, lerpSpeed * Time.deltaTime);
        //同时将百分比数值赋给fillAmount，使能量环前景对应数值发生变化
        image.fillAmount = CurrentImageStrengthValue;
    }
}
