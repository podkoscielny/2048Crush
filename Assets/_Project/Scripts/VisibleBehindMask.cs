using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class VisibleBehindMask : MonoBehaviour
{
    [SerializeField] Material maskMaterial;

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();

        renderer.sharedMaterial.renderQueue = maskMaterial.renderQueue - 1;
    }
}
