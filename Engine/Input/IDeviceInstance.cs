using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Input
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDeviceInstance
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public DeviceType GetDeviceType();
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Guid GetInstanceGuid();
    }

}
