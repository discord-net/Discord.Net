using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Discord.Audio.Opus
{
    internal enum OpusCtl : int
    {
        SetBitrateRequest = 4002,
        GetBitrateRequest = 4003,
        SetInbandFECRequest = 4012,
        GetInbandFECRequest = 4013
    }

    /// <summary>Supported coding modes.</summary>
    internal enum OpusApplication : int
    {
        /// <summary>
        /// Gives best quality at a given bitrate for voice signals. It enhances the input signal by high-pass filtering and emphasizing formants and harmonics. 
        /// Optionally it includes in-band forward error correction to protect against packet loss. Use this mode for typical VoIP applications. 
        /// Because of the enhancement, even at high bitrates the output may sound different from the input.
        /// </summary>
        Voip = 2048,
        /// <summary>
        /// Gives best quality at a given bitrate for most non-voice signals like music. 
        /// Use this mode for music and mixed (music/voice) content, broadcast, and applications requiring less than 15 ms of coding delay.
        /// </summary>
        Audio = 2049,
        /// <summary> Low-delay mode that disables the speech-optimized mode in exchange for slightly reduced delay.  </summary>
        Restricted_LowLatency = 2051
    }

    internal enum OpusError : int
    {
        /// <summary>  No error. </summary>
        OK = 0,
        /// <summary> One or more invalid/out of range arguments. </summary>
        BadArg = -1,
        /// <summary> The mode struct passed is invalid. </summary>
        BufferToSmall = -2,
        /// <summary> An internal error was detected. </summary>
        InternalError = -3,
        /// <summary> The compressed data passed is corrupted. </summary>
        InvalidPacket = -4,
        /// <summary> Invalid/unsupported request number. </summary>
        Unimplemented = -5,
        /// <summary> An encoder or decoder structure is invalid or already freed.  </summary>
        InvalidState = -6,
        /// <summary> Memory allocation has failed. </summary>
        AllocFail = -7
    }

}
