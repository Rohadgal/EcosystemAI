using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalCanvasController : MonoBehaviour
{
    private Canvas currentCanvas; // Reference to the canvas currently being displayed

    void Start() {
        // Hide all canvases at the start

        StartCoroutine(wait());
    }

    IEnumerator wait() {
        yield return new WaitForSeconds(.5f);
        Canvas[] allCanvases = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in allCanvases) {
            canvas.enabled = false;
        }

        // Reset the currentCanvas reference to null
        currentCanvas = null;
    }

    void Update() {
        // Check for mouse click
        if (Input.GetMouseButtonDown(0)) {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Check if the ray hits any GameObject with a canvas
            if (Physics.Raycast(ray, out hit)) {
                GameObject hitObject = hit.collider.gameObject;
                Canvas canvas = hitObject.GetComponentInChildren<Canvas>();

                // Toggle canvas visibility
                if (canvas != null) {
                    if (canvas != currentCanvas) {
                        if (currentCanvas != null) {
                            currentCanvas.enabled = false;
                        }
                        canvas.enabled = true;
                        currentCanvas = canvas;
                    } else {
                        canvas.enabled = !canvas.enabled;
                    }
                }
            } else {
                // If the click is not on an animal, hide the current canvas
                if (currentCanvas != null) {
                    currentCanvas.enabled = false;
                    currentCanvas = null;
                }
            }
        }
    }
}
