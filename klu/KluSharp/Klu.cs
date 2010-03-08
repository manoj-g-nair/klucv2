﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows;

// For interface to C DLL
using System.Runtime.InteropServices;

namespace KluSharp
{
    [StructLayout(LayoutKind.Sequential)]
    public class TestStruct
    {
        public Int32 A;
        public Int32 B;
    };

    [StructLayout(LayoutKind.Sequential)]
    public class ProcessOptions
    {
    };

    [StructLayout(LayoutKind.Sequential)]
    public class KluPoint
    {
        public Int32 X;
        public Int32 Y;
    };

    [StructLayout(LayoutKind.Sequential)]
    public class EyeFeaturePoints
    {
        public KluPoint EyeCenter;
        public KluPoint LidUpCenter;
        public KluPoint LidBottomCenter;
        public KluPoint LidCornerLeft; // from your point of view
        public KluPoint LidCornerRight;// from your point of view
    };

    [StructLayout(LayoutKind.Sequential)]
    public class MouthFeaturePoints
    {
        public KluPoint LipUpCenter;
        public KluPoint LipBottomCenter;
        public KluPoint LipUpRight;
        public KluPoint LipBottomRight;
        public KluPoint LipUpLeft;
        public KluPoint LipBottomLeft;
        public KluPoint LipCornerLeft; // from your point of view
        public KluPoint LipCornerRight;// from your point of view
    };

    [StructLayout(LayoutKind.Sequential)]
    public class FaceFeaturePoints
    {
        public EyeFeaturePoints leftEye;
        public EyeFeaturePoints rightEye;
        public MouthFeaturePoints mouth;
    };

    /// <summary>
    /// This class encapsulates the C klu DLL library and provides some
    /// methods to image content from the library.
    /// </summary>
    public class Klu
    {
        System.Drawing.Bitmap tmpBitmap;

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        private static extern int klu_initializeLibrary();

#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        private static extern int klu_deinitializeLibrary();

        /// <summary>
        /// Constructs a new Klu object and initializes the wrapped C DLL library.
        /// </summary>
        public Klu()
        {
            // A good place to initialize the library
            tmpBitmap = new System.Drawing.Bitmap(320, 280);

            klu_initializeLibrary();
        }

        /// <summary>
        /// Destroys the Klu object and deinitializes the wrapped C DLL library.
        /// </summary>
        ~Klu()
        {
            // A good place to deinitialize the library
            //FreeCapture();

            klu_deinitializeLibrary();
        }

#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        private static extern Int32 klu_createAndSaveAnn(
            [In, MarshalAs(UnmanagedType.LPArray)] int[] numNeuronsPerLayer,
            int numLayers,
            int activationFunction,
            [In, MarshalAs(UnmanagedType.LPStr)] string filepath
        );

        /// <summary>
        /// Create a neural network using the given parameters and saves it as a reusable
        /// OpenCV XML file to the desired "filepath".
        /// </summary>
        /// <param name="numNeuronsPerLayer"></param>
        /// <param name="activationFunction"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public bool CreateAndSaveAnn(int[] numNeuronsPerLayer, ANN.ActivationFunction activationFunction, string filepath)
        {
            int res = klu_createAndSaveAnn(numNeuronsPerLayer, numNeuronsPerLayer.Count(), (int)activationFunction, filepath);
            return res == 1;
        }


#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        private static extern Int32 testStruct([Out, MarshalAs(UnmanagedType.LPStruct)] TestStruct t);
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns>Returns 1 if successful; otherwise 0 is returned.</returns>
#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        private static extern int createCapture();

        /// <summary>
        /// Creates a camera capture inside the DLL. If there already is a camera capture,
        /// it will be used without creating a new one if this method is called twice.
        /// </summary>
        /// <returns></returns>
        public bool CreateCapture()
        {
            return (createCapture() == 1);
        }

        /// <summary>
        /// 
        /// </summary>
#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        private static extern void freeCapture();

        /// <summary>
        /// 
        /// </summary>
        public void FreeCapture()
        {
            freeCapture();
            TestStruct t = new TestStruct();
            t.A = 3;
            t.B = 9;
            testStruct(t);
            Console.WriteLine("Output (Sum): " + testStruct(t));
        }

