using System;
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
    /// <summary>
    /// This class encapsulates the C klu DLL library and provides some
    /// methods to image content from the library.
    /// </summary>
    public class Klu
    {
        #region Constructor and Destructor
#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        private static extern int klu_initializeLibrary();

        /// <summary>
        /// Constructs a new Klu object and initializes the wrapped C DLL library.
        /// </summary>
        public Klu()
        {            
            klu_initializeLibrary();
        }

#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        private static extern int klu_deinitializeLibrary();

        /// <summary>
        /// Destroys the Klu object and deinitializes the wrapped C DLL library.
        /// </summary>
        ~Klu()
        {
            // A good place to deinitialize the library
            //FreeCapture();

            klu_deinitializeLibrary();
        }
        #endregion

        #region Create and save ANN
#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        private static extern Int32 klu_createAndSaveAnn(
            [In, MarshalAs(UnmanagedType.LPArray)] int[] numNeuronsPerLayer,
            int numLayers,
            int activationFunction,
            double alpha,
            double beta,
            [In, MarshalAs(UnmanagedType.LPStr)] string filepath
        );

        /// <summary>
        /// Create a neural network using the given parameters and saves it as a reusable
        /// OpenCV XML file to the desired "filepath".
        /// </summary>
        /// <param name="numNeuronsPerLayer"></param>
        /// <param name="activationFunction"></param>
        /// <param name="filepath"></param>
        /// <returns>true if successful; otherwise false</returns>
        public bool CreateAndSaveAnn(int[] numNeuronsPerLayer, ANN.ActivationFunction activationFunction, double alpha, double beta, string filepath)
        {
            int res = klu_createAndSaveAnn(numNeuronsPerLayer, numNeuronsPerLayer.Count(), (int)activationFunction, alpha, beta, filepath);
            return res == 1;
        }
        #endregion

        #region Create and free capture
#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        private static extern int klu_createCapture();

        /// <summary>
        /// Creates a camera capture inside the DLL. If there already is a camera capture,
        /// it will be used without creating a new one if this method is called twice.
        /// </summary>
        /// <returns>true if successful; otherwise false</returns>
        public bool CreateCapture()
        {
            return (klu_createCapture() == 1);
        }        

        /// <summary>
        /// 
        /// </summary>
#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        private static extern int klu_freeCapture();

        /// <summary>
        /// 
        /// </summary>
        public bool FreeCapture()
        {
            return (klu_freeCapture() == 1);
        }
        #endregion

        #region Convert binary OpenCV data to "System.Drawing.Bitmap"
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
                if (imageData == null)
                {
                    return;
                }

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
        #endregion

        #region Process capture image
#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        private static extern int klu_processCaptureImage(
            [In, MarshalAs(UnmanagedType.LPStruct)] ProcessOptions processOptions,
            [Out, MarshalAs(UnmanagedType.LPStruct)] FaceFeaturePoints ffp);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if successful; otherwise false</returns>
        public bool ProcessCaptureImage(ref ProcessOptions options, ref FaceFeaturePoints ffp)
        {
            return (klu_processCaptureImage(options, ffp) == 1);
        }
        #endregion

        #region Load ANN
#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        private static extern int klu_loadAnn([In, MarshalAs(UnmanagedType.LPStr)] string filepath);

        public bool LoadANN(string filepath)
        {
            return (klu_loadAnn(filepath) == 1);
        }
        #endregion

        #region Load ANN
#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        private static extern int klu_saveAnn([In, MarshalAs(UnmanagedType.LPStr)] string filepath);

        public bool SaveANN(string filepath)
        {
            return (klu_saveAnn(filepath) == 1);
        }
        #endregion

        #region Process still image
#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        private static extern int klu_processStillImage(
            [In, MarshalAs(UnmanagedType.LPStr)] string filepath,
            [In, MarshalAs(UnmanagedType.LPStruct)] ProcessOptions processOptions,
            [Out, MarshalAs(UnmanagedType.LPStruct)] FaceFeaturePoints ffp);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filepath">Path to the file that shall be processed</param>
        /// <returns>true if successful; otherwise false</returns>
        public bool ProcessStillImage(string filepath, ref ProcessOptions options, ref FaceFeaturePoints ffp)
        {
            return (klu_processStillImage(filepath, options, ffp) == 1);
        }
        #endregion

        #region Get last processed image and it's dims
#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        unsafe private static extern int klu_getLastProcessedImage(byte** data, int* width, int* height, int* nChannels, int* widthStep);
        
        /// <summary>
        /// Writes the last processed image into the bitmap.
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns>true if successful; otherwise false</returns>
        public bool GetLastProcessedImage(ref System.Drawing.Bitmap bitmap)
        {
            int res = 0;
            unsafe
            {
                int width = 0, height = 0, widthStep = 0, channels = 0;
                byte* imageData;
                res = klu_getLastProcessedImage(&imageData, &width, &height, &channels, &widthStep);
                //Console.WriteLine("klu_getLastProcessedImage " + res);
                if (res == 1)
                {
                    //System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(width, height);
                    convertToBitmap2(ref bitmap, ref imageData, width, height, channels, widthStep);
                }
            }
            return res == 1;
        }

