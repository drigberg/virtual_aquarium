using UnityEngine;

public class MoveCamera : MonoBehaviour {

    public Transform player;
    public float followDistance;

    void Update() {
        transform.position = player.transform.position - player.transform.forward * followDistance - player.transform.up * 2.0f;
    }
}