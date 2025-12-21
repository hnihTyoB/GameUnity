using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class GrapeProjectile : MonoBehaviour
{
    [SerializeField] private float duration = 1f;
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField] private float heightY = 3f;
    [SerializeField] private GameObject grapeProjectileShadow;
    [SerializeField] private GameObject splatterPrefab;
    [SerializeField] private float targetOffsetY = -0.5f;

    private Vector3 targetPosition;
    private bool hasTarget = false;

 
    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
        hasTarget = true;
    }

    private void Start()
    {
        GameObject grapeShadow =
        Instantiate(grapeProjectileShadow, transform.position + new Vector3(0, -0.3f, 0), Quaternion.identity);

   
        Vector3 targetPos;
        if (hasTarget)
        {
            targetPos = targetPosition;
        }
        else if (PlayerController.Instance != null)
        {
          
            targetPos = PlayerController.Instance.transform.position;
        }
        else
        {
          
            Destroy(grapeShadow);
            Destroy(gameObject);
            return;
        }
        

        targetPos += new Vector3(0, targetOffsetY, 0);
        
        Vector3 grapeShadowStartPosition = grapeShadow.transform.position;

        StartCoroutine(ProjectileCurveRoutine(transform.position, targetPos));
        StartCoroutine(MoveGrapeShadowRoutine(grapeShadow, grapeShadowStartPosition, targetPos));
    }
    private IEnumerator ProjectileCurveRoutine(Vector3 startPosition, Vector3 endPosition)
    {
        float timePassed = 0f;

        while (timePassed < duration)
        {
            timePassed += Time.deltaTime;
            float linearT = timePassed / duration;
            float heightT = animCurve.Evaluate(linearT);
            float height = Mathf.Lerp(0f, heightY, heightT);

            transform.position = Vector2.Lerp(startPosition, endPosition, linearT) + new Vector2(0f, height);

            yield return null;
        }
        Instantiate(splatterPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
    private IEnumerator MoveGrapeShadowRoutine(GameObject grapeShadow, Vector3 startPosition, Vector3 endPosition) {
        float timePassed = 0f;

        while (timePassed < duration) 
        {
            timePassed += Time.deltaTime;
            float linearT = timePassed / duration;
            grapeShadow.transform.position = Vector2.Lerp(startPosition, endPosition, linearT);
            yield return null;
        }

        Destroy(grapeShadow);
    }
}
