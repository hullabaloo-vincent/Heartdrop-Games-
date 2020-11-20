using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Enemy_Spawning : MonoBehaviour
{
    void Start()
    {
        _Spawned = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++)
        {
            _EnemySpawns.Add(transform.GetChild(i).position);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_SpawnedEnemies)
        {
            if (other.tag == "Player")
            {
                _SpawnedEnemies = true;
                //Spawn enemies
                for (int i = 0; i < _EnemySpawns.Count; i++)
                {
                    int enemyIndex = GetRandomWeightedIndex(EnemyWeights);
                    GameObject go = Instantiate(
                        Enemies[GetRandomWeightedIndex(EnemyWeights)], _EnemySpawns[i], transform.rotation
                    );
                    _Spawned.Add(go);
                }
                //Send team list to each enemy
                foreach (GameObject go in _Spawned)
                {
                    go.GetComponent<Script_Enemy_Base>().SetSpawnActor(gameObject);
                    go.GetComponent<Script_Enemy_Base>().SetTeam(_Spawned);
                }
            }
        }
    }

    public void RemoveFromList(GameObject go)
    {
        _Spawned.Remove(go);
        Debug.Log(_Spawned.Count);
    }

    public int GetRandomWeightedIndex(float[] weights)
    {
        if (weights == null || weights.Length == 0) return -1;

        float w;
        float t = 0;
        int i;
        for (i = 0; i < weights.Length; i++)
        {
            w = weights[i];

            if (float.IsPositiveInfinity(w))
            {
                return i;
            }
            else if (w >= 0f && !float.IsNaN(w))
            {
                t += weights[i];
            }
        }

        float r = Random.value;
        float s = 0f;

        for (i = 0; i < weights.Length; i++)
        {
            w = weights[i];
            if (float.IsNaN(w) || w <= 0f) continue;

            s += w / t;
            if (s >= r) return i;
        }

        return -1;
    }

    public GameObject[] Enemies;
    public float[] EnemyWeights;

    private List<Vector3> _EnemySpawns = new List<Vector3>();
    private List<GameObject> _Spawned;

    private bool _SpawnedEnemies = false;
}
