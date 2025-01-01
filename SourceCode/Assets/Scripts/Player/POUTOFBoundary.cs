//实现玩家掉出地图后再次落到地图上的循环，直接挂载在玩家身上
using UnityEngine;

public class POUTOFBoundary : MonoBehaviour
{
    public float respawnHeight;
    public float outOfBoundaryHeight;

    Transform trans;
    Transform cam;
    Transform scenes;

    bool isOutOfBoundary;
    //改变条件与isOutOfB一致，用于判断是否可以切换关卡地形，因为无法直接使用isOutOfB
    //其他脚本无法得到isOutOfB的真值，一旦其为真，其会在此脚本立马被赋值为假
    bool canPlayerSeeScene;

    public bool CanPlayerSeeScene { get => canPlayerSeeScene; set => canPlayerSeeScene = value; }

    // Start is called before the first frame update
    void Start()
    {
        trans = transform;
        cam = GameObject.Find("ZeroPoint").transform.Find("Main Camera");
        scenes = GameObject.Find("ZeroPoint").transform.Find("Scenes");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        DetermineOUTOFBoundaryCodeBlock();
    }

    void DetermineOUTOFBoundaryCodeBlock()
    {
        if (trans.position.y <= outOfBoundaryHeight)
        {
            isOutOfBoundary = true;
            if (scenes.GetComponent<ScenesChanging>().IsFinishThisScene)
            {
                canPlayerSeeScene = true;
            }
        }

        //当玩家落出地图后先获得当前相机相对于玩家的位置，再重置玩家和相机的位置，且相机保持原有相对位置，可以保证真实性
        if (isOutOfBoundary)
        {
            //R: relative
            float camRPlayerX = cam.position.x - trans.position.x;
            float camRPlayerY = cam.position.y - trans.position.y;
            float camRPlayerZ = cam.position.z - trans.position.z;

            trans.position = Vector3.zero + respawnHeight * Vector3.up;
            cam.position = new Vector3(trans.position.x + camRPlayerX, trans.position.y + camRPlayerY, trans.position.z + camRPlayerZ);
            isOutOfBoundary = false;
        }
    }
}
