﻿// Copyright 2016 Stefan Solnts// Copyright 2012, 2013, 2014 Derek J. Bailey
// Modified work copyright 2016 Stefan Solntsev
//
// This file (Spectrum.cs) is part of MassSpectrometry.
//
// MassSpectrometry is free software: you can redistribute it and/or modify it
// under the terms of the GNU Lesser General Public License as published
// by the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// MassSpectrometry is distributed in the hope that it will be useful, but WITHOUT
// ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or
// FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public
// License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with MassSpectrometry. If not, see <http://www.gnu.org/licenses/>.

using MzLibUtil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Spectra
{
    public abstract class Spectrum<TPeak> : ISpectrum<TPeak>
        where TPeak : IPeak
    {

        #region Protected Fields

        protected TPeak[] peakList;

        #endregion Protected Fields

        #region Private Fields

        protected double yofPeakWithHighestY = double.NaN;

        protected double sumOfAllY = double.NaN;

        protected TPeak peakWithHighestY;

        #endregion Private Fields

        #region Protected Constructors

        /// <summary>
        /// Initializes a new spectrum
        /// </summary>
        /// <param name="x">The m/z's</param>
        /// <param name="y">The intensities</param>
        /// <param name="shouldCopy">Indicates whether the input arrays should be copied to new ones</param>
        protected Spectrum(double[] x, double[] y, bool shouldCopy)
        {
            if (shouldCopy)
            {
                XArray = new double[x.Length];
                YArray = new double[y.Length];
                Array.Copy(x, XArray, x.Length);
                Array.Copy(y, YArray, y.Length);
            }
            else
            {
                XArray = x;
                YArray = y;
            }
            peakList = new TPeak[Count];
        }

        /// <summary>
        /// Initializes a new spectrum from another spectrum
        /// </summary>
        /// <param name="spectrumToClone">The spectrum to clone</param>
        protected Spectrum(ISpectrum<IPeak> spectrumToClone)
            : this(spectrumToClone.XArray, spectrumToClone.YArray, true)
        {
        }

        /// <summary>
        /// Initializes a new spectrum
        /// </summary>
        /// <param name="xy"></param>
        protected Spectrum(double[,] xy)
            : this(xy, xy.GetLength(1))
        {
        }

        protected Spectrum(double[,] xy, int count)
        {
            int length = xy.GetLength(1);

            XArray = new double[count];
            YArray = new double[count];
            Buffer.BlockCopy(xy, 0, XArray, 0, sizeof(double) * count);
            Buffer.BlockCopy(xy, sizeof(double) * length, YArray, 0, sizeof(double) * count);
            peakList = new TPeak[Count];
        }

        #endregion Protected Constructors

        #region Public Properties

        public double FirstX { get { return XArray[0]; } }

        public double LastX { get { return XArray[Count - 1]; } }

        public int Count { get { return XArray.Length; } }

        public double[] XArray { get; private set; }

        public double[] YArray { get; private set; }

        public double YofPeakWithHighestY
        {
            get
            {
                if (double.IsNaN(yofPeakWithHighestY))
                    yofPeakWithHighestY = YArray.Max();
                return yofPeakWithHighestY;
            }
        }

        public double SumOfAllY
        {
            get
            {
                if (double.IsNaN(sumOfAllY))
                    sumOfAllY = YArray.Sum();
                return sumOfAllY;
            }
        }

        public DoubleRange Range
        {
            get
            {
                return new DoubleRange(FirstX, LastX);
            }
        }

        public TPeak PeakWithHighestY
        {
            get
            {
                if (peakWithHighestY == null)
                    peakWithHighestY = this[Array.IndexOf(YArray, YArray.Max())];
                return peakWithHighestY;
            }
        }

        #endregion Public Properties

        #region Public Indexers

        public virtual TPeak this[int index]
        {
            get
            {
                if (peakList[index] == null)
                    peakList[index] = (TPeak)Activator.CreateInstance(typeof(TPeak), new object[] { XArray[index], YArray[index] });
                return peakList[index];
            }
        }

        #endregion Public Indexers

        #region Public Methods

        public override string ToString()
        {
            return string.Format("{0} (Peaks {1})", Range, Count);
        }

        public Spectrum<TPeak> NewSpectrumFilterByNumberOfMostIntense(int topNPeaks)
        {
            var ok = FilterByNumberOfMostIntense(topNPeaks);
            return CreateSpectrumFromTwoArrays(ok.Item1, ok.Item2, false);
        }

        public abstract Spectrum<TPeak> CreateSpectrumFromTwoArrays(double[] item1, double[] item2, bool v);

        public Spectrum<TPeak> NewSpectrumWithRangeRemoved(double minX, double maxX)
        {
            var ok = WithRangeRemoved(minX, maxX);
            return CreateSpectrumFromTwoArrays(ok.Item1, ok.Item2, false);
        }

        public Spectrum<TPeak> NewSpectrumWithRangesRemoved(IEnumerable<DoubleRange> xRanges)
        {
            var ok = WithRangesRemoved(xRanges);
            return CreateSpectrumFromTwoArrays(ok.Item1, ok.Item2, false);
        }

        public Spectrum<TPeak> NewSpectrumExtract(double minX, double maxX)
        {
            var ok = Extract(minX, maxX);
            return CreateSpectrumFromTwoArrays(ok.Item1, ok.Item2, false);
        }

        public Spectrum<TPeak> NewSpectrumFilterByY(double minY, double maxY)
        {
            var ok = FilterByY(minY, maxY);
            return CreateSpectrumFromTwoArrays(ok.Item1, ok.Item2, false);
        }

        public Spectrum<TPeak> NewSpectrumApplyFunctionToX(Func<double, double> convertor)
        {
            var ok = ApplyFunctionToX(convertor);
            return CreateSpectrumFromTwoArrays(ok.Item1, ok.Item2, false);
        }

        public virtual double[,] CopyTo2DArray()
        {
            double[,] data = new double[2, Count];
            const int size = sizeof(double);
            Buffer.BlockCopy(XArray, 0, data, 0, size * Count);
            Buffer.BlockCopy(YArray, 0, data, size * Count, size * Count);
            return data;
        }

        public ISpectrum<TPeak> NewSpectrumFilterByY(DoubleRange yRange)
        {
            return NewSpectrumFilterByY(yRange.Minimum, yRange.Maximum);
        }

        public ISpectrum<TPeak> NewSpectrumWithRangeRemoved(DoubleRange xRange)
        {
            return NewSpectrumWithRangeRemoved(xRange.Minimum, xRange.Maximum);
        }

        public ISpectrum<TPeak> NewSpectrumExtract(DoubleRange xRange)
        {
            return NewSpectrumExtract(xRange.Minimum, xRange.Maximum);
        }

        public TPeak GetClosestPeak(double x)
        {
            return this[GetClosestPeakIndex(x)];
        }

        public double GetClosestPeakXvalue(double x)
        {
            return XArray[GetClosestPeakIndex(x)];
        }

        public IEnumerator<TPeak> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int NumPeaksWithinRange(double minX, double maxX)
        {
            int index = Array.BinarySearch(XArray, minX);

            if (index < 0)
                index = ~index;

            if (index >= Count)
                return 0;

            int startingIndex = index;

            // TODO: replace by binary search here as well
            while (index < Count && XArray[index] <= maxX)
                index++;

            return index - startingIndex;
        }

        #endregion Public Methods

        #region Protected Methods

        protected Tuple<double[], double[]> ApplyFunctionToX(Func<double, double> convertor)
        {
            double[] modifiedXarray = new double[Count];
            for (int i = 0; i < Count; i++)
                modifiedXarray[i] = convertor(XArray[i]);
            double[] newYarray = new double[YArray.Length];
            Array.Copy(YArray, newYarray, YArray.Length);
            return new Tuple<double[], double[]>(modifiedXarray, newYarray);
        }

        protected Tuple<double[], double[]> WithRangeRemoved(double minX, double maxX)
        {
            int count = Count;

            // Peaks to remove
            HashSet<int> indiciesToRemove = new HashSet<int>();

            int index = Array.BinarySearch(XArray, minX);
            if (index < 0)
                index = ~index;

            while (index < count && XArray[index] <= maxX)
            {
                indiciesToRemove.Add(index);
                index++;
            }

            // The size of the cleaned spectrum
            int cleanCount = count - indiciesToRemove.Count;

            // Create the storage for the cleaned spectrum
            double[] newXarray = new double[cleanCount];
            double[] newYarray = new double[cleanCount];

            // Transfer peaks from the old spectrum to the new one
            int j = 0;
            for (int i = 0; i < count; i++)
            {
                if (indiciesToRemove.Contains(i))
                    continue;
                newXarray[j] = XArray[i];
                newYarray[j] = YArray[i];
                j++;
            }
            return new Tuple<double[], double[]>(newXarray, newYarray);
        }

        protected Tuple<double[], double[]> WithRangesRemoved(IEnumerable<DoubleRange> xRanges)
        {
            int count = Count;

            // Peaks to remove
            HashSet<int> indiciesToRemove = new HashSet<int>();

            // Loop over each range to remove
            foreach (DoubleRange range in xRanges)
            {
                double min = range.Minimum;
                double max = range.Maximum;

                int index = Array.BinarySearch(XArray, min);
                if (index < 0)
                    index = ~index;

                while (index < count && XArray[index] <= max)
                {
                    indiciesToRemove.Add(index);
                    index++;
                }
            }

            // The size of the cleaned spectrum
            int cleanCount = count - indiciesToRemove.Count;

            // Create the storage for the cleaned spectrum
            double[] newXarray = new double[cleanCount];
            double[] newYarray = new double[cleanCount];

            // Transfer peaks from the old spectrum to the new one
            int j = 0;
            for (int i = 0; i < count; i++)
            {
                if (indiciesToRemove.Contains(i))
                    continue;
                newXarray[j] = XArray[i];
                newYarray[j] = YArray[i];
                j++;
            }

            return new Tuple<double[], double[]>(newXarray, newYarray);
        }

        protected Tuple<double[], double[]> FilterByY(double minY, double maxY)
        {
            int count = Count;
            double[] newXarray = new double[count];
            double[] newYarray = new double[count];
            int j = 0;
            for (int i = 0; i < count; i++)
            {
                double intensity = YArray[i];
                if (intensity >= minY && intensity < maxY)
                {
                    newXarray[j] = XArray[i];
                    newYarray[j] = intensity;
                    j++;
                }
            }

            if (j != count)
            {
                Array.Resize(ref newXarray, j);
                Array.Resize(ref newYarray, j);
            }

            return new Tuple<double[], double[]>(newXarray, newYarray);
        }

        protected Tuple<double[], double[]> Extract(double minX, double maxX)
        {
            int index = GetClosestPeakIndex(minX);
            if (this[index].X < minX)
                index++;

            int count = Count;
            double[] newXarray = new double[count];
            double[] newYarray = new double[count];
            int j = 0;

            while (index < Count && XArray[index] <= maxX)
            {
                newXarray[j] = XArray[index];
                newYarray[j] = YArray[index];
                index++;
                j++;
            }

            Array.Resize(ref newXarray, j);
            Array.Resize(ref newYarray, j);

            return new Tuple<double[], double[]>(newXarray, newYarray);
        }

        protected Tuple<double[], double[]> FilterByNumberOfMostIntense(int topNPeaks)
        {
            double[] newXarray = new double[XArray.Length];
            double[] newYarray = new double[YArray.Length];
            Array.Copy(XArray, newXarray, XArray.Length);
            Array.Copy(YArray, newYarray, YArray.Length);

            Array.Sort(newYarray, newXarray, Comparer<double>.Create((i1, i2) => i2.CompareTo(i1)));

            double[] newXarray2 = new double[topNPeaks];
            double[] newYarray2 = new double[topNPeaks];
            Array.Copy(newXarray, newXarray2, topNPeaks);
            Array.Copy(newYarray, newYarray2, topNPeaks);

            Array.Sort(newXarray2, newYarray2);
            return new Tuple<double[], double[]>(newXarray2, newYarray2);
        }

        protected int GetClosestPeakIndex(double targetX)
        {
            if (Count == 0)
                throw new IndexOutOfRangeException("No peaks in spectrum!");

            int index = Array.BinarySearch(XArray, targetX);
            if (index >= 0)
                return index;
            index = ~index;

            int indexm1 = index - 1;

            if (index >= Count && indexm1 >= 0)
            {
                return indexm1;
            }
            if (index == 0)
            {
                // only the index can be closer
                return index;
            }

            double p1 = XArray[indexm1];
            double p2 = XArray[index];

            if (targetX - p1 > p2 - targetX)
                return index;
            return indexm1;
        }

        #endregion Protected Methods

        #region Private Methods

        ISpectrum<TPeak> ISpectrum<TPeak>.NewSpectrumFilterByNumberOfMostIntense(int topNPeaks)
        {
            throw new NotImplementedException();
        }

        ISpectrum<TPeak> ISpectrum<TPeak>.NewSpectrumExtract(double minX, double maxX)
        {
            throw new NotImplementedException();
        }

        ISpectrum<TPeak> ISpectrum<TPeak>.NewSpectrumWithRangesRemoved(IEnumerable<DoubleRange> xRanges)
        {
            throw new NotImplementedException();
        }

        ISpectrum<TPeak> ISpectrum<TPeak>.NewSpectrumWithRangeRemoved(double minX, double maxX)
        {
            throw new NotImplementedException();
        }

        ISpectrum<TPeak> ISpectrum<TPeak>.NewSpectrumFilterByY(double minY, double maxY)
        {
            throw new NotImplementedException();
        }

        ISpectrum<TPeak> ISpectrum<TPeak>.NewSpectrumApplyFunctionToX(Func<double, double> convertor)
        {
            throw new NotImplementedException();
        }

        #endregion Private Methods

    }
}