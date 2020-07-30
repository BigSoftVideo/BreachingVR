
using System;
using UnityEngine;

using Valve.VR;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class FlipCamera : MonoBehaviour
{
    public SteamVR_Action_Boolean flipAction;

    public Material material;

    public bool flipped = false;

    void Start()
    {
        if (!SystemInfo.supportsImageEffects || null == material ||
           null == material.shader || !material.shader.isSupported)
        {
            enabled = false;
            return;
        }
    }

    void OnEnable()
    {
        if (flipAction != null)
        {
            flipAction.AddOnChangeListener(ToggleFlipped, SteamVR_Input_Sources.Any);
        }
    }


    private void OnDisable()
    {
        if (flipAction != null)
        {
            flipAction.RemoveOnChangeListener(ToggleFlipped, SteamVR_Input_Sources.Any);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            flipped = !flipped;
        }
    }

    private void ToggleFlipped(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        if (newState == true) flipped = !flipped;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (flipped) Graphics.Blit(source, destination, material);
        else Graphics.Blit(source, destination);
    }
}
