using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLifeScript : MonoBehaviour
{
    //source texture to get it started
    public Texture initTex;

    public Color color;

    public int width = 512;
    public int height = 512;
    //this is where you would stick the computer shader I wrote
    public ComputeShader compute;
    public RenderTexture input;
    public RenderTexture output;

    private int kernel;

    public Material material;

    // Use this for initialization
    void Start()
    {
        //creating of textures for the gpu to read/write to
        input = new RenderTexture(width, height, 24);
        input.wrapMode = TextureWrapMode.Repeat;
        input.enableRandomWrite = true;
        input.filterMode = FilterMode.Point;
        input.Create();

        //this copyies the souce texture to the newly created input texture
        Graphics.Blit(initTex, input);

        output = new RenderTexture(width, height, 24);
        output.wrapMode = TextureWrapMode.Repeat;
        output.enableRandomWrite = true;
        output.filterMode = FilterMode.Point;
        output.useMipMap = false;
        output.Create();

        if (height < 1 || width < 1) return;
        //find our gpu kernel (like finding main but for the gpu script)
        kernel = compute.FindKernel("GOL");
        //give it our textures and variables
        compute.SetTexture(kernel, "Input", input);
        compute.SetTexture(kernel, "Output", output);
        compute.SetFloat("Width", width);
        compute.SetFloat("Height", height);
        //and then we finally set the texture of the object to the output texture
        material.mainTexture = output;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //we update the color to any changes, just in case
        compute.SetVector("Color", color);
        //and we dispatch our kernal the thread groups
        compute.Dispatch(kernel, width / 8, height / 8, 1);
    }
    void OnDestroy()
    {
        //free up vram
        input.Release();
        output.Release();
    }
}