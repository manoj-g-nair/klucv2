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
    #region Types for interacting with the DLL

    public enum TrainingTermination
    {
        MaxIterationTermination = 0,
        EpsilonTermination = 1
    };

    [StructLayout(LayoutKind.Sequential)]
    public class TerminationCriteria
    {
        public int TerminationType; // "Or" TrainingTermination flags using "|"
        public float Epsilon;
        public int MaxIteration;
    };

    public enum TrainingAlgorithm
    {
        // LeCun, L. Bottou, G.B. Orr and K.-R. Muller,
        // “Efficient backprop”,
        // in Neural Networks—Tricks of the Trade, Springer Lecture Notes in Computer Sciences 1524, pp.5-50, 1998.
        BackpropAlgorithm = 0,
        // Riedmiller and H. Braun,
        // “A Direct Adaptive Method for Faster Backpropagation Learning: The RPROP Algorithm”,
        // Proc. ICNN, San Francisco (1993)
        RpropAlgorithm = 1
    };

    [StructLayout(LayoutKind.Sequential)]
    public class TrainOptions
    {
        public TerminationCriteria Termination;
        public TrainingAlgorithm Algorithm;

        // backpropagation parameters
        public double BackpropDeltaWeightScale;    // default = 0.1
        public double BackpropMomentumScale;       // default = 0.1

        // rprop parameters
        public double RpropDeltaWeight0;           // default = 1
        public double RpropDeltaWeightPlus;        // default = 1.2
        public double RpropDeltaWeightMinus;       // default = 0.5
        public double RpropDeltaWeightMin;         // default = FLT_EPSILON (smallest floating point number such that (1.0+FLT_EPSILON != 1.0)
        public double RpropDeltaWeightMax;         // default = 50
    };

    [StructLayout(LayoutKind.Sequential)]
    public class ProcessOptions
    {
        public Int32 DrawAnthropometricPoints;
        public Int32 DrawSearchRectangles;
        public Int32 DrawFaceRectangle;
        public Int32 DrawDetectionTime;
        public Int32 DrawFeaturePoints;
        public Int32 DoEyeProcessing;
        public Int32 DoMouthProcessing;
        public Int32 DoVisualDebug;
    };

    [StructLayout(LayoutKind.Sequential)]
    public class KluPoint
    {
        public Int32 X;
        public Int32 Y;
    };

    [StructLayout(LayoutKind.Sequential)]
    public class KluRectangle
    {
        public Int32 X;
        public Int32 Y;
        public Int32 Width;
        public Int32 Height;
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
        public EyeFeaturePoints LeftEye;
        public EyeFeaturePoints RightEye;
        public MouthFeaturePoints Mouth;
        public KluRectangle FaceRectangle;
    };
    #endregion
}