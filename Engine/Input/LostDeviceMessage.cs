using Engine.Core;
using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Input {
    public class LostDeviceMessage(
        Guid deviceGuid,
        DeviceType deviceType
    ) : Message {
        delegate void LostDeviceMessageDelegate(
            Guid deviceGuid,
            DeviceType deviceType
        );

        Guid DeviceGuid = deviceGuid;
        DeviceType DeviceType = deviceType;
        public override void Send() {
            Messenger<LostDeviceMessageDelegate>.Trigger?.Invoke(
                DeviceGuid,
                DeviceType
            );
        }
    }
}
