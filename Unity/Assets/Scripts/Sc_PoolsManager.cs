using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sc_PoolsManager : MonoBehaviour
{
    public enum poolType { Bullet, Ray, Soul};

    [System.Serializable]
    public class pool
    {
        public poolType type;
        public GameObject ball;
        public int startPoolSize;
    }

    public List<pool> pools = new List<pool>();

    // Start is called before the first frame update
    void Start()
    {
        foreach(pool p in pools)
        {
            GameObject go = new GameObject();
            go.name = p.type.ToString() + "sPool";
            go = Instantiate(go, transform);
            go.name = go.name.Replace("(Clone)", "");

            GameObject b = Instantiate(p.ball);
            b.SetActive(false);
            for(int i = 0; i < p.startPoolSize; i++)
            {
                Instantiate(b, go.transform);
            }
        }
    }

    public GameObject ballToSpawn(poolType type)
    {
        GameObject ball = null;

        Transform Pool = null;
        foreach (Transform tr in transform)
        {
            if(tr.gameObject.name == type.ToString() + "sPool")
            {
                Pool = tr;
                break;
            }
        }

        foreach(Transform tr in Pool)
        {
            if (!tr.gameObject.activeInHierarchy)
            {
                ball = tr.gameObject;
                break;
            }
        }
        if(ball == null)
        {
            ball = Instantiate(Pool.GetChild(0).gameObject, Pool);
        }
        ball.gameObject.name.Replace("(Clone)","");

        return ball;
    }
}
