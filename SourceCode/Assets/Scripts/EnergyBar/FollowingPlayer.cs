//控制能量环跟随玩家的脚本，挂载在能量环本体
using UnityEngine;

public class FollowingPlayer : MonoBehaviour
{
    public float followLerpSpeed;
    public float upOffset;
    public float rightOffset;

    Transform trans;
    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        trans = transform;
        player = GameObject.Find("ZeroPoint").transform.Find("Player");
        trans.position = Camera.main.WorldToScreenPoint(player.position + upOffset * Vector3.up + rightOffset * Vector3.right);
    }

    // Update is called once per frame
    void Update()
    {
        EnergyBarFollowingCodeBlock();
    }

    //与相机的跟随同理，也用线性插值使跟随时位置变化更平滑
    void EnergyBarFollowingCodeBlock()
    {
        trans.position = Vector2.Lerp(trans.position, Camera.main.WorldToScreenPoint(player.position + upOffset * Vector3.up + rightOffset * Vector3.right), followLerpSpeed * Time.deltaTime);
    }
}
