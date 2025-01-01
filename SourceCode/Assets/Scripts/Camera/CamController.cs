//相机控制脚本
using UnityEngine;

public class CamController : MonoBehaviour
{
    public float camFollowSpeed;

    Transform trans;
    Transform player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trans = transform;
        player = GameObject.Find("ZeroPoint").transform.Find("Player");
    }

    //Rigidbody
    //将相机跟随玩家移动的方法放在FixedUpdate里执行是为了避免玩家身上刚体组件带来的卡顿，刚体的更新总是放在FixedUpdate中
    private void FixedUpdate()
    {
        CamFollowCodeBlock();
    }

    void CamFollowCodeBlock()
    {
        //保持对玩家positon有y和z分别的向上50和向后的50的偏移，并用线性插值使移动更加平滑
        trans.position = Vector3.Lerp(trans.position, new Vector3(player.position.x, player.position.y + 50, player.position.z - 50), camFollowSpeed * Time.deltaTime);
    }
}
