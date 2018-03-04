using UnityEngine;
using UnityEngine.UI;
using System.Text;
using POpusCodec;
using System.Collections.Generic;
using System;

[RequireComponent(typeof(AudioSource))]
public class OpusNetworked : MonoBehaviour
{
    // device id of the microphone used to record sound
    public int micDeviceId = 0;

    // the audio source for the mic, used for recording sound
    AudioSource audiorecorder;
    // the audio source for playback, must not be the same as the audio recording audiosource
    [Header("add a different audioSource for the playback")]
    public AudioSource audioplayer;

    // buffers to receive and send audio data
    List<float> micBuffer;
    List<float> receiveBuffer;
    // size of a network package of opus, audio needs to be split in equal size packages and sent
    int packageSize;

    // decoder and encoder instances of opus
    OpusEncoder encoder;
    OpusDecoder decoder;

    // other values then stereo will not yet work
    POpusCodec.Enums.Channels opusChannels = POpusCodec.Enums.Channels.Stereo;
    // other values then 48000 do not work at the moment, it requires and additional conversion before sending and at receiving
    // also osx runs at 44100 i think, this causes also some hickups
    POpusCodec.Enums.SamplingRate opusSamplingRate = POpusCodec.Enums.SamplingRate.Sampling48000;

    void Update()
    {
        // 毎フレームデータの送信
        SendData();
    }


    // Use this for initialization
    void Start()
    {
        Debug.Log("start.");
        micBuffer = new List<float>();
        if (true)
        {

            //data = new float[samples];
            encoder = new OpusEncoder(opusSamplingRate, opusChannels);
            encoder.EncoderDelay = POpusCodec.Enums.Delay.Delay20ms;
            Debug.Log("Opustest.Start: framesize: " + encoder.FrameSizePerChannel + " " + encoder.InputChannels);

            // the encoder delay has some influence on the amount of data we need to send, but it's not a multiplication of it
            // 960byte * channel sizeになってる。
            packageSize = encoder.FrameSizePerChannel * (int)opusChannels;
            //dataSendBuffer = new float[packageSize];

            //encoder.ForceChannels = POpusCodec.Enums.ForceChannels.NoForce;
            //encoder.SignalHint = POpusCodec.Enums.SignalHint.Auto;
            //encoder.Bitrate = samplerate;
            //encoder.Complexity = POpusCodec.Enums.Complexity.Complexity0;
            //encoder.DtxEnabled = true;
            //encoder.MaxBandwidth = POpusCodec.Enums.Bandwidth.Fullband;
            //encoder.ExpectedPacketLossPercentage = 0;
            //encoder.UseInbandFEC = true;
            //encoder.UseUnconstrainedVBR = true;

            // setup a microphone audio recording
            Debug.Log("Opustest.Start: setup mic with " + Microphone.devices[micDeviceId] + " " + AudioSettings.outputSampleRate);
            audiorecorder = GetComponent<AudioSource>();
            audiorecorder.loop = true;

            audiorecorder.clip = Microphone.Start(
                Microphone.devices[micDeviceId], true, 1, AudioSettings.outputSampleRate
            );

            audiorecorder.Play();
        }

        // playback stuff
        decoder = new OpusDecoder(opusSamplingRate, opusChannels);

        receiveBuffer = new List<float>();

        // setup a playback audio clip, length is set to 1 sec (should not be used anyways)
        AudioClip myClip = AudioClip.Create(
            "MyPlayback",
            (int)opusSamplingRate,
            (int)opusChannels,
            (int)opusSamplingRate,
            true,
            OnAudioRead, // pcmreader callback.
            OnAudioSetPosition
        );

        // ここでプレイすれば自分に入力したサウンドが聞こえてきたりするんだろうか
        // audioplayer.loop = true;
        // audioplayer.clip = myClip;
        // audioplayer.Play();
    }

    /**
        Unityからのオーディオ入力部
     */
    void OnAudioFilterRead(float[] data, int channels)
    {
        // add mic data to buffer
        micBuffer.AddRange(data);

        // clear array so we dont output any sound
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = 0;
        }
    }

    void SendData()
    {
        // take pieces of buffer and send data
        while (micBuffer.Count > packageSize)
        {
            // ここでエンコードをやってる。
            byte[] encodedData = encoder.Encode(micBuffer.GetRange(0, packageSize).ToArray());
            Debug.Log("enc length:" + encodedData.Length);
            SendEncodedData(encodedData);
            micBuffer.RemoveRange(0, packageSize);
        }
    }

    private void SendEncodedData(byte[] data)
    {
        // encodeしたデータを送信する。
        // 受診側は ReceiveEncodedData でサウンドをdecodeする。
    }

    void ReceiveEncodedData(byte[] encodedData)
    {
        receiveBuffer.AddRange(decoder.DecodePacketFloat(encodedData));
    }

    // this is used by the second audio source, to read data from playData and play it back
    // OnAudioRead requires the AudioSource to be on the same GameObject as this script
    void OnAudioRead(float[] data)
    {
        // 再生箇所っぽい。UNET経由で着火してるんだろうか。
        Debug.Log("Opustest.OnAudioRead: " + data.Length);

        int pullSize = Mathf.Min(data.Length, receiveBuffer.Count);
        float[] dataBuf = receiveBuffer.GetRange(0, pullSize).ToArray();
        dataBuf.CopyTo(data, 0);
        receiveBuffer.RemoveRange(0, pullSize);

        // clear rest of data
        for (int i = pullSize; i < data.Length; i++)
        {
            data[i] = 0;
        }
    }


    void OnAudioSetPosition(int newPosition)
    {
        Debug.Log("いつ呼ばれるんだろうこれ。");
        // we dont need the audio position at the moment
    }
}