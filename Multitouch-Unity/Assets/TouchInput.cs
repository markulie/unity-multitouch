using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TouchInput : MonoBehaviour
{
    [Header("Interaction")]
    [SerializeField] private LayerMask interactableLayer;

    [Header("Effects")]
    [SerializeField] private Color touchColor = Color.yellow;
    [SerializeField] private float scaleAmount = 0.2f;
    [SerializeField] private float effectDuration = 0.1f;

    private Camera mainCamera;
    private static readonly Vector3 ScaleOffset = Vector3.one * 0.2f;

    private void Awake()
    {
        mainCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        // Touch input
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);

                if (touch.phase == TouchPhase.Began)
                    HandleInput(touch.position);
            }

            return;
        }

        // Mouse input (Editor/Desktop)
        if (Input.GetMouseButtonDown(0))
        {
            HandleInput(Input.mousePosition);
        }
    }

    private void HandleInput(Vector2 inputPosition)
    {
        Ray ray = mainCamera.ScreenPointToRay(inputPosition);

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, interactableLayer))
            return;

        GameObject touchedObject = hit.collider.gameObject;

        if (!hit.collider.TryGetComponent(out Renderer renderer))
            return;

        hit.collider.TryGetComponent(out AudioSource audioSource);

        Color originalColor = renderer.material.color;
        Vector3 originalScale = touchedObject.transform.localScale;

        // Effects
        renderer.material.color = touchColor;
        touchedObject.transform.localScale += Vector3.one * scaleAmount;
        audioSource?.Play();

        mainCamera.backgroundColor = Random.ColorHSV(
            0f, 1f,
            1f, 1f,
            0.5f, 1f);

        Debug.Log(touchedObject.name);

        StartCoroutine(ResetEffects(
            renderer,
            touchedObject.transform,
            originalColor,
            originalScale));
    }

    private IEnumerator ResetEffects(
        Renderer renderer,
        Transform objectTransform,
        Color originalColor,
        Vector3 originalScale)
    {
        yield return new WaitForSeconds(effectDuration);

        if (renderer != null)
            renderer.material.color = originalColor;

        if (objectTransform != null)
            objectTransform.localScale = originalScale;
    }
}