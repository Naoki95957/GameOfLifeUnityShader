using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptSideCompute : MonoBehaviour
{

    public ComputeShader compute;

    public Color color;
    public Color color2;

    public RenderTexture result;

    private int kernel;

    void Start()
    {
        kernel = compute.FindKernel("CSMain");

        //512x512 24bit
        result = new RenderTexture(512, 512, 24);
        result.enableRandomWrite = true;//needed so compute can write to the tex
        result.Create();
    }

    void Update()
    {

        compute.SetTexture(kernel, "Result", result);
        compute.SetVector("Color", color);
        compute.SetVector("Color2", color2);
        //Remember that kernel had set up workgroups for 8, 8, 1 and the texture is 512x512
        compute.Dispatch(kernel, 512 / 8, 512 / 8, 1);

    }
}