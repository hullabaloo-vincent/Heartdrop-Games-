using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Script_Enemy_Spawning : MonoBehaviour{
    public GameObject[] Enemies;
    public float[] EnemyWeights;

    List<Vector3> enemySpawns = new List<Vector3>();

    bool spawnedEnemies = false;

    void Start(){
        for (int i = 0; i < transform.childCount; i++) {
            enemySpawns.Add(transform.GetChild(i).position);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (!spawnedEnemies) {
            spawnedEnemies = true;
            if (other.tag == "Player") {
                for (int i = 0; i < enemySpawns.Count; i++) {
                    Instantiate(Enemies[GetRandomWeightedIndex(EnemyWeights)], enemySpawns[i], transform.rotation);
                }
            }
        }
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