#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        private static extern int klu_getLastProcessedImageDims(out int width, out int height);

        /// <summary>
        /// Stores the width and height of the last processed image.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns>true if successful; otherwise false</returns>
        public bool GetLastProcessedImageDims(ref int width, ref int height)
        {
            int res = klu_getLastProcessedImageDims(out width, out height);
            return res == 1;
        }
        #endregion

        #region Set a bitmap as the source of an image or an image brush.

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        private static extern bool DeleteObject(IntPtr hObject);

        /// <summary>
        /// Implicitly converts the old System.Drawing.Bitmap (GDI) "bitmap" to a new
        /// System.Windows.Media.Imaging.BitmapSource (DirectX) and sets the
        /// "Source" attribute of the "image" to the result of the conversion.
        /// 
        /// Use this function if your are programming with WPF.
        /// </summary>
        /// <param name="image">The WPF image control</param>
        /// <param name="bitmap">The System.Drawing.Bitmap object to set as the image's "Source" attribute</param>
        public bool SetWpfImageFromBitmap(ref System.Windows.Controls.Image image, ref System.Drawing.Bitmap bitmap)
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

            return true;
        }

        public bool SetImageBrushFromBitmap(ref System.Windows.Media.ImageBrush image, ref System.Drawing.Bitmap bitmap)
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

            return true;
        }
        #endregion

        #region Train ANN
#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        private static extern int klu_trainAnn(
            [In, MarshalAs(UnmanagedType.LPStruct)] TrainOptions options,
            int numTrainingSets,
            [In, MarshalAs(UnmanagedType.LPArray)] float[] inputs,
            int numInputNeurons,
            [In, MarshalAs(UnmanagedType.LPArray)] float[] outputs,
            int numOutputNeurons);

        /// <summary>
        /// Trains the ANN that has been previously been loaded using "LoadANN()"
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public bool TrainAnn(TrainOptions options, int numTrainingSets, float[] inputs, float[] outputs)
        {
            //const int numTrainingSets = 4;
            int numInputLayers = inputs.Count() / numTrainingSets;
            int numOutputLayers = 1;
            //float[] inputs = new float[numTrainingSets * numInputLayers];
            //float[] outputs = new float[numTrainingSets * numOutputLayers];

            return (klu_trainAnn(options, numTrainingSets, inputs, numInputLayers, outputs, numOutputLayers) == 1);
        }
        #endregion  
        
        #region Predict ANN output
#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        private static extern int klu_predictAnn(
                                    [In, MarshalAs(UnmanagedType.LPArray)] float[] inputs,
                                    int numInputNeurons,
                                    [In, MarshalAs(UnmanagedType.LPArray)] float[] results,
                                    int numResults);

        /// <summary>
        /// Uses all the inputs for prediction.
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        public bool PredictANN(float[] inputs, float[] results)
        {
            return (klu_predictAnn(inputs, inputs.Count(), results, results.Count()) == 1);
        }

        /// <summary>
        /// Uses only "numInputs" for prediction.
        /// TODO: (Ko) Find out if this might come in handy if doing 7 out of 10 validation?
        /// </summary>
        /// <param name="inputs"></param>
        /// <param name="numInputs"></param>
        /// <param name="results"></param>
        /// <returns></returns>
        public bool PredictANN(float[] inputs, int numInputs, float[] results, int numResults)
        {
            return (klu_predictAnn(inputs, numInputs, results, numResults) == 1);
        }
        #endregion

        #region Interface to "low level" VFW configuration dialogs
#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        private static extern int klu_configureCaptureDialog();

        /// <summary>
        /// Instructs the DLL to open the VFW dialog for the capture device.
        /// If you want to change the resolution and related parameters, you must call "ConfigureCaptureResolutionDialog"
        /// If there isn't a capture device, this function will returns false.
        /// </summary>
        /// <returns></returns>
        public bool ConfigureCaptureDialog()
        {
            return (klu_configureCaptureDialog() == 1);
        }

#if DEBUG
        [DllImport(@"..\..\..\Debug\klulib.dll")]
#else
        [DllImport(@"..\..\..\Release\klulib.dll")]
#endif
        private static extern int klu_configureCaptureResolutionDialog();

        /// <summary>
        /// Instructs the DLL to open the VFW dialog for the capture device.
        /// If you want to change the brightness and related parameters, you must call "ConfigureCaptureDialog"
        /// If there isn't a capture device, this function will returns false.
        /// </summary>
        /// <returns></returns>
        public bool ConfigureCaptureResolutionDialog()
        {
            return (klu_configureCaptureResolutionDialog() == 1);
        }
        #endregion
    }
}
