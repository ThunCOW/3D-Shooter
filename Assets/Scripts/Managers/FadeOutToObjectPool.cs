using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FadeOutToObjectPool : MonoBehaviour
{
    public ObjectPool<GameObject> Pool;

    [SerializeField] private float fadeDelay = 10f;
    [SerializeField] private float currentAlpha = 1;
    [SerializeField] private float requiredAlpha = 0;

    [Space]
    [SerializeField] private List<Material> materials = new List<Material>();
    [SerializeField] private bool getVariables;
    [SerializeField] private List<SkinnedMeshRenderer> renderers = new List<SkinnedMeshRenderer>();


    private void OnValidate()
    {
        if (renderers.Count == 0)
        {
            renderers.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>());
        }

        if (getVariables)
        {
            getVariables = false;
            
            renderers.Clear();

            renderers.AddRange(GetComponentsInChildren<SkinnedMeshRenderer>());
        }
    }

    private void Start()
    {
        foreach (var r in renderers)
            materials.AddRange(r.materials);
    }
    private void OnEnable()
    {
        Appear();
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutTimed(currentAlpha, requiredAlpha, fadeDelay));
    }

    private IEnumerator FadeOutTimed(float currentAlpha, float requiredAlpha, float fadeTime)
    {
        foreach (Material mat in materials)
        {
            mat.SetFloat("_Surface", 1);
            mat.SetShaderPassEnabled("SHADOWCASTER", true);
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
            mat.SetFloat("_DstBlend", 10);
            mat.SetFloat("_SrcBlend", 5);
            mat.SetFloat("_ZWrite", 0);

        }
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / fadeTime)
        {

            foreach (Material mat in materials)
            {
                Color c = new Color(mat.color.r, mat.color.g, mat.color.b, Mathf.Lerp(currentAlpha, requiredAlpha, t));

                mat.color = c;

                if (t > 0.7f && t < 0.85f)
                    mat.SetShaderPassEnabled("SHADOWCASTER", false);
            }


            yield return null;
        }

        //Debug.Log("Completed");
        Pool.Release(gameObject);
        gameObject.SetActive(false);
    }

    public void Appear()
    {
        foreach (Material mat in materials)
        {
            mat.SetFloat("_Surface", 0);
            mat.SetShaderPassEnabled("SHADOWCASTER", true);
            mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Geometry;
            mat.SetFloat("_DstBlend", 0);
            mat.SetFloat("_SrcBlend", 1);
            mat.SetFloat("_ZWrite", 1);

        }
        foreach (Material mat in materials)
        {
            Color c = new Color(mat.color.r, mat.color.g, mat.color.b, 1);

            mat.color = c;
        }
    }
}
