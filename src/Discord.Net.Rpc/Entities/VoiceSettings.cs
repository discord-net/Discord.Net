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
            if (model.AutomaticGainControl.IsSpecified)
                AutomaticGainControl = model.AutomaticGainControl.Value;
            if (model.EchoCancellation.IsSpecified)
                EchoCancellation = model.EchoCancellation.Value;
            if (model.NoiseSuppression.IsSpecified)
                NoiseSuppression = model.NoiseSuppression.Value;
            if (model.QualityOfService.IsSpecified)
                QualityOfService = model.QualityOfService.Value;
            if (model.SilenceWarning.IsSpecified)
                SilenceWarning = model.SilenceWarning.Value;

            if (model.Input.DeviceId.IsSpecified)
                InputDeviceId = model.Input.DeviceId.Value;
            if (model.Input.Volume.IsSpecified)
                InputVolume = model.Input.Volume.Value;
            if (model.Input.AvailableDevices.IsSpecified)
                AvailableInputDevices = model.Input.AvailableDevices.Value.Select(x => VoiceDevice.Create(x)).ToImmutableArray();

            if (model.Output.DeviceId.IsSpecified)
                OutputDeviceId = model.Output.DeviceId.Value;
            if (model.Output.Volume.IsSpecified)
                OutputVolume = model.Output.Volume.Value;
            if (model.Output.AvailableDevices.IsSpecified)
                AvailableInputDevices = model.Output.AvailableDevices.Value.Select(x => VoiceDevice.Create(x)).ToImmutableArray();

            if (model.Mode.Type.IsSpecified)
                ActivationMode = model.Mode.Type.Value;
            if (model.Mode.AutoThreshold.IsSpecified)
                AutoThreshold = model.Mode.AutoThreshold.Value;
            if (model.Mode.Threshold.IsSpecified)
                Threshold = model.Mode.Threshold.Value;
            if (model.Mode.Shortcut.IsSpecified)
                Shortcuts = model.Mode.Shortcut.Value.Select(x => VoiceShortcut.Create(x)).ToImmutableArray();
            if (model.Mode.Delay.IsSpecified)
                Delay = model.Mode.Delay.Value;
        }
    }
}
