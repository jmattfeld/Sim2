using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;


// This is the code for your desktop app.
// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.

namespace TransTemp
{
    public partial class Form1 : Form
    {

        public OpenFileDialog rfd = new OpenFileDialog();
        private StreamReader stDataFile;

        public OpenFileDialog crfd = new OpenFileDialog();
        private StreamReader stCalFile;

        public SaveFileDialog sfd = new SaveFileDialog();
        private StreamWriter stOutFile;

        public SaveFileDialog mmfd = new SaveFileDialog();
        private StreamWriter stMaxMinFile;


        // FILTER_LEN 10 / cutoff freq=0.010417 Hz
        //        double[] coeffs = new double[] { 0.016154,0.037930,0.093105,0.155901,0.196910,
        //            0.196910,0.155901,0.093105,0.037930,0.016154 };

        double[] coeffs = new double[] { 0.035697, 0.241063, 0.446480, 0.241063, 0.035697 };

        const int FILTER_LEN = 5;
        const uint INPUT_SAMPLE_LEN = 1;
        const uint INPUT_BUFFER_LEN = (FILTER_LEN - 1 + INPUT_SAMPLE_LEN);
        float[] sampleBuffer = new float[INPUT_BUFFER_LEN];
        uint idx;

        // Master calibration tables arrays
        int[] nMLen = new int[64];              // Current length of master point table
        float[,] fMTemp = new float[64, 450];   	// Temperatures
        float[,] fMPress = new float[64, 450];		// Pressures
        long[,] lMCounts = new long[64, 450];       // Counts

        int nNumCalT = 15;            // Number of calibration temperature points
        int nNumCalP = 25;          // Number of calibration pressure points

        float[] fTemp = new float[64];

        // Low temp pressures for current temperature plane
        float[] fLCTemp = new float[25];    // Temperatures
        float[] fLCPress = new float[25];   // Pressures
        long[] lLCCounts = new long[25];    // Counts

        // High temp pressures for current temperature plane
        float[] fHCTemp = new float[25];    // Temperatures
        float[] fHCPress = new float[25];   // Pressures
        long[] lHCCounts = new long[25];   // Counts

        // Current temperature plane
        float[,] fCPress = new float[64, 25];	// Pressures
        long[,] lCCounts = new long[64, 25];	    // Counts
        float[,] fCCounts = new float[64, 25];	// Counts in float form
        int[] nCNumPress = new int[64];		// Number of pressures in the CTP

        // Current Temperature Plane Data Structure, used by pressure conversion
        float[] fMNdxCtp = new float[64];
        float[] fBNdxCtp = new float[64];
        float[,] fMctp = new float[64, 25];
        float[,] fBctp = new float[64, 25];


        // Low temp pressures for current press plane
        float[] fLPTemp = new float[25];    // Temperatures
        float[] fLPPress = new float[25];   // Pressures
        long[] lLPCounts = new long[25];    // Counts

        // High temp pressures for current temperature plane
        float[] fHPTemp = new float[25];    // Temperatures
        float[] fHPPress = new float[25];   // Pressures
        long[] lHPCounts = new long[25];   // Counts

        // Current pressures plane
        float[,] fPPress = new float[64, 25];   // Pressures
        float[,] fPTemps = new float[64, 25];	// Temperatures
        long[,] lPCounts = new long[64, 25];	    // Counts
        float[,] fPCounts = new float[64, 25];	// Counts in float form
        int[] nPNumTemps = new int[64];		// Number of temperatures in the CPP

        // Current pressures Plane Data Structure, used by pressure conversion
        float[] fMNdxCpp = new float[64];
        float[] fBNdxCpp = new float[64];
        float[,] fMcpp = new float[64, 25];
        float[,] fBcpp = new float[64, 25];

        int nWorkingChan = 0;           // WOrking channel zero based
        float fWorkingPress = 0.0F;     // WOrking channel zero based

//        int[] nSampleFrame = new int[5000];
//        float[] fSamplePress = new float[5000];

        float[] fTempOld = new float[10] {0.0F,0.0F,0.0F,0.0F,0.0F,0.0F,0.0F,0.0F,0.0F,0.0F};

        //temp history and filtered history
        float[] fTempHist = new float[40];
        float[] fTempHistF1 = new float[40];
        float[] fTempHistF2 = new float[40];
        float[] fTempHistF3 = new float[40];
        float[] fTempHistF4 = new float[40];
        float[] fTempHistF5 = new float[40];
        float[] fTempHistF6 = new float[40];
        float[] fTempHistF7 = new float[40];
        float[] fTempHistF8 = new float[40];

        float[] fTempDerHist = new float[40];
        float[] fTempDerHistF1 = new float[40];
        float[] fTempDerHistF2 = new float[40];
        float[] fTempDerHistF3 = new float[40];
        float[] fTempDerHistF4 = new float[40];
        float[] fTempDerHistF5 = new float[40];
        float[] fTempDerHistF6 = new float[40];
        float[] fTempDerHistF7 = new float[40];
        float[] fTempDerHistF8 = new float[40];

        float[] fTempSecDerHist = new float[20];
        float[] fTempSecDerHistF1 = new float[20];
        float[] fTempSecDerHistF2 = new float[20];
        float[] fTempSecDerHistF3 = new float[20];
        float[] fTempSecDerHistF4 = new float[20];
        float[] fTempSecDerHistF5 = new float[20];
        float[] fTempSecDerHistF6 = new float[20];
        float[] fTempSecDerHistF7 = new float[20];
        float[] fTempSecDerHistF8 = new float[20];

        float fMaxDer;
        float fMinDer;
        float[] fMaxCor = new float[64];
        float[] fMinCor = new float[64];

        int[] nGotTemp = new int[8];
        int[] nGotCounts = new int[64];

        int[] nFrameAtStepT = new int[8];
        int[] nFrameAtStepC = new int[64];


        float[] fBaseTemp = new float[8];
        int[] nBaseCounts = new int[64];
        int nStartSample = 2575;
//        float fTempInc = 0.08F;
//        int nCountsInc = -200;

        float fTempInc = 0.08F;
        int nCountsInc = -200;

        private float RollingAvgFilter(float[] fTempHistory, int samples)
        {
            float fRet;
            int s;
            float fSum = 0.0F;

            for (s=0; s<samples; s++)
            {
                fSum = fSum + fTempHistory[s];
            }

            fRet = fSum / (float)samples;

            return fRet;
        }

        private void UpdateHistory(float newvalue, float[] fTempHistory)
        {
            // Shift old temp samples
            for (int i = 19; i > 0; i--)
            {
                fTempHistory[i] = fTempHistory[i - 1];
            }
            fTempHistory[0] = newvalue;

            return;
        }







        float[] fDerTempOld = new float[4] { 0.0F, 0.0F, 0.0F, 0.0F };

        private float DerAvgFilter(float tempin)
        {
            float fRet;

            fRet = (tempin + fDerTempOld[0] + fDerTempOld[1] + fDerTempOld[2]) / 4.0F;

//            fDerTempOld[3] = fDerTempOld[2];
            fDerTempOld[2] = fDerTempOld[1];
            fDerTempOld[1] = fDerTempOld[0];
            fDerTempOld[0] = tempin;

            return fRet;
        }

