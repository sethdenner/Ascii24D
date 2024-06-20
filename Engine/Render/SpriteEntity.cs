using Engine.Characters.UI;
using Engine.Core;
using Engine.Core.ECS;
using Engine.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Render {
    public class SpriteEntity : Entity {
        public SpriteEntity(
            int width,
            int height,
            Vector3 modelPosition,
            bool transformToScreenSpace,
            ConsolePixel[] bufferPixels
        ) {
            AddComponent(new SpriteComponent(
                width,
                height,
                modelPosition,
                transformToScreenSpace,
                bufferPixels
            ));

            Message.Register<UpdateSpritePixelsMessage.Delegate>((
                entityID,
                width,
                height,
                bufferPixels
            ) => {
                if (EntityID == entityID) {
                    var component = GetComponent<SpriteComponent>();
                    component.Width = width;
                    component.Height = height;
                    component.BufferPixels = bufferPixels;
                    SetComponent(component);
                }
            });
            Message.Register<UpdateSpriteModelPositionMessage.Delegate>((
                entityID,
                position
            ) => {
                if (EntityID == entityID) {
                    var component = GetComponent<SpriteComponent>();
                    component.ModelPosition = position;
                    SetComponent(component);
                }
            });
        }

    }
}
