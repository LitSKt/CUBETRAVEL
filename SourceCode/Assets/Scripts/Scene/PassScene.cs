//判断玩家是否通关的脚本，F挂载在Scene_1的PassTrigger上
using Unity.VisualScripting;
using UnityEngine;

public class PassScene : MonoBehaviour
{
    Transform scenes;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scenes = GameObject.Find("ZeroPoint").transform.Find("Scenes");
    }

    //当结束碰撞时判断碰撞物体是否是玩家，因为碰撞本身是由玩家模型产生的，所以需要用其父物体判断
    void OnTriggerExit(Collider col)
    {
        if (col.transform.parent != null)
        {
            if (col.transform.parent.name.Equals("Player"))
            {
                scenes.GetComponent<ScenesChanging>().IsFinishThisScene = true;
            }
        }
    }
}
