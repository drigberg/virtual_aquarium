using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterTrigger : MonoBehaviour
{
    private Color normalColor;
    private Color underwaterColor;

    // Start is called before the first frame update
    void Start()
    {
        normalColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        underwaterColor = new Color(0.22f, 0.65f, 0.77f, 0.5f);
        SetUnderwater(); 
    }
    private void SetUnderwater() {
        RenderSettings.fogColor = underwaterColor;
        RenderSettings.fogDensity = 0.03f;
    }

    private void SetNormal() {
        RenderSettings.fogColor = normalColor;
        RenderSettings.fogDensity = 0.002f;
    }
    //When the Primitive collides with the walls, it will reverse direction
    private void OnTriggerEnter(Collider other)
    {
        SetNormal();
    }

    //When the Primitive exits the collision, it will change Color
    private void OnTriggerExit(Collider other)
    {
        SetUnderwater();
    }
}
