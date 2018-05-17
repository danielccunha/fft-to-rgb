﻿using System;
using System.Linq;
using FFTtoRGB.FFT;
using FFTtoRGB.Color;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;

namespace FFTtoRGB
{
    /// <summary>
    /// Generates the RGB using the FFTProvider
    /// </summary>
    public class RGBGenerator
    {
        /// <summary>
        /// FFT Provider
        /// </summary>
        private FFTProvider FFTProvider { get; set; }

        /// <summary>
        /// FFT Provider's config
        /// </summary>
        private FFTProviderConfig FFTProviderSettings { get; set; }

        /// <summary>
        /// Frequencies array
        /// </summary>
        private double[] Frequencies { get; set; }

        /// <summary>
        /// RGB colors distribution 
        /// </summary>
        public ColorSettings Settings { get; set; }

        /// <summary>
        /// Biggest value generated by the FFT used to calculate the RGB colors
        /// </summary>
        public double MaxValue { get; private set; } = 0;

        public RGBGenerator() => InitGenerator(new FFTProviderConfig());
        public RGBGenerator(FFTProviderConfig config) => InitGenerator(config);

        private void InitGenerator(FFTProviderConfig config)
        {
            FFTProvider = new FFTProvider(config);
            FFTProviderSettings = config;
            Settings = new ColorSettings();

            Frequencies = FFTProvider.GetFreqArray();
        }

        /// <summary>
        /// Normalize the FFT array, making all values positive, adding the abs of the min value to all the others.
        /// The reason is that some values of the FFT may be negative, what prejudices the RGB generating.
        /// </summary>
        /// <param name="data">FFT Array</param>
        /// <returns>Normalized Array</returns>
        private double[] NormalizeArray(double[] data)
        {
            var minValue = data.Min();

            if (minValue < 0)
            {
                var N = Math.Abs(minValue);
                for (int i = 0; i < data.Length; i++)
                    data[i] += N;
            }

            return data;
        }

        /// <summary>
        /// Calculate the RGB color based on the FFT array
        /// </summary>
        /// <param name="FFT">FFT double array</param>
        /// <returns>Generated RGB color</returns>
        public RGB GenerateColor(double[] FFT)
        {
            if (FFT.Max() > MaxValue)
                MaxValue = FFT.Max();

            int pX = (int)Math.Floor(FFT.Length * Settings.X);
            int pY = (int)Math.Floor(FFT.Length * Settings.Y);

            // Ordered values
            var X = (int)Math.Ceiling(Calc.SubArray(FFT, 0, pX).Average().Map(0, MaxValue, 0, 255));
            var Y = (int)Math.Ceiling(Calc.SubArray(FFT, pX, pY - pX).Average().Map(0, MaxValue, 0, 255));
            var Z = (int)Math.Ceiling(Calc.SubArray(FFT, pY, FFT.Length - pY).Average().Map(0, MaxValue, 0, 255));

            return new RGB(X, Y, Z, Settings.Order);
        }

        /// <summary>
        /// CancellationTokenSource to control the task
        /// </summary>
        private CancellationTokenSource TokenSource { get; set; }

        /// <summary>
        /// Start generating the colors and sending them to the Arduino
        /// </summary>
        public void Run()
        {
            TokenSource = new CancellationTokenSource();

            var RunningTask = new Task(() =>
            {
                FFTProvider.StartRecording();
                var time = (int)(FFTProvider.BufferSize / (double)FFTProvider.Rate * 1000);

                while (!TokenSource.IsCancellationRequested)
                {
                    var NRM = NormalizeArray(FFTProvider.Read());

                    if (double.IsInfinity(NRM[0]) || double.IsNaN(NRM[0]))
                        continue;

                    try
                    {
                        var color = GenerateColor(NRM);
                        Console.WriteLine(color);
                        
                        // TODO Send the generated color to Arduino
                    }
                    catch (InvalidColorValueException)
                    {
                        // TODO Fix Exception
                        break;
                    }
                    Thread.Sleep(time);
                }
            }, TokenSource.Token);
            RunningTask.Start();
        }

        public void Stop() => TokenSource.Cancel();
    }
}