        private void SaveOld(float tempin, float[] fTempHist)
        {
            // Shift old temp samples
            for (int i = 19 ; i > 0 ; i--) {
                fTempHist[i] = fTempHist[i-1];
            }
            fTempHist[0] = tempin;

            return;
        }

        private void SaveOldDer(float tempin)
        {

            // Shift old temp samples
            for (int i = 19; i > 0; i--)
            {
                fTempDerHist[i] = fTempDerHist[i - 1];
            }
            fTempDerHist[0] = tempin;

            return;
        }


        private float Derivative(float[] fTempHist, int delta)
        {
            float fRet;
            fRet = fTempHist[0] - fTempHist[delta];
            return fRet;
        }


        private float Gain(float tempin)
        {
            float fRet;

            if (tempin < 45.0F)
                fRet = 0.1F;
            else
                fRet = 2.0F;

            return fRet;
        }

        float[] fSecDerTempOld = new float[4] { 0.0F, 0.0F, 0.0F, 0.0F };
        private float SecDerivative(float tempin)
        {
            float fRet;

#if(false)
            int cntp = 0;
            int cntn = 0;



            for (int i=1; i<10; i++)
            {
                if (fTempDerHist[i] > fTempDerHist[i-1])
                {
                    cntp++;
                }

                if (fTempDerHist[i] < fTempDerHist[i - 1])
                {
                    cntn++;
                }

            }

            if (cntp > 9) fRet = 0.21F;
            else if (cntn > 9) fRet = -0.21F;
            else fRet = 0.0F;
#endif

            fRet = fTempDerHist[0] - fTempDerHist[1];

            return fRet;
        }


        private float SecDerAvgFilter(float tempin)
        {
            float fRet;

            fRet = (tempin + fSecDerTempOld[0] + fSecDerTempOld[1] + fSecDerTempOld[2]) / 4.0F;

            //            fDerTempOld[3] = fDerTempOld[2];
            fSecDerTempOld[2] = fSecDerTempOld[1];
            fSecDerTempOld[1] = fSecDerTempOld[0];
            fSecDerTempOld[0] = tempin;

            return fRet;
        }


#if(false)
        
        private float LowPassFilter(float tempin)
        {
            float acc = 0;  // accumulator
            int k;          // coeff idx

            // circular buffer index
            uint n = idx % INPUT_BUFFER_LEN;

            // write new sample to the index of the oldest sample
            sampleBuffer[n] = tempin;

            // apply filter to the input sample:
            // y[n] = sum_{k=0}..{N-1}(h(k) * x(n-k))
            for (k = 0; k < FILTER_LEN; k++)
            {
                acc += (float)coeffs[k] * sampleBuffer[(n + INPUT_BUFFER_LEN - k) % INPUT_BUFFER_LEN];
            }

            // move the index for next function call and return sum
            idx++;
            return acc;
        }
#endif

        public Form1()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // Click on the link below to continue learning how to build a desktop app using WinForms!
//            System.Diagnostics.Process.Start("http://aka.ms/dotnet-get-started-desktop");

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Thanks!");
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // Processes one line at a time
        float[] fPressOdd = new float[64];
        float[] fPressEven = new float[64];
        float fTempOdd;

        float[,] fACorPress = new float[64,10000];
        int nACorPressNdx;

