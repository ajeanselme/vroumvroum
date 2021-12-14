using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrownLevitate : MonoBehaviour
{
    private float upDistance;
    private float speed;

    private float t = 0f;

    private float yStartPos;
    private bool isDown = false;
    
    public void Initialize(float _upDistance, float _speed)
    {
        upDistance = _upDistance;
        speed = _speed;

        yStartPos = transform.localPosition.y;
    }

    private void Update()
    {
        Levitate();
    }

    private void Levitate()
    {
        t += Time.deltaTime * speed;

        float nPosY = isDown ? Mathf.Lerp(yStartPos + upDistance, yStartPos, t) : Mathf.Lerp(yStartPos, yStartPos + upDistance, t);

        transform.localPosition = new Vector3(transform.localPosition.x, nPosY, transform.localPosition.z);

        if (t >= 1f)
        {
            t = 0f;
            isDown = !isDown;
        }
    }
}
