using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchInput : MonoBehaviour
{
    // Layer mask to filter out only the objects you want to interact with.
    public LayerMask interactableLayer;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
    }

    void Update()
    {
        // Check for touch input.
        if (Input.touchCount > 0)
        {
            // Loop through all active touches.
            for (int i = 0; i < Input.touchCount; i++)
            {
                UnityEngine.Touch touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Began) HandleInput(touch.position); // Check if the touch phase is "Began" (when the touch first starts).
            }
        }
        else // If no touch, use mouse input for debugging.
        {
            if (Input.GetMouseButtonDown(0)) HandleInput(Input.mousePosition);
            else if (Input.GetMouseButton(0)){} // You can also add logic for continuous interaction while the mouse button is held.
        }
    }

    void HandleInput(Vector2 inputPosition)
    {
        // Create a ray from the input position.
        Ray ray = Camera.main.ScreenPointToRay(inputPosition);

        // Check if the ray hits any objects with the interactableLayer.
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interactableLayer))
        {
            // Object touched, you can perform actions here.
            GameObject touchedObject = hit.collider.gameObject;
            //Saved Color and Size
            Color originalColor = touchedObject.GetComponent<Renderer>().material.color;
            Vector3 originalSize = new Vector3(0.4f, 0.4f, 8f);
            //Effects
            touchedObject.GetComponent<Renderer>().material.color = Color.yellow;
            touchedObject.GetComponent<AudioSource>().Play();
            Debug.Log(touchedObject.transform.name);
            touchedObject.transform.localScale += new Vector3(0.2f, 0.2f, 0.2f);
            mainCamera.backgroundColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            //Call Method for reset
            StartCoroutine(ResetEffects(touchedObject, originalColor, originalSize));
        }
        // Debug.Log(inputPosition);
    }

    IEnumerator ResetEffects(GameObject go, Color originalColor, Vector3 originalSize)
    {
        yield return new WaitForSeconds(0.1f);
        go.transform.localScale = originalSize;
        go.GetComponent<Renderer>().material.color = originalColor;
    }
}