        private void ProcessData(string[] token)
        {
            int nFrame;
            float fTemp;
            float[] fAllTemps = new float[8];
            float fAvgTemp;
            float fPress;
            int lCnts;
            float fFilteredTemp;
            float fFilteredTemp1;
            float fFilteredTemp2;
            float fFilteredTemp3;
            float fFilteredTemp4;
            float fFilteredTemp5;
            float fFilteredTemp6;
            float fFilteredTemp7;
            float fFilteredTemp8;
            float fCorrPress;
            float fUnPress;
            float fFilPress;
            float fDer;
            float fDer1;
            float fDer2;
            float fDer3;
            float fDer4;
            float fDer5;
            float fDer6;
            float fDer7;
            float fDer8;

            float fDerUn;
            float fSecDer;
            float fSecDer1;
            float fSecDer2;
            float fSecDer3;
            float fSecDer4;
            float fSecDer5;
            float fSecDer6;
            float fSecDer7;
            float fSecDer8;

            float fSecDerUn;
            float fAdj;


            //            nWorkingChan = Convert.ToInt16(textBox1.Text) - 1;
            //            fWorkingPress = Convert.ToSingle(textBox2.Text);

            nFrame = Convert.ToInt16(token[0]);

            if ((nFrame < 2000) || (nFrame > 3500)) return;

            fAvgTemp = 0.0F;
            for (int ts = 0; ts < 8; ts++)
            {
                fAllTemps[ts] = Convert.ToSingle(token[3 + ts]);
                fAvgTemp = fAvgTemp + fAllTemps[ts];
            }
            fAvgTemp = fAvgTemp / 8.0F;
            //            fTemp = fAvgTemp;

            //            fTemp = (fAllTemps[0] + fAllTemps[1]) / 2.0F;
            fTemp = fAllTemps[0];   // 0 fastest, 7 slowest

#if (false)
            // Handle odd frames
            if ((nFrame % 2) == 1)
            {
                // Odd frame
                fTempOdd = fTemp;
                for (int c = 0; c < 64; c++)
                {
                    fPressOdd[c] = Convert.ToSingle(token[c + 12]);
                }
                return;
            }
            else
            {
                // Even frame
                fTemp = (fTempOdd + fTemp) / 2.0F;
                for (int c = 0; c < 64; c++)
                {
                    fPressEven[c] = (fPressOdd[c] + Convert.ToSingle(token[c + 12])) / 2.0F;
                }
            }
#endif

#if (false)
            nFrame = Convert.ToInt16(token[0]);
            fTemp = Convert.ToSingle(token[2]);
            fPress = Convert.ToSingle(token[16]);    // 9
            lCnts = Convert.ToInt32(token[32]);      // 25
#endif

            //            int avgamt = 38; // Was 2, 4
            //            int avgamtd = 4; // Was 2, 4
            //            int deramt = 2;

            int avgamt = 19; // Was 2, 4
            int avgamtd = 2; // Was 2, 4
            int deramt = 1;


            // Save old samples temp history
            UpdateHistory(fTemp, fTempHist);
            fFilteredTemp = RollingAvgFilter(fTempHist, avgamt);

            UpdateHistory(fFilteredTemp, fTempHistF1);
            fFilteredTemp1 = RollingAvgFilter(fTempHistF1, avgamt);

            UpdateHistory(fFilteredTemp1, fTempHistF2);
            fFilteredTemp2 = RollingAvgFilter(fTempHistF2, avgamt);

            UpdateHistory(fFilteredTemp2, fTempHistF3);
            fFilteredTemp3 = RollingAvgFilter(fTempHistF3, avgamt);

            UpdateHistory(fFilteredTemp3, fTempHistF4);
            fFilteredTemp4 = RollingAvgFilter(fTempHistF4, avgamt);

            UpdateHistory(fFilteredTemp4, fTempHistF5);
            fFilteredTemp5 = RollingAvgFilter(fTempHistF5, avgamt);

            UpdateHistory(fFilteredTemp5, fTempHistF6);
            fFilteredTemp6 = RollingAvgFilter(fTempHistF6, avgamt);

            UpdateHistory(fFilteredTemp6, fTempHistF7);
            fFilteredTemp7 = RollingAvgFilter(fTempHistF7, avgamt);

            UpdateHistory(fFilteredTemp7, fTempHistF8);
            fFilteredTemp8 = RollingAvgFilter(fTempHistF8, avgamt);

            // Derivative
            fDerUn = Derivative(fTempHist, deramt);  // Was 1 with .5

            UpdateHistory(fDerUn, fTempDerHist);
            fDer = RollingAvgFilter(fTempDerHist, avgamtd);

            UpdateHistory(fDer, fTempDerHistF1);
            fDer1 = RollingAvgFilter(fTempDerHistF1, avgamtd);

            UpdateHistory(fDer1, fTempDerHistF2);
            fDer2 = RollingAvgFilter(fTempDerHistF2, avgamtd);

            UpdateHistory(fDer2, fTempDerHistF3);
            fDer3 = RollingAvgFilter(fTempDerHistF3, avgamtd);

            UpdateHistory(fDer3, fTempDerHistF4);
            fDer4 = RollingAvgFilter(fTempDerHistF4, avgamtd);

            UpdateHistory(fDer4, fTempDerHistF5);
            fDer5 = RollingAvgFilter(fTempDerHistF5, avgamtd);

            UpdateHistory(fDer5, fTempDerHistF6);
            fDer6 = RollingAvgFilter(fTempDerHistF6, avgamtd);

            UpdateHistory(fDer6, fTempDerHistF7);
            fDer7 = RollingAvgFilter(fTempDerHistF7, avgamtd);

            UpdateHistory(fDer7, fTempDerHistF8);
            fDer8 = RollingAvgFilter(fTempDerHistF8, avgamtd);
#if (false)


            // Second derivative
            fSecDerUn = Derivative(fTempDerHistF3, 5) * 10.0F;

            UpdateHistory(fSecDerUn, fTempSecDerHist);
            fSecDer = RollingAvgFilter(fTempSecDerHist, avgamt);   // Was 4

            UpdateHistory(fSecDer, fTempSecDerHistF1);
            fSecDer1 = RollingAvgFilter(fTempSecDerHistF1, avgamt);   // Was 4

            UpdateHistory(fSecDer1, fTempSecDerHistF2);
            fSecDer2 = RollingAvgFilter(fTempSecDerHistF2, avgamt);   // Was 4

            UpdateHistory(fSecDer2, fTempSecDerHistF3);
            fSecDer3 = RollingAvgFilter(fTempSecDerHistF3, avgamt);   // Was 4

            UpdateHistory(fSecDer3, fTempSecDerHistF4);
            fSecDer4 = RollingAvgFilter(fTempSecDerHistF4, avgamt);   // Was 4

            UpdateHistory(fSecDer4, fTempSecDerHistF5);
            fSecDer5 = RollingAvgFilter(fTempSecDerHistF5, avgamt);   // Was 4

            UpdateHistory(fSecDer5, fTempSecDerHistF6);
            fSecDer6 = RollingAvgFilter(fTempSecDerHistF6, avgamt);   // Was 4

            UpdateHistory(fSecDer6, fTempSecDerHistF7);
            fSecDer7 = RollingAvgFilter(fTempSecDerHistF7, avgamt);   // Was 4

            UpdateHistory(fSecDer7, fTempSecDerHistF8);
            fSecDer8 = RollingAvgFilter(fTempSecDerHistF8, avgamt);   // Was 4
#endif
#if (false)

            // Test picking gain
            if (fDer > 0.0F) {
                if (fSecDer > 0.0F)
                    fGain = 0.03F;
                else if (fSecDer < 0.0F)
                    fGain = 0.05F;
                else
                    fGain = 0.0F;
            }

            if (fDer < 0.0F)
            {
                if (fSecDer > 0.0F)
                    fGain = 0.09F;
                else if (fSecDer < 0.0F)
                    fGain = 0.09F;
                else
                    fGain = 0.0F;
            }
#endif
#if (false)
            // STEP RESPONSE CODE
            // Find the first sample past N that has increased by X
            if (nFrame == nStartSample)
            {
                // Temps
                for (int c = 0; c < 8; c++)
                {
                    fBaseTemp[c] = fAllTemps[c];
                    Console.WriteLine("Base temp " + c.ToString() + " " + fBaseTemp[c].ToString());
                }

                // Counts
                for (int c = 0; c < 64; c++)
                {
                    nBaseCounts[c] = Convert.ToInt32(token[c + 12 + 64 + 64]);
                    Console.WriteLine("Base counts " + c.ToString() + " " + nBaseCounts[c].ToString());
                }
            }


            if (nFrame > nStartSample)
            {
                for (int c = 0; c < 8; c++)
                { 
                    if (nGotTemp[c] != 1)
                    {
                        if ((fAllTemps[c] - fBaseTemp[c]) > fTempInc)
                        {
                            //Save frame number
                            nFrameAtStepT[c] = nFrame;
                            nGotTemp[c] = 1;
                            Console.WriteLine("Temp step," + c.ToString() + ", " + nFrameAtStepT[c].ToString() + "," + (fAllTemps[c] - fBaseTemp[c]).ToString());
                        }
                    }
                }
            }



            if (nFrame > nStartSample)
            {
                for (int c = 0; c < 64; c++)
                {
                    if (nGotCounts[c] != 1)
                    {
                        if ((Convert.ToInt32(token[c + 12 + 64 + 64])- nBaseCounts[c]) < nCountsInc)
                        { 
                            //Save frame number
                            nFrameAtStepC[c] = nFrame;
                            nGotCounts[c] = 1;
                            Console.WriteLine("Counts step," + c.ToString() + "," + nFrameAtStepC[c].ToString() + "," + (Convert.ToInt32(token[c + 12 + 64 + 64]) - nBaseCounts[c]).ToString());
                        }
                    }
                }
            }
#endif
#if (false)
            if (nFrame > 500)
            {
                if (fDer8 > fMaxDer) fMaxDer = fDer8;
                if (fDer8 < fMinDer) fMinDer = fDer8;
            }
#endif
#if (false)
            if (nFrame < 277)
            {
                fGain = 0.15F;
            } else if (nFrame < 2329)
            {
                fGain = 0.11F;
            } else if (nFrame< 2407)
            {
                fGain = 0.05F;
            }
            else if (nFrame < 2564)
            {
                fGain = 0.04F;
            }
            else if (nFrame < 2662)
            {
                fGain = 0.01F;
            }
            else if (nFrame< 4150)
            {
                fGain = -0.2F;
            } else if (nFrame< 4475)
            {
                fGain = 0.15F;
            } else
            {
                fGain = 0.11F;
            }
#endif
#if (false)
            if (fDer > 0.01F)
            {
                fGain = 0.05F;  // Pos der

            } else if (fDer < -0.01F)
            {
                fGain = 0.13F;  // Neg der

            } else
            {
                fGain = 0.0F;
            }
#endif
#if (false)

            //            SaveOldDer(fDer);
            //            fSecDer = SecDerivative(fDer);
            //            fSecDer = SecDerAvgFilter(fSecDerUn);

            // Create steady state temperature for these counts
            cvtCreateCpp(fWorkingPress, nWorkingChan);
            float fTss = cvtCntsToTemp(nWorkingChan, lCnts);

            //            fAdj = (fFilteredTemp - fTss) * fDer;

            // Work with unadjusted temp
            //            cvtCreateCtp(fFilteredTemp, nWorkingChan);
            //            fUnPress = cvtSingleRawPktToEu(nWorkingChan, lCnts);



            // Work with adjusted temp
            //            cvtCreateCtp(fFilteredTemp + fAdj, nWorkingChan);
            //            cvtCreateCtp(fFilteredTemp + fAdj, nWorkingChan);
            //            fCorrPress = cvtSingleRawPktToEu(nWorkingChan, lCnts);

            // Place in sample array for chart
            //            nSampleFrame[nFrame] = nFrame;
            //            fSamplePress[nFrame] = fUnPress;
#endif
#if (false)
            string sOut = nFrame.ToString() + "," +
                fFilteredTemp.ToString() + "," +
                fDer.ToString() + "," +
                fAdj.ToString() + "," +
                fTss.ToString() + "," +
                lCnts.ToString() + "," +
                fPress.ToString() + "," +
                fUnPress.ToString() + "," +
                fCorrPress.ToString();
#endif

            string sOut1 =
                nFrame.ToString() + "," +
                fTemp.ToString() + "," +
                fFilteredTemp8.ToString() + "," +
                fDer8.ToString() + ",";

            string sOut2;
            float fChanTemp;
            float fCorTemp;
            for (int c = 0; c < 64; c++)
            {
                fPress = Convert.ToSingle(token[c + 12]);
                lCnts = Convert.ToInt32(token[c + 12 + 64 + 64]);

                // Create corrected pressure
                fChanTemp = cvtGetChanTemp(fAllTemps, c);
                //                fCorTemp = fChanTemp - (fDer8 * cvtGetChanGain(c));   // For run

                fCorTemp = fChanTemp - (fDer8 * fStoredGains[nStoredGainNdx]);     // For finding gain

                cvtCreateCtp(fCorTemp, c);
                fCorrPress = cvtSingleRawPktToEu(c, lCnts);

                fACorPress[c, nACorPressNdx] = fCorrPress;

                // Correct pressure from temperature derivative
                //                fCorrPress = fPress - (fDer8 * cvtGetChanGain(c));

#if (false)
                // For pressure = 15
                if (fDer8 > 0.0F)
                    fAdj = fChanPosGain[c] / 1.4F * fDer8;
                else
                    fAdj = fChanNegGain[c] / 2.0F * fDer8;
#endif
#if (false)
                fAdj = 0.03F * fDer;

                if (fAdj > 0.003F) fAdj = 0.003F;
                if (fAdj < -0.003F) fAdj = -0.003F;


                fCorrPress = fPress - fAdj;

                float fCor = fPress - fWorkingPress;
                if (nFrame > 500)
                {
                    if (fCor > fMaxCor[c]) fMaxCor[c] = fCor;
                    if (fCor < fMinCor[c]) fMinCor[c] = fCor;
                }
#endif

                sOut2 = fPress.ToString() + "," +
                    fCorrPress.ToString() + ",";

                sOut1 += sOut2;
            }
            nACorPressNdx++;

            if (bOutFileOpen) {
                if (nFrame > 500)
                {
                    stOutFile.WriteLine(sOut1);
                }
            }
        }

