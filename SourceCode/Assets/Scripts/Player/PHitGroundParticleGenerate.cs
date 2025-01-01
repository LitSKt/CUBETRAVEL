//控制玩家落地产生尘埃粒子的脚本，直接挂载在玩家身上，同时需要3个尘埃粒子预制体
//思路为计算出玩家在y方向的速度(用当前帧的高度减去上一帧的高度后除以帧时间，实际上该方法是在FixedUpdate中调用，所以并非帧之间的高度差)
//根据落地时的速度来判断生成哪一种粒子(大小和密度不同)
using Unity.VisualScripting;
using UnityEngine;

public class PHitGroundParticleGenerate : MonoBehaviour
{
    public GameObject pHitGroundParticleLarge;
    public GameObject pHitGroundParticleMiddle;
    public GameObject pHitGroundParticleSmall;

    Transform trans;

    float lastFrameHeight;
    float currentFrameHeight;
    float verticalVelocity;

    // Start is called before the first frame update
    void Start()
    {
        trans = transform;

        currentFrameHeight = trans.position.y;
    }

    //RigidBody
    //Fixed this: idk why but it actually fixed. When I put this code block into Update function, Unity will get a wrong veloctiy returned every frame interval
    private void FixedUpdate()
    {
        //此处不能使用判断trans.position.y是否小于某个值来判断玩家是否接近地面以此优化性能，因为游戏中可被视为地面的物体海拔不固定
        verticalVelocity = GetVerticalVelocityCodeBlock();
    }

    //计算玩家y方向的速度
    float GetVerticalVelocityCodeBlock()
    {
        lastFrameHeight = currentFrameHeight;
        currentFrameHeight = trans.position.y;
        return (currentFrameHeight - lastFrameHeight) / Time.fixedDeltaTime;
    }

    //生成尘埃粒子的方法
    //Generate particles if player's vertical velocity is as fast as expected
    private void OnCollisionEnter(Collision collision)
    {
        if (verticalVelocity <= -18)
        {
            Instantiate(pHitGroundParticleLarge, trans.position, Quaternion.identity).GetComponent<ParticleSystem>();
        }
        else if (verticalVelocity <= -10)
        {
            Instantiate(pHitGroundParticleMiddle, trans.position, Quaternion.identity).GetComponent<ParticleSystem>();
        }
        else if (verticalVelocity <= -5)
        {
            Instantiate(pHitGroundParticleSmall, trans.position, Quaternion.identity).GetComponent<ParticleSystem>();
        }
    }
}
