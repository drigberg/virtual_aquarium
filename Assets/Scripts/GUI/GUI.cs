using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUI : MonoBehaviour
{
    public Canvas canvas;
    public bool paused = false;

    // Start is called before the first frame update
    void Start() {
        PauseGame();
        paused = true;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.P)) {
            TogglePaused();
        }
    }

    
    void OnGUI() {
        Cursor.lockState = CursorLockMode.Locked;
        if (GUILayout.Button("Instructions")) {
            Debug.Log("Instructions!");
        }
    }

    void OnApplicationFocus(bool hasFocus) {
        if (!hasFocus) {
            PauseGame();
        }
    }

    void TogglePaused() {
        if (paused) {
            UnpauseGame();
        } else {
            PauseGame();
        }
    }

    void PauseGame() {
        Time.timeScale = 0;
        canvas.enabled = true;
        paused = true;
    }

    void UnpauseGame() {
        Time.timeScale = 1;
        canvas.enabled = false;
        paused = false;
    }
}
