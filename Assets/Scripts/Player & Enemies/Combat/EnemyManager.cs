using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject Enemy;

    public bool Set;
    // Start is called before the first frame update

    private void Awake()
    {
        if (Set == false)
        {
            Enemy.SetActive(false);
        }
    }

    public void EnemySpawn(bool set) 
    {
        if (Enemy != null) 
        {
            Enemy.SetActive(set);
            Set = false;
        }
    }
}
