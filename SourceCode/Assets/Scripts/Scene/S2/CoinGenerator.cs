using UnityEngine;

public class CoinGenerator : MonoBehaviour
{
    public GameObject coin;

    public int coinNum;

    void Awake()
    {
        GenerateCoinCodeBlock();
    }

    //以Scene_2为父物体生成金币
    void GenerateCoinCodeBlock()
    {
        for (int i = 0; i < coinNum; ++i)
        {
            GameObject temp = Instantiate(coin, new Vector3(Random.Range(-5f, 5f), Random.Range(2f, 10f), Random.Range(-6f, 2f)), Quaternion.identity);
            temp.transform.SetParent(GameObject.Find("ZeroPoint").transform.Find("Scenes").Find("Scene_2"));
        }
    }
}
