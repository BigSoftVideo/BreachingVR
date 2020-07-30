using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Valve.VR;

[RequireComponent(typeof(Camera))]
public class RealCameraFlip : MonoBehaviour
{
    public SteamVR_Action_Boolean flipAction;

    public Material material;
    public bool undistorted = true;
    public bool cropped = true;

    public bool flipped = false;

    public float ImageOffset = 0;

    private int flippedParamId = Shader.PropertyToID("_Flipped");
    private int useRightParamId = Shader.PropertyToID("_UseRight");
    private int imageOffsetParamId = Shader.PropertyToID("_ImageOffset");

    void Start()
    {
        if (!SystemInfo.supportsImageEffects || null == material ||
           null == material.shader || !material.shader.isSupported)
        {
            Debug.Log("Disabling real camera flip due to some issue.");
            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {
        if (flipAction != null)
        {
            flipAction.AddOnChangeListener(ToggleFlipped, SteamVR_Input_Sources.Any);
        }

        // The video stream must be symmetrically acquired and released in
        // order to properly disable the stream once there are no consumers.
        SteamVR_TrackedCamera.VideoStreamTexture source = SteamVR_TrackedCamera.Source(undistorted);
        source.Acquire();

        // Auto-disable if no camera is present.
        if (!source.hasCamera)
        {
            enabled = false;
            Debug.LogWarning("No SteamVR_TrackedCamera was detected.");
        }
    }

    private void OnDisable()
    {
        if (flipAction != null)
        {
            flipAction.RemoveOnChangeListener(ToggleFlipped, SteamVR_Input_Sources.Any);
        }

        // Clear the texture when no longer active.
        material.mainTexture = null;

        // The video stream must be symmetrically acquired and released in
        // order to properly disable the stream once there are no consumers.
        SteamVR_TrackedCamera.VideoStreamTexture source = SteamVR_TrackedCamera.Source(undistorted);
        source.Release();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ImageOffset += 1f / 128;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            ImageOffset -= 1f / 128;
        }
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            flipped = !flipped;
        }

        SteamVR_TrackedCamera.VideoStreamTexture source = SteamVR_TrackedCamera.Source(undistorted);
        Texture2D texture = source.texture;
        if (texture == null)
        {
            Debug.LogWarning("Texture from tracked camera was null.");
            return;
        }

        // Apply the latest texture to the material.  This must be performed
        // every frame since the underlying texture is actually part of a ring
        // buffer which is updated in lock-step with its associated pose.
        // (You actually really only need to call any of the accessors which
        // internally call Update on the SteamVR_TrackedCamera.VideoStreamTexture).
        material.mainTexture = texture;

        // Adjust the height of the quad based on the aspect to keep the texels square.
        float aspect = (float)texture.width / texture.height;

        // The undistorted video feed has 'bad' areas near the edges where the original
        // square texture feed is stretched to undo the fisheye from the lens.
        // Therefore, you'll want to crop it to the specified frameBounds to remove this.
        if (cropped)
        {
            VRTextureBounds_t bounds = source.frameBounds;
            material.mainTextureOffset = new Vector2(bounds.uMin, bounds.vMin);

            float du = bounds.uMax - bounds.uMin;
            float dv = bounds.vMax - bounds.vMin;
            material.mainTextureScale = new Vector2(du, dv);

            aspect *= Mathf.Abs(du / dv);
        }
        else
        {
            material.mainTextureOffset = Vector2.zero;
            material.mainTextureScale = new Vector2(1, -1);
        }

        //target.localScale = new Vector3(1, 1.0f / aspect, 1);

        // Apply the pose that this frame was recorded at.
        if (source.hasTracking)
        {
            SteamVR_Utils.RigidTransform rigidTransform = source.transform;
            //target.localPosition = rigidTransform.pos;
            //target.localRotation = rigidTransform.rot;
        }
    }

    private void ToggleFlipped(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource, bool newState)
    {
        if (newState == true) flipped = !flipped;
    }

    void OnRenderImage(RenderTexture _source, RenderTexture destination)
    {
        //Debug.Log(destination.name);
        SteamVR_TrackedCamera.VideoStreamTexture cameraSource = SteamVR_TrackedCamera.Source(undistorted);
        Texture2D texture = cameraSource.texture;
        if (texture == null)
        {
            Debug.LogWarning("Texture from tracked camera was null.");
            return;
        }
        material.SetFloat(flippedParamId, flipped ? 1 : 0);
        material.SetFloat(useRightParamId, destination.name.Contains("Right") ? 1 : 0);
        material.SetFloat(imageOffsetParamId, ImageOffset);
        Graphics.Blit(texture, destination, material);
    }
}
