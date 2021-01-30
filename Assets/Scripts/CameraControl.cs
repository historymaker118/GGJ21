﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public Rect bounds;
    public Color debugColor;

    new public Camera camera;
    private Transform camTransform;

    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        camTransform = camera.transform;
    }

    // Update is called once per frame
    void Update()
    {
        var pos = (Vector2)transform.position;
        var bottomLeft = pos + bounds.min;
        var topLeft = pos + new Vector2(bounds.xMin, bounds.yMax);
        var topRight = pos + bounds.max;
        var bottomRight = pos + new Vector2(bounds.xMax, bounds.yMin);

        var tarPos = target.transform.position;
        var x = tarPos.x;
        var y = tarPos.y;

        var camHeight = camera.orthographicSize;
        var camWidth = camHeight * camera.aspect;

        var minX = pos.x + bounds.xMin + camWidth;
        var maxX = pos.x + bounds.xMax - camWidth;
        var minY = pos.y + bounds.yMin + camHeight;
        var maxY = pos.y + bounds.yMax - camHeight;

        x = Mathf.Clamp(x, minX, maxX);
        y = Mathf.Clamp(y, minY, maxY);

        camTransform.position = new Vector3(x, y, -10.0f);


    }

    private void OnDrawGizmos()
    {
        var pos = (Vector2)transform.position;
        var bottomLeft = pos + bounds.min;
        var topLeft = pos + new Vector2(bounds.xMin, bounds.yMax);
        var topRight = pos + bounds.max;
        var bottomRight = pos + new Vector2(bounds.xMax, bounds.yMin);

        Gizmos.color = debugColor;
        Gizmos.DrawLine(bottomLeft, topLeft);
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);

        Gizmos.color = Color.white;
    }
}
