namespace Discord.Audio
{
    //https://github.com/gcp/opus/blob/master/include/opus_defines.h
    internal enum OpusCtl : int
    {
        SetBitrate = 4002,
        SetBandwidth = 4008,
        SetInbandFEC = 4012,
        SetPacketLossPercent = 4014,
        SetSignal = 4024
    }
}
