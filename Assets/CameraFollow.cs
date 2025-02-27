using UnityEngine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    public Transform target; 
    public Vector3 offset = new Vector3(0, 5, -10); 
    public float smoothSpeed = 5f; 
    private bool isFocusingOnUpgrade = false;
    private Vector3 originalOffset;

    void Start()
    {
        originalOffset = offset;
    }

    void LateUpdate()
    {
        if (target == null || isFocusingOnUpgrade) return;

        
        Vector3 desiredPosition = target.position + offset;

        
        transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        
        transform.LookAt(target);
    }

    public void MoveToUpgradedDragon(Transform upgradedDragon, Transform cameraTargetPosition, float duration)
    {
        StartCoroutine(FocusOnUpgradedDragon(upgradedDragon, cameraTargetPosition, duration));
    }

    private IEnumerator FocusOnUpgradedDragon(Transform upgradedDragon, Transform cameraTargetPosition, float duration)
    {
        isFocusingOnUpgrade = true;
        float elapsedTime = 0;
        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startPosition, cameraTargetPosition.position, elapsedTime / duration);
            transform.rotation = Quaternion.Lerp(startRotation, cameraTargetPosition.rotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = cameraTargetPosition.position;
        transform.rotation = cameraTargetPosition.rotation;

        yield return new WaitForSeconds(2f);

        isFocusingOnUpgrade = false;
    }
}
