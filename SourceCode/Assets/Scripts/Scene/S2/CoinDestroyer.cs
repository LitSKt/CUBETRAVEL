//有掉出箱子的金币需要销毁，挂载在金币预制体上
using UnityEngine;

public class CoinDestroyer : MonoBehaviour
{
    Transform trans;
    Transform cam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trans = transform;
        cam = GameObject.Find("ZeroPoint").transform.Find("Main Camera");
    }

    // Rigidbody
    void FixedUpdate()
    {
        //当金币掉出箱子再判断是否销毁，箱子内的金币不做判断
        if (trans.position.y < -10)
        {
            if (!JudgeVisibleCodeBlock(cam.GetComponent<Camera>(), trans.gameObject))
            {
                Destroy(trans.gameObject);
            }
        }
    }

    //判断物体是否在视锥体内
    bool JudgeVisibleCodeBlock(Camera cam, GameObject obj)
    {
        Bounds bounds = obj.GetComponent<Renderer>().bounds;
        return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(cam), bounds);
    }
}
