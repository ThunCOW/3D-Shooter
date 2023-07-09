using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutToObjectPool : MonoBehaviour
{
    [SerializeField] private float fadeDelay = 10f;
    [SerializeField] private float currentAlpha = 1;
    [SerializeField] private float requiredAlpha = 0;

    [SerializeField] private List<Material> materials = new List<Material>();
    [SerializeField] private List<SkinnedMeshRenderer> renderers = new List<SkinnedMeshRenderer>();

    public bool getVariables;

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

    private void OnEnable()
    {
        foreach (var r in renderers)
            materials.AddRange(r.materials);
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutTimed(currentAlpha, requiredAlpha, fadeDelay));
        //StartCoroutine(Test());
    }

    private IEnumerator Test()
    {
        yield return new WaitForSeconds(fadeDelay);
        Debug.Log("works");
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
        gameObject.SetActive(false);
    }
}
