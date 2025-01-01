//控制能量环淡入淡出的脚本，挂载在能量环本体上
using UnityEngine;
using UnityEngine.UI;

public class FadeInAndOut : MonoBehaviour
{
    public float fadeInSpeed;
    public float fadeInLerpSpeed;
    public float fadeOutSpeed;
    public float fadeOutLerpSpeed;
    [Tooltip("Lower value makes energy loop fade when it has a more stable value. P.S. Lower value makes it fade slower. Recommended value: 0.03 ~ 0.6")]
    public float fadeThresholdValue;

    Transform trans;
    Transform frontGround;

    Image background;
    Image foreground;

    float lastFrameStrength;
    float currentFrameStrength;

    float fadeAlpha;

    // Start is called before the first frame update
    void Start()
    {
        trans = transform;
        frontGround = trans.Find("Foreground").transform;

        background = trans.Find("Background").GetComponent<Image>();
        foreground = frontGround.GetComponent<Image>();

        currentFrameStrength = frontGround.GetComponent<ChangingValue>().CurrentImageStrengthValue;
    }

    // Update is called once per frame
    void Update()
    {
        FadingCodeBlock();
    }

    //此代码块用以判断前一帧与当前帧的能量百分比数值是否相同(差值小于阈值)
    //若是，则表明此时可以淡出，返回true;反之亦然
    bool DetermineIfFadeOutCodeBlock()
    {
        lastFrameStrength = currentFrameStrength;
        currentFrameStrength = frontGround.GetComponent<ChangingValue>().CurrentImageStrengthValue;

        //if last frame strength is equal to current one then ready to fade
        if (Mathf.Abs(lastFrameStrength - currentFrameStrength) * 100 < fadeThresholdValue)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void FadingCodeBlock()
    {
        //fade out if strength value is not changing
        if (DetermineIfFadeOutCodeBlock())
        {
            //若当前透明度未达到目标透明度(还未到0)，则平滑减小透明度;下方同理
            if (fadeAlpha - fadeOutSpeed * Time.deltaTime > 0)
            {
                fadeAlpha = Mathf.Lerp(fadeAlpha, fadeAlpha - fadeOutSpeed, fadeOutLerpSpeed * Time.deltaTime);
            }
            //若已经是0(或变化后会小于0)，那就保持为0，即表现为不显示;下方同理
            else
            {
                fadeAlpha = Mathf.Lerp(fadeAlpha, 0, fadeOutLerpSpeed * Time.deltaTime);
            }
        }
        //fade in if strength value is changing
        else
        {
            if (fadeAlpha + fadeInSpeed * Time.deltaTime < 1)
            {
                fadeAlpha = Mathf.Lerp(fadeAlpha, fadeAlpha + fadeInSpeed, fadeInLerpSpeed * Time.deltaTime);
            }
            else
            {
                fadeAlpha = Mathf.Lerp(fadeAlpha, 1, fadeInLerpSpeed * Time.deltaTime);
            }
        }
        //将fadeAlpha赋给color，从而在在游戏中实现淡入淡出
        background.color = new Color(1, 1, 1, fadeAlpha);
        foreground.color = new Color(1, 1, 1, fadeAlpha);
    }
}
