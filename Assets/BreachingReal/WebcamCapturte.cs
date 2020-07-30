﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
using UnityEngine;
using OpenCvSharp;
using OpenCvSharp.Demo;

public class WebcamCapturte : WebCamera
{
    public int selectedDeviceId = 0;

    protected override void Awake()
    {
        base.Awake();
        this.forceFrontalCamera = true;

        Debug.Log("There are " + WebCamTexture.devices.Length + " devices");
        DeviceName = WebCamTexture.devices[selectedDeviceId].name;
    }

    // Our sketch generation function
    protected override bool ProcessTexture(WebCamTexture input, ref Texture2D output)
    {
        Mat img = OpenCvSharp.Unity.TextureToMat(input, TextureParameters);

        //Convert image to grayscale
        Mat imgGray = new Mat();
        Cv2.CvtColor(img, imgGray, ColorConversionCodes.BGR2GRAY);

        // Clean up image using Gaussian Blur
        Mat imgGrayBlur = new Mat();
        Cv2.GaussianBlur(imgGray, imgGrayBlur, new Size(5, 5), 0);

        //Extract edges
        Mat cannyEdges = new Mat();
        Cv2.Canny(imgGrayBlur, cannyEdges, 10.0, 70.0);

        //Do an invert binarize the image
        Mat mask = new Mat();
        Cv2.Threshold(cannyEdges, mask, 70.0, 255.0, ThresholdTypes.BinaryInv);

        // result, passing output texture as parameter allows to re-use it's buffer
        // should output texture be null a new texture will be created
        output = OpenCvSharp.Unity.MatToTexture(img, output);
        return true;
    }
}
*/