        float fInc = 0.1F;
        float fStart = 2.0F;
        float fEnd = 4.0F;
//        int nLoops;
        const int NUM_GAINS = 40;
        float[] fStoredGains = new float[NUM_GAINS];
        int nStoredGainNdx = 0;
        float[,] fMaxP = new float[64,NUM_GAINS];
        float[,] fMinP = new float[64,NUM_GAINS];


        private void loadDataFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sDataFileName;
            string sLine;

            // Loads the input data file
            rfd.Filter = "All Files (*.*)|*.*";
            if (rfd.ShowDialog() == DialogResult.OK)
            {
                sDataFileName = rfd.FileName;

                Console.WriteLine("File is " + sDataFileName);

                // Attempt to open upload file
                if (File.Exists(sDataFileName) == true)
                {
                    fMaxDer = 0.0F;
                    fMinDer = 0.0F;

                    // Init min and max

                    for (int i=0; i< NUM_GAINS; i++)
                    {
                        for (int c = 0; c < 64; c++)
                        {
                            fMaxP[c,i] = 0.0F;
                            fMinP[c,i] = 100.0F;
                        }

                        fStoredGains[i] = fStart + (fInc * (float)i);
                    }


                    // Loop through to find gains
                    float fLoops = (fEnd - fStart) / fInc;
                    int nLoops = (int)fLoops;
                    Console.WriteLine("Looping " + nLoops.ToString());
                    for (nStoredGainNdx = 0; nStoredGainNdx < nLoops; nStoredGainNdx++)
                    {

                        // Open selected file
                        stDataFile = new StreamReader(sDataFileName);
                        Console.WriteLine("Opened file " + sDataFileName);
                        nACorPressNdx = 0;

                        // Loop through file
                        while ((sLine = stDataFile.ReadLine()) != null)
                        {
                            // Read a line
                            //                        System.Console.WriteLine(sLine);

                            // Break into tokens delimited by comma
                            string[] sToken = sLine.Split(',');
                            ProcessData(sToken);   // Adjust temp

                        }

                        stDataFile.Close();
                        if (bOutFileOpen) {
                            stOutFile.Close();
                            Console.WriteLine("Closed file " + sDataFileName);
                        }

                        // Find max and min
                        for (int l = 0; l < nACorPressNdx; l++) {

                            for (int c = 0; c < 64; c++)
                            {
                                if (fACorPress[c, l] > fMaxP[c,nStoredGainNdx]) fMaxP[c,nStoredGainNdx] = fACorPress[c, l];
                                if (fACorPress[c, l] < fMinP[c,nStoredGainNdx]) fMinP[c,nStoredGainNdx] = fACorPress[c, l];
                            }
                        }

                        // Write max min 
                        Console.WriteLine("Max Min " +
                            nStoredGainNdx.ToString() + " " +
                            fStoredGains[nStoredGainNdx].ToString() + " " +
                            fMaxP[6, nStoredGainNdx].ToString() + " " +
                            fMinP[6, nStoredGainNdx].ToString());


                    }


                }
            }
        }

        private void WriteMaxMinFile(string sMaxMinFileName)
        {

            Console.WriteLine("File is " + sMaxMinFileName);
            stMaxMinFile = new StreamWriter(sMaxMinFileName);

            for (int c = 0; c < 64; c++)
            {
                string sOut = "Chan," + Convert.ToString(c + 1) + "," +
                    fMaxCor[c].ToString() + "," + 
                    fMinCor[c].ToString() + "," +
                    Convert.ToString(fMaxCor[c] / fMaxDer) + "," +
                    Convert.ToString(fMinCor[c] / fMinDer);

                Console.WriteLine(sOut);
                stMaxMinFile.WriteLine(sOut);
            }

//            string sLine = "MMD,0," + fMaxDer.ToString() + "," + fMinDer.ToString();
//            stMaxMinFile.WriteLine(sLine);

            stMaxMinFile.Close();
            Console.WriteLine("Closed file " + stMaxMinFile);
        }

