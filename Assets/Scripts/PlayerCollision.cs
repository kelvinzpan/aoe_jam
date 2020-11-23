using UnityEngine;

public class PlayerCollision : MonoBehaviour
{

    public Transform Spawnpoint;
    public GameObject Prefab;

    void Start()
    {
        var Snow = GetComponent<ParticleSystem>().emission;
        Snow.enabled = false;
    }

    void OnCollisionEnter(Collision collisionInfo){
        Debug.Log(collisionInfo.collider.tag);
        if(collisionInfo.collider.tag == "Obstacle"){
            KeepScore.Score += 100;
            Instantiate(Prefab, Spawnpoint.position, Spawnpoint.rotation);
        }

        else if (collisionInfo.collider.tag == "Terrain")
        {
            var Snow = GetComponent<ParticleSystem>().emission;
            Snow.enabled = true;
        }
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        if (collisionInfo.collider.tag == "Terrain")
        {
            var Snow = GetComponent<ParticleSystem>().emission;
            Snow.enabled = false;
        }
    }
}
