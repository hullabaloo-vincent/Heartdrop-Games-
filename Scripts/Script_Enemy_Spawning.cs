using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Enemy_Spawning : MonoBehaviour{
    public GameObject[] Enemies;
    public float[] EnemyWeights;

    List<Vector3> enemySpawns = new List<Vector3>();
    List<GameObject> Spawned;

    bool spawnedEnemies = false;

    void Start(){
        Spawned = new List<GameObject>();
        for (int i = 0; i < transform.childCount; i++) {
            enemySpawns.Add(transform.GetChild(i).position);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!spawnedEnemies) {
            if (other.tag == "Player") {
                spawnedEnemies = true;
                //Spawn enemies
                for (int i = 0; i < enemySpawns.Count; i++) {
                    int enemyIndex = GetRandomWeightedIndex(EnemyWeights);
                    GameObject go = Instantiate(
                        Enemies[GetRandomWeightedIndex(EnemyWeights)], enemySpawns[i], transform.rotation
                    );  
                    Spawned.Add(go);
                }
                //Send team list to each enemy
                for (int i = 0; i < Spawned.Count; i++) {
                    Spawned[i].GetComponent<Script_Enemy_Base>().SetSpawnActor(gameObject);  
                    Spawned[i].GetComponent<Script_Enemy_Base>().SetTeam(Spawned);
                }
            }
        }
    }

    public void RemoveFromList(GameObject go){
        Spawned.Remove(go);
        Debug.Log(Spawned.Count);
    }

    public int GetRandomWeightedIndex(float[] weights) {
        if (weights == null || weights.Length == 0) return -1;

        float w;
        float t = 0;
        int i;
        for (i = 0; i < weights.Length; i++) {
            w = weights[i];

            if (float.IsPositiveInfinity(w)) {
                return i;
            } else if (w >= 0f && !float.IsNaN(w)) {
                t += weights[i];
            }
        }

        float r = Random.value;
        float s = 0f;

        for (i = 0; i < weights.Length; i++) {
            w = weights[i];
            if (float.IsNaN(w) || w <= 0f) continue;

            s += w / t;
            if (s >= r) return i;
        }

        return -1;
    }
}
