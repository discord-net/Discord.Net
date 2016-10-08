using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Model = Discord.API.Rpc.VoiceSettings;

namespace Discord.Rpc
{
    public class VoiceSettings
    {
        public string InputDeviceId { get; private set; }
        public float InputVolume { get; private set; }
        public IReadOnlyCollection<VoiceDevice> AvailableInputDevices { get; private set; }

        public string OutputDeviceId { get; private set; }
        public float OutputVolume { get; private set; }
        public IReadOnlyCollection<VoiceDevice> AvailableOutputDevices { get; private set; }

        public bool AutomaticGainControl { get; private set; }
        public bool EchoCancellation { get; private set; }
        public bool NoiseSuppression { get; private set; }
        public bool QualityOfService { get; private set; }
        public bool SilenceWarning { get; private set; }

        public string ActivationMode { get; private set; }
        public bool AutoThreshold { get; private set; }
        public float Threshold { get; private set; }
        public IReadOnlyCollection<VoiceShortcut> Shortcuts { get; private set; }
        public float Delay { get; private set; }

        internal VoiceSettings() { }
        internal static VoiceSettings Create(Model model)
        {
            var entity = new VoiceSettings();
            entity.Update(model);
            return entity;
        }
        internal void Update(Model model)
        {
            AutomaticGainControl = model.AutomaticGainControl;
            EchoCancellation = model.EchoCancellation;
            NoiseSuppression = model.NoiseSuppression;
            QualityOfService = model.QualityOfService;
            SilenceWarning = model.SilenceWarning;

            InputDeviceId = model.Input.DeviceId;
            InputVolume = model.Input.Volume;
            AvailableInputDevices = model.Input.AvailableDevices.Select(x => VoiceDevice.Create(x)).ToImmutableArray();

            OutputDeviceId = model.Output.DeviceId;
            OutputVolume = model.Output.Volume;
            AvailableInputDevices = model.Output.AvailableDevices.Select(x => VoiceDevice.Create(x)).ToImmutableArray();

            ActivationMode = model.Mode.Type;
            AutoThreshold = model.Mode.AutoThreshold;
            Threshold = model.Mode.Threshold;
            Shortcuts = model.Mode.Shortcut.Select(x => VoiceShortcut.Create(x)).ToImmutableArray();
            Delay = model.Mode.Delay;
        }
    }
}
