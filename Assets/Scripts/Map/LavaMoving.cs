using UnityEngine;
using System.Collections;

public class LavaMover : MonoBehaviour
{
    [SerializeField] float topOffset = 5f;      // mennyit menjen fel
    [SerializeField] float speed = 2f;          // mozgási sebesség
    [SerializeField] float waitTimeTop = 1f;    // várakozás fent
    [SerializeField] float waitTimeBottom = 1f; // várakozás alul

    private Vector3 startPos;
    private Vector3 topPos;

    private void Start()
    {
        startPos = transform.position;
        topPos = startPos + Vector3.up * topOffset;

        StartCoroutine(MoveRoutine());
    }

    private IEnumerator MoveRoutine()
    {
        while (true)
        {
            // Fel
            yield return MoveTo(topPos);

            // Vár fent
            yield return new WaitForSeconds(waitTimeTop);

            // Le
            yield return MoveTo(startPos);

            // Vár alul
            yield return new WaitForSeconds(waitTimeBottom);
        }
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        while (Vector3.Distance(transform.position, target) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                speed * Time.deltaTime
            );
            yield return null;
        }
    }
}
