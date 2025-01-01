//玩家移动控制核心脚本，直接挂载在玩家身上
using UnityEngine;
using UnityEngine.InputSystem;

public class PController : MonoBehaviour
{
    public float moveForceMultiplier;
    public float moveYForceMultiplier;
    public float rotateForceMultiplier;
    public float torque;

    Transform trans;
    Rigidbody rig;

    Vector3 moveForceDir;

    int effectingForceNum;

    //make sure effecting force only count once when move callbackcontext is performing
    int xNum, yNum, zNum;

    bool isMoving;
    bool rotateLeft;
    bool rotateRight;

    //does have force effect, including move and rotate
    bool isPushing;

    public bool IsPushing { get => isPushing; set => isPushing = value; }
    public int EffectingForceNum { get => effectingForceNum; set => effectingForceNum = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trans = transform;
        rig = trans.GetComponent<Rigidbody>();

        EffectingForceNum = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (trans.GetComponent<PBeforeStart>().IsStarted)
        {
            //判断玩家是否正在对物体施力
            IsPushing = DetermineIsPushingCodeBlock();
        }
    }

    //Rigidbody
    private void FixedUpdate()
    {
        //在游戏开始后使用带体力判断的移动
        if (trans.GetComponent<PBeforeStart>().IsStarted)
        {
            //当体力值大于0时玩家的移动或旋转操作才可以奏效
            if (trans.GetComponent<PStrength>().CurrentStrength > 0)
            {
                MoveCodeBlock(moveForceMultiplier, moveYForceMultiplier);

                RotateCodeBlock(rotateForceMultiplier);
            }
        }
        else
        {
            MoveCodeBlock(moveForceMultiplier * 0.1f, moveYForceMultiplier * 0.1f);
            RotateCodeBlock(rotateForceMultiplier * 0.1f);
        }
    }

    //Code blocks
    //该移动方法的移动方向参考世界坐标系
    void MoveCodeBlock(float mFM, float mYFM)
    {
        //moveFD由InputSystem读入，可理解为键盘的输入，即当时是否按下某个移动按键
        //moveFM为玩家平面移动时的移动倍数，public
        //moveYFM为玩家垂直移动时的移动倍数，public
        rig.AddForce(mFM * moveForceDir.x * Vector3.right.normalized);
        rig.AddForce(mFM * moveForceDir.z * Vector3.forward.normalized);
        rig.AddForce(mYFM * moveForceDir.y * Vector3.up.normalized);
    }

    //该移动方法的移动方法参考玩家的模型坐标系，会使操作难度直线上升，故称反人类的移动方法
    void AntiHumanityMoveCodeBlock(float mFM, float mYFM)
    {
        rig.AddForce(mFM * moveForceDir.x * trans.right.normalized);
        rig.AddForce(mFM * moveForceDir.z * trans.forward.normalized);
        rig.AddForce(mYFM * moveForceDir.y * trans.up.normalized);
    }

    //控制玩家旋转，思路为以torque为力矩施力
    void RotateCodeBlock(float rFM)
    {
        if (rotateLeft)
        {
            rig.AddForceAtPosition(rFM * Vector3.forward, trans.position + torque * Vector3.right, ForceMode.Force);
        }
        else if (rotateRight)
        {
            rig.AddForceAtPosition(-rFM * Vector3.forward, trans.position + torque * Vector3.right, ForceMode.Force);
        }
    }

    //返回一个布尔值，用于判断玩家当前是否在操控物体移动或旋转
    bool DetermineIsPushingCodeBlock()
    {
        if (!isMoving && !rotateLeft && !rotateRight)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //Input functions
    public void Move(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            isMoving = true;
        }
        else if (ctx.performed)
        {
            moveForceDir = ctx.ReadValue<Vector3>();

            //当检测到玩家有按下x方向按键则记录一次EFN，同时xNum++
            //当松开x方向按键时则会进入else，当判断xNum!=0即之前进入过if中时，应消除EFN的记录，因为此时已经没有在x方向有按下按键
            //y，z同理
            //counting effecting force number for the strength calculation
            if (moveForceDir.x != 0)
            {
                if (xNum == 0)
                {
                    EffectingForceNum++;
                    xNum++;
                }
            }
            else
            {
                //decrease effecting force number after the key is released, so does below
                if (xNum != 0)
                {
                    if (EffectingForceNum > 1)
                    {
                        EffectingForceNum--;
                    }
                }
                xNum = 0;
            }

            if (moveForceDir.y != 0)
            {
                if (yNum == 0)
                {
                    EffectingForceNum++;
                    yNum++;
                }
            }
            else
            {
                if (yNum != 0)
                {
                    if (EffectingForceNum > 1)
                    {
                        EffectingForceNum--;
                    }
                }
                yNum = 0;
            }

            if (moveForceDir.z != 0)
            {
                if (zNum == 0)
                {
                    EffectingForceNum++;
                    zNum++;
                }
            }
            else
            {
                if (zNum != 0)
                {
                    if (EffectingForceNum > 1)
                    {
                        EffectingForceNum--;
                    }
                }
                zNum = 0;
            }
        }
        else if (ctx.canceled)
        {
            //当所有方向键都松开时，归零moveFD，布尔值isM也设为false，该项会决定布尔变量isPushing的值
            moveForceDir = Vector3.zero;
            isMoving = false;

            //此处不能直接令EFN=0，因为EFN同时还会记录旋转的施力个数，而旋转的输入事件方法与此不同
            if (zNum != 0)
            {
                if (EffectingForceNum > 1)
                {
                    EffectingForceNum--;
                }
            }
            if (yNum != 0)
            {
                if (EffectingForceNum > 1)
                {
                    EffectingForceNum--;
                }
            }
            if (xNum != 0)
            {
                if (EffectingForceNum > 1)
                {
                    EffectingForceNum--;
                }
            }

            xNum = 0;
            yNum = 0;
            zNum = 0;
        }
    }

    //旋转同理，见上
    public void RotateLeft(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            EffectingForceNum++;
            rotateLeft = true;
        }
        else if (ctx.canceled)
        {
            rotateLeft = false;

            if (EffectingForceNum > 1)
            {
                EffectingForceNum--;
            }
        }
    }

    public void RotateRight(InputAction.CallbackContext ctx)
    {
        if (ctx.started)
        {
            EffectingForceNum++;
            rotateRight = true;
        }
        else if (ctx.canceled)
        {
            rotateRight = false;

            if (EffectingForceNum > 1)
            {
                EffectingForceNum--;
            }
        }
    }
}
