using UnityEngine;

public class PlayerCollision : MonoBehaviour
{

    public Transform Spawnpoint;
    public GameObject Prefab;

    void OnCollisionEnter(Collision collisionInfo){
        Debug.Log(collisionInfo.collider.tag);
        if(collisionInfo.collider.tag == "Obstacle"){
            KeepScore.Score += 100;
            Instantiate(Prefab, Spawnpoint.position, Spawnpoint.rotation);
        }
    }
}
