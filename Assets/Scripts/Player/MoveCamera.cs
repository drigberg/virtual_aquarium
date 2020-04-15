using UnityEngine;

public class MoveCamera : MonoBehaviour {

    public Transform player;
    public float followDistance = 3.0f;
    public float height = 1.0f;

    void Update() {
        transform.position = player.transform.position - player.transform.forward * followDistance + player.transform.up * height;
    }
}