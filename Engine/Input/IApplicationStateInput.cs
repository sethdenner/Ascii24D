using Engine.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Input {
    public interface IApplicationStateInput :
        IApplicationState,
        IHistorySerializable
    {
        public InputFrame InputFrame {
            get; set;
        }

    }
}
