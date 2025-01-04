//控制玩家体力值的脚本，直接挂载在玩家身上
using UnityEngine;

public class PStrength : MonoBehaviour
{
    public float recoverMultiplier;
    public float maxStrength;
    public float maxStrengthChangingSpeed;
    public float speedChangingAcceleration;

    Transform trans;

    float currentStrength;
    //current speed changing acceleration
    float currentSCA;
    //current strength changing speed
    float currentSCS;

    //CSCS: current strength changing speed
    bool resetCSCS;

    public float CurrentStrength { get => currentStrength; set => currentStrength = value; }

    // Start is called before the first frame update
    void Start()
    {
        trans = transform;

        CurrentStrength = maxStrength;
    }

    // Update is called once per frame
    void Update()
    {
        StrengthChangingCodeBlock();
    }

    void StrengthChangingCodeBlock()
    {
        //当前体力变化速度的加速度由施力数决定，所以如果体力消耗的越快，那恢复时也会更快
        currentSCA = trans.GetComponent<PController>().EffectingForceNum * speedChangingAcceleration;

        if (trans.GetComponent<PController>().IsPushing)
        {
            //当状态从未被施力转换为施力时，应重置当前体力变化速度，否则体力消耗的初速度将不正常(可能会很大)
            //reset current strength chaning speed after switch recover strength to decrease strength, so does below (not pushing case)
            if (!resetCSCS)
            {
                resetCSCS = true;
                currentSCS = 0;
            }

            if (currentSCS + currentSCA * Time.deltaTime < maxStrengthChangingSpeed)
            {
                currentSCS += currentSCA * Time.deltaTime;
            }
            else
            {
                currentSCS = maxStrengthChangingSpeed;
            }

            if (CurrentStrength - currentSCS * Time.deltaTime > 0)
            {
                CurrentStrength -= currentSCS * Time.deltaTime;
            }
            else
            {
                CurrentStrength = 0;
            }
        }
        else
        {
            //状态从施力变为未被施力，同上
            if (resetCSCS)
            {
                resetCSCS = false;
                currentSCS = 0;
            }

            //体力的恢复会比消耗时更快
            //strength recover speed is faster than consume
            if (currentSCS + recoverMultiplier * currentSCA * Time.deltaTime < maxStrengthChangingSpeed)
            {
                currentSCS += recoverMultiplier * currentSCA * Time.deltaTime;
            }
            else
            {
                currentSCS = maxStrengthChangingSpeed;
            }

            if (CurrentStrength + recoverMultiplier * currentSCS * Time.deltaTime < maxStrength)
            {
                CurrentStrength += recoverMultiplier * currentSCS * Time.deltaTime;
            }
            else
            {
                CurrentStrength = maxStrength;
            }
        }
    }
}