        /// <summary>
        /// Writes raw OpenCV image data into the System.Drawing.Bitmap object.
        /// </summary>
        /// <param name="bitmap">This System.Drawing.Bitmap object needs to have the same width and height as the corresponding parameters.</param>
        /// <param name="imageData">This is a basically an C unsigned char pointer to the OpenCV image data.</param>
        /// <param name="width">The image's width</param>
        /// <param name="height">The image's height</param>
        /// <param name="channels">The image's number of channels</param>
        /// <param name="widthStep">The image's width step</param>
        unsafe private void convertToBitmap2(ref System.Drawing.Bitmap bitmap, ref byte* imageData, int width, int height, int channels, int widthStep)
        {
            unsafe
            {
                int nl = height;
                int nc = width * channels;
                int step = widthStep;
                int t = 0;
                for (int i = 0; i < nl; i++)
                {
                    for (int j = 0; j < nc; j += channels)
                    {
                        //bitmap.SetPixel(j / 3, i, System.Drawing.Color.FromArgb(imageData[j + t], imageData[j + 1 + t], imageData[j + 2 + t]));
                        // Somehow Red and Blue where switched.
                        bitmap.SetPixel(j / 3, i, System.Drawing.Color.FromArgb(imageData[j + 2 + t], imageData[j + 1 + t], imageData[j + t]));
                    }
                    t += step;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="nChannels"></param>
        /// <param name="widthStep"></param>
#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        unsafe private static extern void queryCaptureImage(byte** data, int* width, int* height, int* nChannels, int* widthStep);
        //safe alternative (didn't work):
        //private static extern void queryImage(out byte data, out int width, out int height, out int nChannels, out int widthStep);

        /// <summary>
        /// Queries an image from the capture device previously created with
        /// "CreateCapture()".
        /// 
        /// Use this function if your are programming with WindowsForms.
        /// 
        /// If you want to set the image in a WPF Image control use this code:
        /// 
        ///    System.Drawing.Bitmap bitmap = klu.QueryCaptureImage())
        ///    klu.SetWpfImageFromBitmap(ref YOUR_WPF_IMAGE, ref bitmap);
        /// 
        /// </summary>
        /// <returns>Returns the queried image as a System.Drawing.Bitmap</returns>
        public System.Drawing.Bitmap QueryCaptureImage()
        {
            unsafe
            {
                int width, height, widthStep, channels;
                byte* imageData;
                queryCaptureImage(&imageData, &width, &height, &channels, &widthStep);
                //System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height);
                //convertToBitmap2(ref bitmap, ref imageData, width, height, channels, widthStep);
                convertToBitmap2(ref tmpBitmap, ref imageData, width, height, channels, widthStep);
                return tmpBitmap;
            }
        }

        public void QueryCaptureImage2(ref System.Drawing.Bitmap bitmap)
        {
            unsafe
            {
                int width, height, widthStep, channels;
                byte* imageData;
                queryCaptureImage(&imageData, &width, &height, &channels, &widthStep);
                //System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height);
                convertToBitmap2(ref bitmap, ref imageData, width, height, channels, widthStep);
            }
        }

#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        private static extern Int32 klu_processCaptureImage(
            [In, MarshalAs(UnmanagedType.LPStruct)] ProcessOptions processOptions,
            [Out, MarshalAs(UnmanagedType.LPStruct)] FaceFeaturePoints ffp);

        public bool ProcessCaptureImage()
        {
            ProcessOptions options = new ProcessOptions();
            FaceFeaturePoints ffp = new FaceFeaturePoints();
            int res = klu_processCaptureImage(options, ffp);
            return res == 1;
        }

#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        private static extern Int32 klu_processStillImage(
            [In, MarshalAs(UnmanagedType.LPStr)] string filepath,
            [In, MarshalAs(UnmanagedType.LPStruct)] ProcessOptions processOptions,
            [Out, MarshalAs(UnmanagedType.LPStruct)] FaceFeaturePoints ffp);

        public bool ProcessStillImage(string filepath)
        {
            ProcessOptions options = new ProcessOptions();
            FaceFeaturePoints ffp = new FaceFeaturePoints();
            int res = klu_processStillImage(filepath, options, ffp);
            return res == 1;
        }

#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        unsafe private static extern int klu_getLastProcessedImage(byte** data, int* width, int* height, int* nChannels, int* widthStep);

        public void GetLastProcessedImage(ref System.Drawing.Bitmap bitmap)
        {
            unsafe
            {
                int width, height, widthStep, channels;
                byte* imageData;
                int res = klu_getLastProcessedImage(&imageData, &width, &height, &channels, &widthStep);
                //System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height);
                convertToBitmap2(ref bitmap, ref imageData, width, height, channels, widthStep);
            }
        }

        /// <summary>
        /// Implicitly converts the old System.Drawing.Bitmap (GDI) "bitmap" to a new
        /// System.Windows.Media.Imaging.BitmapSource (DirectX) and sets the
        /// "Source" attribute of the "image" to the result of the conversion.
        /// 
        /// Use this function if your are programming with WPF.
        /// </summary>
        /// <param name="image">The WPF image control</param>
        /// <param name="bitmap">The System.Drawing.Bitmap object to set as the image's "Source" attribute</param>
        public void SetWpfImageFromBitmap(ref System.Windows.Controls.Image image, ref System.Drawing.Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();

            System.Windows.Media.Imaging.BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            image.Source = bitmapSource;

            // TODO: Will this cause a crash?
            DeleteObject(hBitmap);
        }

        public void SetImageBrushFromBitmap(ref System.Windows.Media.ImageBrush image, ref System.Drawing.Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();

            System.Windows.Media.Imaging.BitmapSource bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                hBitmap,
                IntPtr.Zero,
                Int32Rect.Empty,
                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            image.ImageSource = bitmapSource;

            // TODO: Will this cause a crash?
            DeleteObject(hBitmap);
        }
    }
}