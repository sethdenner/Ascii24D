using Engine.Core.ECS;
using Engine.Native;
using System.Numerics;
using Engine.Core;
using Engine.Render;
namespace Engine.Characters.UI {
    public class UIProceduralSpritesEntity : Entity {
        public UIProceduralSpritesEntity(
            int spriteEntityId,
            int width,
            int height,
            Vector3 position,
            ConsolePixel backgroundPixel,
            ConsolePixel borderPixel,
            int borderWidth,
            int paddingTop,
            int paddingRight,
            int paddingBottom,
            int paddingLeft,
            string text,
            byte textForegroundColor,
            byte textBackgroundColor,
            ChildLayout childLayout,
            bool autoSize,
            int autoSizeMargin = 0
        ) {
            AddComponent(new UIProceduralSpritesComponent(
                spriteEntityId,
                width,
                height,
                position,
                backgroundPixel,
                borderPixel,
                borderWidth,
                paddingTop,
                paddingRight,
                paddingBottom,
                paddingLeft,
                text,
                textForegroundColor,
                textBackgroundColor,
                childLayout,
                autoSize,
                autoSizeMargin
            ));

            Message.Register<UpdateProceduralUITextMessage.Delegate>(
                (int entityID, string text) => {
                    if (EntityID == entityID) {
                        var component = GetComponent<
                            UIProceduralSpritesComponent
                        >();
                        component.Text = text;
                        component.Regenerate = true;
                        SetComponent(component);
                    }
                }
            );
            Message.Register<UpdateProceduralUIWidthMessage.Delegate>(
                (int entityID, int width) => {
                    if (EntityID == entityID) {
                        var component = GetComponent<
                            UIProceduralSpritesComponent
                        >();
                        component.Width = width;
                        component.Regenerate = true;
                        SetComponent(component);
                    }
                }
            );
            Message.Register<UpdateProceduralUIHeightMessage.Delegate>(
                (int entityID, int height) => {
                    if (EntityID == entityID) {
                        var component = GetComponent<
                            UIProceduralSpritesComponent
                        >();
                        component.Height = height;
                        component.Regenerate = true;
                        SetComponent(component);
                    }
                }
            );
            Message.Register<UpdateProceduralUIPositionMessage.Delegate>(
                (int entityID, Vector3 position) => {
                    if (EntityID == entityID) {
                        var component = GetComponent<
                            UIProceduralSpritesComponent
                        >();
                        component.Position = position;
                        component.Regenerate = true;
                        SetComponent(component);
                    }
                }
            );
            Message.Register<ApplicationWindowResizedMessage.Delegate>(
                (newSizeRect) => {
                    var component = GetComponent<
                        UIProceduralSpritesComponent
                    >();
                    if (component.AutoSize) {
                        int width = (
                            newSizeRect.Right -
                            newSizeRect.Left -
                            2 * component.AutoSizeMargin
                        );
                        int height = (
                            newSizeRect.Bottom -
                            newSizeRect.Top - 
                            2 * component.AutoSizeMargin
                        );
                        Vector3 position = new(
                            component.AutoSizeMargin,
                            component.AutoSizeMargin,
                            component.Position.Z
                        );
                        component.Width = width;
                        component.Height = height;
                        component.Position = position;
                    }
                    component.Regenerate = true;
                    SetComponent(component);
                }
            );
        }
    }
}
