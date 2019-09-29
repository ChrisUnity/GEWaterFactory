// ========================================================
// 描 述：
// 作 者：张天驰 
// 创建时间：2019/05/09 14:05:46
// 版 本：v 1.0  @ HoloView  @ ShowNow2.0
// 修改人：
// 修改时间：
// 描述：
// ========================================================
using UnityEngine;
using System.Collections;
using System.IO;

public class PCMConvert
{

    // convert two bytes to one float in the range -1 to 1
    static float bytesToFloat(byte firstByte, byte secondByte)
    {
        // convert two bytes to one short (little endian)
        short s = (short)((secondByte << 8) | firstByte);
        // convert to range from -1 to (just below) 1
        return s / 32768.0F;
    }

    private static byte[] GetBytes(string filename)
    {
        return File.ReadAllBytes(filename);
    }

    public float[] LeftChannel { get; internal set; }
    public float[] RightChannel { get; internal set; }


    public PCMConvert(byte[] pcmdata, int bits_per_sample, int sample_rate, int number_of_channels, int number_of_frames)
    {
        // Allocate memory (right will be null if only mono sound)
        LeftChannel = new float[number_of_frames];
        if (number_of_channels == 2) RightChannel = new float[number_of_frames];
        else RightChannel = null;

        // Write to double array/s:
        int pos = 0;
        int i = 0;
        while (pos < pcmdata.Length)
        {
            LeftChannel[i] = bytesToFloat(pcmdata[pos], pcmdata[pos + 1]);
            pos += 2;
            if (number_of_channels == 2)
            {
                RightChannel[i] = bytesToFloat(pcmdata[pos], pcmdata[pos + 1]);
                pos += 2;
            }
            i++;
        }
    }
   
}

