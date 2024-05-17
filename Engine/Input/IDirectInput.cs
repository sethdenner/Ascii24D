using SharpDX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Input
{
    /// <summary>
    /// <c>IDirectInput</c> declares the interface required
    /// to interact with <c>SharpDX.DirectInput</c>. 
    /// </summary>
    public interface IDirectInput
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<IDeviceInstance> GetDevices();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceInstance"></param>
        /// <returns></returns>
        public InputDevice? CreateInputDevice(IDeviceInstance deviceInstance);
        /// <summary>
        /// 
        /// </summary>
        public DirectInput? SharpDXDirectInput { get; set; }
    }
}
