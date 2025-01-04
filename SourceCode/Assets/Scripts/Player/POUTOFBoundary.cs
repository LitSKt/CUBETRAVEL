//实现玩家掉出地图后再次落到地图上的循环，直接挂载在玩家身上
using UnityEngine;

public class POUTOFBoundary : MonoBehaviour
{
    [Tooltip("Camera's Upper Limit. 200~500")]
    public int binaryUpperLimit;
    public int binaryLowerLimit;

    Transform trans;
    Transform cam;
    Transform scenes;
    GameObject[] scenesChild;

    bool isOutOfBoundary;
    //改变条件与isOutOfB一致，用于判断是否可以切换关卡地形，因为无法直接使用isOutOfB
    //其他脚本无法得到isOutOfB的真值，一旦其为真，其会在此脚本立马被赋值为假
    bool canPlayerSeeScene = true;
    float respawnHeight;
    int activeScene;

    public bool CanPlayerSeeScene { get => canPlayerSeeScene; set => canPlayerSeeScene = value; }

    public float RespawnHeight { get => respawnHeight; set => respawnHeight = value; }

    // Start is called before the first frame update
    void Start()
    {
        trans = transform;
        cam = GameObject.Find("ZeroPoint").transform.Find("Main Camera");
        scenes = GameObject.Find("ZeroPoint").transform.Find("Scenes");
        scenesChild = new GameObject[scenes.childCount];

        GetScenesCodeBlock();

        activeScene = 0;

        InitializeRHCodeBlock(scenesChild[activeScene]);
    }

    // Rigidbody
    void FixedUpdate()
    {
        GetActiveSceneCodeBlock();

        DetermineOUTOFBoundaryCodeBlock();
    }

    //获得所有场景
    void GetScenesCodeBlock()
    {
        for (int i = 0; i < scenesChild.Length; ++i)
        {
            scenesChild[i] = scenes.GetChild(i).gameObject;
        }
    }

    //获得当前激活的场景
    void GetActiveSceneCodeBlock()
    {
        for (int i = 0; i < scenesChild.Length; ++i)
        {
            if (scenesChild[i].activeSelf)
            {
                activeScene = i;
                break;
            }
        }
    }

    //判断玩家是否掉出地图，并将其传送至上方
    void DetermineOUTOFBoundaryCodeBlock()
    {
        if (trans.position.y < 0)
        {
            if (!JudgeObjectVisibleCodeBlock(cam.GetComponent<Camera>(), scenesChild[activeScene]))
            {
                isOutOfBoundary = true;
                if (scenes.GetComponent<ScenesChanging>().IsFinishThisScene)
                {
                    canPlayerSeeScene = false;
                }
            }
        }

        //当玩家落出地图后先获得当前相机相对于玩家的位置，再重置玩家和相机的位置，且相机保持原有相对位置，可以保证真实性
        if (isOutOfBoundary)
        {
            //R: relative
            float camRPlayerX = cam.position.x - trans.position.x;
            float camRPlayerY = cam.position.y - trans.position.y;
            float camRPlayerZ = cam.position.z - trans.position.z;

            if (scenes.GetComponent<ScenesChanging>().IsFinishThisScene)
            {
                InitializeRHCodeBlock(scenesChild[(activeScene + 1) % scenesChild.Length]);
            }
            else
            {
                InitializeRHCodeBlock(scenesChild[activeScene]);
            }

            trans.position = Vector3.zero + respawnHeight * Vector3.up;
            cam.position = new Vector3(trans.position.x + camRPlayerX, trans.position.y + camRPlayerY, trans.position.z + camRPlayerZ);
            isOutOfBoundary = false;
        }
    }

    //判断物体(包括子物体)是否在视锥体内的代码块
    bool JudgeObjectVisibleCodeBlock(Camera cam, GameObject obj)
    {
        bool isVisible = false;
        if (obj.transform.childCount != 0)
        {
            for (int i = 0; i < obj.transform.childCount; ++i)
            {
                Bounds bounds = obj.transform.GetChild(i).GetComponent<Renderer>().bounds;
                if (GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(cam), bounds))
                {
                    isVisible = true;
                    break;
                }
            }
        }
        else
        {
            Bounds bounds = obj.GetComponent<Renderer>().bounds;
            return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(cam), bounds);
        }
        return isVisible;
    }

    //用二分法算出玩家应该在多高处重生，二分注意mid+1(或者mid-1)，不然会陷入死循环
    void InitializeRHCodeBlock(GameObject referenceScene)
    {
        int upperLimit = binaryUpperLimit;
        int lowerLimit = binaryLowerLimit;
        //R: relative
        float camRPlayerX = cam.position.x - trans.position.x;
        float camRPlayerY = cam.position.y - trans.position.y;
        float camRPlayerZ = cam.position.z - trans.position.z;
        while (upperLimit > lowerLimit)
        {
            int mid = (upperLimit + lowerLimit) / 2;
            cam.transform.position = new Vector3(camRPlayerX, mid + camRPlayerY, camRPlayerZ);
            if (JudgeObjectVisibleCodeBlock(cam.GetComponent<Camera>(), referenceScene))
            {
                lowerLimit = mid + 1;
            }
            else
            {
                upperLimit = mid - 1;
            }
        }
        respawnHeight = upperLimit;
    }
}
