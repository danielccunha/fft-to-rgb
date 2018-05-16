﻿using System;
using System.Numerics;

namespace FFTtoRGB
{
    public static class Calc
    {
        /// <summary>
        /// Calculate the FFT
        /// </summary>
        /// <returns>FFT's double array</returns>
        public static double[] FFT(double[] data)
        {
            double[] fft = new double[data.Length];
            Complex[] fftComplex = new Complex[data.Length];

            for (int i = 0; i < data.Length; i++)
                fftComplex[i] = new Complex(data[i], 0.0);

            Accord.Math.FourierTransform.FFT(fftComplex, Accord.Math.FourierTransform.Direction.Forward);

            for (int i = 0; i < data.Length; i++)
                fft[i] = Math.Log10(fftComplex[i].Magnitude);

            return fft;
        }
    }
}