        bool bOutFileOpen = false;
        private void outFIleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sOutFileName;

            // Loads the input data file
            sfd.Filter = "All Files (*.*)|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                sOutFileName = sfd.FileName;

                // Save selected file
                stOutFile = new StreamWriter(sOutFileName);
                Console.WriteLine("Opened out file " + stOutFile);
                bOutFileOpen = true;

                string sOut1 = "F,T,T,D,";
//                string sOut1 = "F,T,";

                for (int c = 0; c < 64; c++)
                {
                    sOut1 += "Cp" + Convert.ToString(c + 1) + ",";
//                    sOut1 += "C" + Convert.ToString(c + 1) + ","; 
                    sOut1 += "Ccp" + Convert.ToString(c + 1) + ","; 
                }

                stOutFile.WriteLine(sOut1);

            }
        }

        private void DisplayChart(int samples, float[] fPressUncor, float[] fPressCor)
        {




        }


        private void displayChartToolStripMenuItem_Click(object sender, EventArgs e)
        {


            chart1.Titles.Add("Pressure");

            chart1.Series[0].Label = "#VALY{#.###}";

            //            string[] xValues = { "First", "Second", "Third" };
//            int[] xValues = nSampleFrame;
//            float[] yValues = { 6.12F, 7.23F, 5.45F };

//            chart1.Series[0].Points.DataBindXY(xValues, yValues);
//            chart1.Series[0].Points.DataBindXY(nSampleFrame, fSamplePress);
//            chart1.Series[0].IsValueShownAsLabel = false;
        }


        // Processes the calibration file 
        private void ProcessCal(int lineno, string[] token)
        {
            int nChan;
            int ndx;

            if (lineno <= 8)
            {
                for (int i=0; i<64; i++) {
                    nMLen[i] = 0;
                }
                return;    // Ignore the first 8 lines
            }

            // Extract cal data
            nChan = Convert.ToInt16(token[3]);  // Get channel from cal table
            ndx = nMLen[nChan-1];
            fMTemp[nChan-1, ndx] = Convert.ToSingle(token[2]);       // Get temp from cal table
            fMPress[nChan-1, ndx] = Convert.ToSingle(token[4]);     // Get pressure from cal table
            lMCounts[nChan-1, ndx] = Convert.ToInt32(token[5]);     // Get counts from cal table

            Console.WriteLine("SET PT " +
                fMTemp[nChan - 1, ndx].ToString() + " " +
                nChan.ToString() + " " +
                fMPress[nChan - 1, ndx].ToString() + " " +
                lMCounts[nChan - 1, ndx].ToString() + " " +
                nMLen[nChan - 1].ToString());

            nMLen[nChan - 1]++;
        }


        private void loadCalibrationFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Loads the calibration file in the arrays

            string sCalFileName;
            string sLine;
            int nLineNo;

            // Loads the input data file
            crfd.Filter = "All Files (*.cfg)|*.cfg";
            if (crfd.ShowDialog() == DialogResult.OK)
            {
                sCalFileName = crfd.FileName;

                Console.WriteLine("File is " + sCalFileName);

                // Attempt to open upload file
                if (File.Exists(sCalFileName) == true)
                {
                    // Open selected file
                    stCalFile = new StreamReader(sCalFileName);
                    Console.WriteLine("Opened file " + sCalFileName);

                    // Loop through file
                    nLineNo = 0;
                    while ((sLine = stCalFile.ReadLine()) != null)
                    {
                        // Read a line
//                        System.Console.WriteLine(sLine);

                        // Break into tokens delimited by comma
                        string[] sToken = sLine.Split(' ');
                        ProcessCal(nLineNo, sToken);
                        nLineNo++;
                    }

                    stCalFile.Close();
                    Console.WriteLine("Closed file " + sCalFileName);

                }
            }
        }


        // Channel is zero based
        void cvtCreateCtp(float flTemp, int nChan)
        {
            int p, n;
            int bFound;
            int bOneTp;
            float fMDeltaTemp;
            float fRatioP;
            float fRatioC;
            float fCurPres;
            long lCurCnts;
            float x0, x1, xval;
            float y0, y1;   // Interp values
            int t;

            bFound = 0;
            bOneTp = 0;

            //            pCtdsStaging->fTemp[nChan] = flTemp;
            if (nNumCalT == 1)
            {
                // One temperature plane
                t = 0;
                for (p = 0; p < nNumCalP; p++)
                {
                    fCPress[nChan,p] = fMPress[nChan,p];      // Pressures
                    lCCounts[nChan,p] = lMCounts[nChan,p];    // Counts
                    fCCounts[nChan,p] = (float)lCCounts[nChan,p];     // Counts table in float form
                }

                nCNumPress[nChan] = nNumCalP;
            }
            else
            {
                // Multiple temperature planes

                // Work one temperature grouping at a time
                for (t = 0, n = 0; t < nNumCalT; n += nNumCalP, t++)
                {
                    // If current temp is below the first, use the first
                    // If the table is correct all pressure will have a current temperature below the first in table

                    if (flTemp <= fMTemp[nChan,0])
                    {
                        // Current temperature is lower than the lowest temp plane
                        // Fill the current temp plane directly
                        for (p = 0; p < nNumCalP; p++, n++)
                        {
                            fCPress[nChan,p] = fMPress[nChan,n];      // Pressures
                            lCCounts[nChan,p] = lMCounts[nChan,n];    // Counts
                            fCCounts[nChan,p] = (float)lCCounts[nChan,p];     // Counts table in float form
                        }
                        bOneTp = 1;
                        bFound = 1;
                        nCNumPress[nChan] = nNumCalP;

                        break;
                    }
                    else
                    {
                        // Find first temp point just above current temp
                        if (fMTemp[nChan,n] >= flTemp)
                        {
                            // Copy over the high point, that is this point
                            for (p = 0; p < nNumCalP; p++, n++)
                            {
                                fHCTemp[p] = fMTemp[nChan,n];
                                fHCPress[p] = fMPress[nChan,n];
                                lHCCounts[p] = lMCounts[nChan,n];

                                // Copy over the low plane, that is this point - nNumCalP
                                fLCTemp[p] = fMTemp[nChan,(n - nNumCalP)];
                                fLCPress[p] = fMPress[nChan,(n - nNumCalP)];
                                lLCCounts[p] = lMCounts[nChan,(n - nNumCalP)];
                            }

                            bFound = 1;
                            nCNumPress[nChan] = nNumCalP;
                            break;
                        } // End temp in table just above current temp
                    } // End current temp not below first entry
                } // End loop each pressure

                if (bFound == 0)
                {
                    // If not found current temperature is higher than the higest temp
                    // Fill the current temp plane directly
                    t--;    // T is high by 1 if not found

                    n = n-nNumCalP;   // Table index is high by number of pressure if not found
                    for (p = 0; p < nNumCalP; p++, n++)
                    {
                        fCPress[nChan,p] = fMPress[nChan,n];
                        lCCounts[nChan,p] = lMCounts[nChan,n];
                        fCCounts[nChan,p] = (float)lCCounts[nChan,p];     // Counts table in float form
                    }

                    bOneTp = 1;
                    nCNumPress[nChan] = nNumCalP;
                } // End not found, use last

                // Interpolate current temperature plane when more than one temp plane
                if (bOneTp == 0)
                {
                    // Using linear interpolation
                    for (p = 0; p < nNumCalP; p++)
                    {
                        // Calculate delta temp x1 - x0, used by press and counts
                        x0 = fLCTemp[p];
                        x1 = fHCTemp[p];
                        fMDeltaTemp = x1 - x0;
                        xval = flTemp;

                        // Calculate current pressure
                        // Calculate delta press (y) / delta temp (x)
                        y0 = fLCPress[p];
                        y1 = fHCPress[p];

                        fRatioP = (y1 - y0) / fMDeltaTemp;
                        fCurPres = y0 + ((xval - x0) * fRatioP);

                        // Calculate current counts
                        // Calculate delta counts (y) / delta temp (x)
                        y0 = (float)lLCCounts[p];
                        y1 = (float)lHCCounts[p];

                        fRatioC = (y1 - y0) / fMDeltaTemp;
                        lCurCnts = (long)(y0 + ((xval - x0) * fRatioC));

                        // Put into current temp plane
                        fCPress[nChan,p] = fCurPres;   // Pressure table
                        lCCounts[nChan,p] = lCurCnts;  // Counts table
                        fCCounts[nChan,p] = (float)lCurCnts;   // Counts table in float form
                    } // End loop through all num points
                } // End not one temp plane
            } // End multiple temp points

            // Calculate M and B terms for each pressure segment. Works for one or many temp planes
            for (p = 0; p < nNumCalP - 1; p++)
            {
                // Calculate M and B terms for pressure conversion
                fMctp[nChan,p] = (fCPress[nChan,p] - fCPress[nChan,p + 1]) / (fCCounts[nChan,p] - fCCounts[nChan,p + 1]);
                fBctp[nChan,p] = fCPress[nChan,p + 1] - (fMctp[nChan,p] * fCCounts[nChan,p + 1]);
            }

            // Fill in the top entry, use the one just before
            if (nNumCalP > 1)
            {
                fMctp[nChan,nNumCalP - 1] = fMctp[nChan,nNumCalP - 2];
                fBctp[nChan,nNumCalP - 1] = fBctp[nChan,nNumCalP - 2];
            }

            // Calculate the slope and offset for the index conversion
            fMNdxCtp[nChan] = (0.0F - (nNumCalP - 1)) / (fCCounts[nChan,0] - fCCounts[nChan,nNumCalP - 1]);
            fBNdxCtp[nChan] = (nNumCalP - 1) - (fMNdxCtp[nChan] * fCCounts[nChan,nNumCalP - 1]);

        }



        // Channel is zero based
        void cvtCreateCpp(float fPress, int nChan)
        {
            int p, n;
            int bFound;
            int bOneTp;
            float fMDeltaPress;
            float fRatioP;
            float fRatioC;
            float fCurTemp;
            long lCurCnts;
            float x0, x1, xval;
            float y0, y1;   // Interp values
            int t;

            bFound = 0;
            bOneTp = 0;

            Console.WriteLine("Called CPP " + nChan.ToString());

            // Work one temp grouping at a time
            for (p = 0, n = 0; p < nNumCalP; p++)
            {

//                Console.WriteLine("Temp " + t.ToString());

                // If current temp is below the first, use the first
                // If the table is correct all pressure will have a current temperature below the first in table
                if (fPress <= fMPress[nChan, 0])
                {
                    // Current pressure is lower than the lowest pressure plane
                    // Fill the current press plane directly
                    for (n=p, t = 0; t < nNumCalT; t++, n += nNumCalP)
                    {
                        fPPress[nChan, t] = fMPress[nChan, n];      // Pressures
                        lPCounts[nChan, t] = lMCounts[nChan, n];    // Counts
                        fPCounts[nChan, t] = (float)lMCounts[nChan, t];     // Counts table in float form
                        fPTemps[nChan, t] = fMTemp[nChan, n];      // Temps
                    }
                    bOneTp = 1;
                    bFound = 1;
//                    nPNumTemps[nChan] = nNumCalP;

                    break;
                }
                else
                {
//                    Console.WriteLine("Looking " + fPress.ToString() + " " + fMPress[nChan, p].ToString());

                    // Find first press point just above current press
                    if (fMPress[nChan, p] >= fPress)
                    {
//                        Console.WriteLine("Found " + fPress.ToString() + " " + fMPress[nChan, p].ToString());

                        // Copy over the high point, that is this point
                        for (n=p, t = 0; t < nNumCalT; t++, n += nNumCalP)
                        {
//                            Console.WriteLine("Copying " + n.ToString() + " " + t.ToString() + " " + p.ToString());

                            fHPTemp[t] = fMTemp[nChan, n];
                            fHPPress[t] = fMPress[nChan, n];
                            lHPCounts[t] = lMCounts[nChan, n];

                            // Copy over the low plane, that is this point - nNumCalP
                            fLPTemp[t] = fMTemp[nChan, (n - 1)];
                            fLPPress[t] = fMPress[nChan, (n - 1)];
                            lLPCounts[t] = lMCounts[nChan, (n - 1)];

                            Console.WriteLine("Table " +
                                t.ToString() + " " +
                               fHPTemp[t].ToString() + " " +
                               fHPPress[t].ToString() + " " +
                               lHPCounts[t].ToString() + " " +
                               fLPTemp[t].ToString() + " " +
                               fLPPress[t].ToString() + " " +
                               lLPCounts[t].ToString());
                               
                        }

                        bFound = 1;
//                        nPNumTemps[nChan] = nNumCalT;
                        break;
                    } // End temp in table just above current temp
                } // End current temp not below first entry
            } // End loop each pressure

            if (bFound == 0)
            {
                // If not found current temperature is higher than the higest temp
                // Fill the current temp plane directly
                p--;    // T is high by 1 if not found

                n = n - nNumCalP;   // Table index is high by number of pressure if not found
                for (t = 0; t < nNumCalT; t++, n += nNumCalP)
                {
                    fPPress[nChan, t] = fMPress[nChan, n];
                    lPCounts[nChan, t] = lMCounts[nChan, n];
                    fPCounts[nChan, t] = (float)lCCounts[nChan, t];     // Counts table in float form
                    fPTemps[nChan, t] = fMTemp[nChan, n];      // Pressures
                }

                bOneTp = 1;
//                nPNumTemps[nChan] = nNumCalT;
            } // End not found, use last

            // Interpolate current temperature plane when more than one temp plane
            if (bOneTp == 0)
            {
                // Using linear interpolation
                for (t = 0; t < nNumCalT; t++)
                {
                    // Calculate delta press x1 - x0, used by temp and counts
                    x0 = fLPPress[t];
                    x1 = fHPPress[t];
                    fMDeltaPress = x1 - x0;
                    xval = fPress;

                    // Calculate current pressure
                    // Calculate delta press (y) / delta temp (x)
                    y0 = fLPTemp[t];
                    y1 = fHPTemp[t];

                    fRatioP = (y1 - y0) / fMDeltaPress;
                    fCurTemp = y0 + ((xval - x0) * fRatioP);

                    // Calculate current counts
                    // Calculate delta counts (y) / delta temp (x)
                    y0 = (float)lLPCounts[t];
                    y1 = (float)lHPCounts[t];

                    fRatioC = (y1 - y0) / fMDeltaPress;
                    lCurCnts = (long)(y0 + ((xval - x0) * fRatioC));

                    // Put into current pressure plane
                    fPTemps[nChan, t] = fCurTemp;   // Pressure table
//                    fPPress[nChan, p] = fCurPres;   // Pressure table

                    lPCounts[nChan, t] = lCurCnts;  // Counts table
                    fPCounts[nChan, t] = (float)lCurCnts;   // Counts table in float form

                    Console.WriteLine("Temps and counts " +
        t.ToString() + " " +
        fPTemps[nChan, t].ToString() + " " +
        lPCounts[nChan, t].ToString());

                } // End loop through all num points
            } // End not one temp plane


            // Calculate M and B terms for each pressure segment. Works for one or many temp planes
            for (t = 0; t < nNumCalT - 1; t++)
            {
                // Calculate M and B terms for counts to temperature conversion
                fMcpp[nChan, t] = (fPTemps[nChan, t] - fPTemps[nChan, t + 1]) / (fPCounts[nChan, t] - fPCounts[nChan, t + 1]);
                fBcpp[nChan, t] = fPTemps[nChan, t + 1] - (fMcpp[nChan, t] * fPCounts[nChan, t + 1]);

                Console.WriteLine("M and Bs " +
    t.ToString() + " " +
    fMcpp[nChan,t].ToString() + " " +
    fBcpp[nChan,t].ToString() + " " +
    fPTemps[nChan,t].ToString());

            }

            // Fill in the top entry, use the one just before
            fMcpp[nChan, nNumCalT - 1] = fMcpp[nChan, nNumCalT - 2];
            fBcpp[nChan, nNumCalT - 1] = fBcpp[nChan, nNumCalT - 2];

            // Calculate the slope and offset for the index conversion
            fMNdxCpp[nChan] = (0.0F - (nNumCalT - 1)) / (fPCounts[nChan, 0] - fPCounts[nChan, nNumCalT - 1]);
            fBNdxCpp[nChan] = (nNumCalT - 1) - (fMNdxCpp[nChan] * fPCounts[nChan, nNumCalT - 1]);

            Console.WriteLine("Mn and Bn " +
t.ToString() + " " +
fMNdxCpp[nChan].ToString() + " " +
fBNdxCpp[nChan].ToString());



        }


        private float cvtCntsToTemp(int c, long lCounts)
        {
            float fTemp;
            double fCounts = (double)lCounts / 20000.0;


            // y = 7E-10x2 - 0.0001x - 11.356
            // y = -3E-15x3 - 6E-10x2 - 0.0003x - 21.852

            fTemp = (float)((.3891 * fCounts * fCounts) - (20.778*fCounts) + 263.95);
//            fTemp = (-3e-15F * fCounts * fCounts * fCounts) - (6.0e-10F * fCounts * fCounts) - (.0003F * fCounts) - 21.852F;

            Console.WriteLine("Cnts to temp" + fTemp.ToString() + " " +fCounts.ToString());

#if (false)
            // Find table index
            //            n = (int)(fMNdxCpp[c] * lCounts + fBNdxCpp[c]);
            //            if (n < 0)
            //                n = 0;
            //            else if (n >= nNumCalT)
            //                n = (nNumCalT - 1);

            // Calculate pressure from counts
            //            fTemp = (fMcpp[c, n] * lCounts) + fBcpp[c, n];

            Console.WriteLine("Cnts to temp" +
n.ToString() + " " +
lCounts.ToString() + " " +
fMcpp[c,n].ToString() + " " +
fBcpp[c,n].ToString() + " " +
fTemp.ToString());
#endif




            //	sprintf(szOneLine,"cvtSingleRawPktToEu %d %d %ld %f %f %f %f %f",c,n,lCounts,
            //			pCtdsActive->fMctp[c][n],
            //			pCtdsActive->fBctp[c][n],
            //			pCtdsActive->fMNdxCtp[c],
            //			pCtdsActive->fBNdxCtp[c],
            //			fPress);
            //	strcpy(szOneLineCr,szOneLine);
            //	strcat(szOneLineCr,"\r\n");

            return (fTemp);
        }



        private float cvtSingleRawPktToEu(int c, long lCounts)
        {
            int n;
            float fPress;

            // Find table index
            n = (int)(fMNdxCtp[c] * lCounts + fBNdxCtp[c]);
            if (n < 0)
                n = 0;
            else if (n >= nCNumPress[c])
                n = (nCNumPress[c] - 1);

            // Calculate pressure from counts
            fPress = (fMctp[c,n] * lCounts) + fBctp[c,n];

            //	sprintf(szOneLine,"cvtSingleRawPktToEu %d %d %ld %f %f %f %f %f",c,n,lCounts,
            //			pCtdsActive->fMctp[c][n],
            //			pCtdsActive->fBctp[c][n],
            //			pCtdsActive->fMNdxCtp[c],
            //			pCtdsActive->fBNdxCtp[c],
            //			fPress);
            //	strcpy(szOneLineCr,szOneLine);
            //	strcat(szOneLineCr,"\r\n");

            return (fPress);
        }


        // Returns gain temperature for specific channels
            private float cvtGetChanGain(int c)
        {

            float fGain = 3.0F;

            switch (c + 1)
            {

                case 1:
                    fGain = 2.0F;
                    break;
                case 2:
                    fGain = 1.0F;
                    break;
                case 3:
                    fGain = 1.0F;
                    break;
                case 4:
                    fGain = 1.5F;
                    break;
                case 5:
                    fGain = 3.0F;
                    break;
                case 6:
                    fGain = 2.8F;
                    break;
                case 7:
                    fGain = 3.0F;
                    break;
                case 8:
                    fGain = 2.0F;
                    break;
                case 9:
                    fGain = 3.2F;
                    break;
                case 10:
                    fGain = 2.9F;
                    break;
                case 11:
                    fGain = 5.0F;
                    break;
                case 12:
                    fGain = 5.0F;
                    break;
                case 13:
                    fGain = 5.0F;
                    break;
                case 14:
                    fGain = 5.0F;
                    break;
                case 15:
                    fGain = 5.0F;
                    break;
                case 16:
                    fGain = 3.5F;
                    break;
                case 17:
                    fGain = 0.10F;
                    break;
                case 25:
                    fGain = 0.16F;
                    break;
                case 62:
                    fGain = 0.09F;
                    break;

                default:
                    fGain = 3.0F;
                    break;
            }

#if (false)
            float fGain = 0.15F; ;

            switch (c+1) {

                case 1:
                    fGain = 0.11F;
                    break;
                case 2:
                    fGain = 0.02F;
                    break;
                case 3:
                    fGain = 0.04F;
                    break;
                case 4:
                    fGain = 0.02F;
                    break;
                case 5:
                    fGain = 0.12F;
                    break;
                case 6:
                    fGain = 0.10F;
                    break;
                case 7:
                    fGain = 0.10F;
                    break;
                case 8:
                    fGain = 0.08F;
                    break;
                case 9:
                    fGain = 0.16F;
                    break;
                case 10:
                    fGain = 0.16F;
                    break;
                case 13:
                    fGain = 0.18F;
                    break;
                case 17:
                    fGain = 0.10F;
                    break;
                case 25:
                    fGain = 0.16F;
                    break;
                case 62:
                    fGain = 0.09F;
                    break;

                default:
                    fGain = 0.09F;
                    break;
            }
#endif

                    return (fGain);
        }




        private void channelToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void saveMaxMinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string sOutFileName;

            // Loads the input data file
            mmfd.Filter = "All Files (*.*)|*.*";
            if (mmfd.ShowDialog() == DialogResult.OK)
            {
                sOutFileName = mmfd.FileName;

                // Save selected file
                WriteMaxMinFile(sOutFileName);
            }
        }


        private float cvtGetChanTemp(float[] fpTemp, int nChan)
        {
            float fChanTemp = 0.0F;

            if ((nChan >= 0) && (nChan <= 3))
            {
                if (fpTemp[0] > 100.0F)
                {
                    fChanTemp = fpTemp[1];
                }
                else if (fpTemp[1] > 100.0F)
                {
                    fChanTemp = fpTemp[0];
                }
                else
                {
                    fChanTemp = fpTemp[0];
                }
            }
            else if ((nChan >= 4) && (nChan <= 11))
            {
                if (fpTemp[0] > 100.0F)
                {
                    fChanTemp = fpTemp[1];
                }
                else if (fpTemp[1] > 100.0F)
                {
                    fChanTemp = fpTemp[0];
                }
                else
                {
                    fChanTemp = (fpTemp[0] + fpTemp[1]) / 2.0F;
                }
            }
            else if ((nChan >= 12) && (nChan <= 15))
            {
                if (fpTemp[0] > 100.0F)
                {
                    fChanTemp = fpTemp[1];
                }
                else if (fpTemp[1] > 100.0F)
                {
                    fChanTemp = fpTemp[0];
                }
                else
                {
                    fChanTemp = fpTemp[1];
                }

            }
            else if ((nChan >= 16) && (nChan <= 19))
            {
                if (fpTemp[2] > 100.0F)
                {
                    fChanTemp = fpTemp[3];
                }
                else if (fpTemp[3] > 100.0F)
                {
                    fChanTemp = fpTemp[2];
                }
                else
                {
                    fChanTemp = fpTemp[2];
                }
            }
            else if ((nChan >= 20) && (nChan <= 27))
            {
                if (fpTemp[2] > 100.0F)
                {
                    fChanTemp = fpTemp[3];
                }
                else if (fpTemp[3] > 100.0F)
                {
                    fChanTemp = fpTemp[2];
                }
                else
                {
                    fChanTemp = (fpTemp[2] + fpTemp[3]) / 2.0F;
                }
            }
            else if ((nChan >= 28) && (nChan <= 31))
            {
                if (fpTemp[2] > 100.0F)
                {
                    fChanTemp = fpTemp[3];
                }
                else if (fpTemp[3] > 100.0F)
                {
                    fChanTemp = fpTemp[2];
                }
                else
                {
                    fChanTemp = fpTemp[3];
                }

            }
            else if ((nChan >= 32) && (nChan <= 35))
            {
                if (fpTemp[4] > 100.0F)
                {
                    fChanTemp = fpTemp[5];
                }
                else if (fpTemp[5] > 100.0F)
                {
                    fChanTemp = fpTemp[4];
                }
                else
                {
                    fChanTemp = fpTemp[5];  // Was 4
                }
            }
            else if ((nChan >= 36) && (nChan <= 43))
            {
                if (fpTemp[4] > 100.0F)
                {
                    fChanTemp = fpTemp[5];
                }
                else if (fpTemp[5] > 100.0F)
                {
                    fChanTemp = fpTemp[4];
                }
                else
                {
                    fChanTemp = (fpTemp[4] + fpTemp[5]) / 2.0F;
                }
            }
            else if ((nChan >= 44) && (nChan <= 47))
            {
                if (fpTemp[4] > 100.0F)
                {
                    fChanTemp = fpTemp[5];
                }
                else if (fpTemp[5] > 100.0F)
                {
                    fChanTemp = fpTemp[4];
                }
                else
                {
                    fChanTemp = fpTemp[4];  // Was 5
                }

            }
            else if ((nChan >= 48) && (nChan <= 51))
            {
                if (fpTemp[6] > 100.0F)
                {
                    fChanTemp = fpTemp[7];
                }
                else if (fpTemp[7] > 100.0F)
                {
                    fChanTemp = fpTemp[6];
                }
                else
                {
                    fChanTemp = fpTemp[7];  // Was 6
                }
            }
            else if ((nChan >= 52) && (nChan <= 59))
            {
                if (fpTemp[6] > 100.0F)
                {
                    fChanTemp = fpTemp[7];
                }
                else if (fpTemp[7] > 100.0F)
                {
                    fChanTemp = fpTemp[6];
                }
                else
                {
                    fChanTemp = (fpTemp[6] + fpTemp[7]) / 2.0F;
                }
            }
            else if ((nChan >= 60) && (nChan <= 63))
            {
                if (fpTemp[6] > 100.0F)
                {
                    fChanTemp = fpTemp[7];
                }
                else if (fpTemp[7] > 100.0F)
                {
                    fChanTemp = fpTemp[6];
                }
                else
                {
                    fChanTemp = fpTemp[6];  // Was 7
                }
            }

            return (fChanTemp);
        }




        float[] fChanPosGain = new float[64];
        float[] fChanNegGain = new float[64];

        private void loadMaxMinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Loads the calibration file in the arrays

            string sCalFileName;
            string sLine;
            int nChanNo;

            // Loads the input data file
            crfd.Filter = "All Files (*.csv)|*.csv";
            if (crfd.ShowDialog() == DialogResult.OK)
            {
                sCalFileName = crfd.FileName;

                Console.WriteLine("File is " + sCalFileName);

                // Attempt to open upload file
                if (File.Exists(sCalFileName) == true)
                {
                    // Open selected file
                    stCalFile = new StreamReader(sCalFileName);
                    Console.WriteLine("Opened file " + sCalFileName);

                    // Loop through file
                    nChanNo = 0;
                    while ((sLine = stCalFile.ReadLine()) != null)
                    {
 //                       Console.WriteLine("Line " + sLine);

                        // Break into tokens delimited by comma
                        string[] sToken = sLine.Split(',');


//                        Console.WriteLine("Line " + sToken[4]);
//                        Console.WriteLine("Line " + sToken[5]);

                        // Extract gain
                        nChanNo = Convert.ToInt16(sToken[1]) - 1;
                        fChanPosGain[nChanNo] = Convert.ToSingle(sToken[4]);
                        fChanNegGain[nChanNo] = Convert.ToSingle(sToken[5]);

                        Console.WriteLine("Gain " +
                            nChanNo.ToString() + " " +
                            fChanPosGain[nChanNo].ToString() + " " +
                            fChanNegGain[nChanNo].ToString());
                    }

                    stCalFile.Close();
                    Console.WriteLine("Closed file " + sCalFileName);

                }
            }

        }
    }
}
