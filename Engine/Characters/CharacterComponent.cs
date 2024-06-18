using System.Numerics;

namespace Engine.Characters {
    public struct CharacterComponent() {
        /// <summary>
        /// 
        /// </summary>
        public Vector3 Position {
            get; set;
        } = Vector3.Zero;
        /// <summary>
        /// 
        /// </summary>
        public int[] SpriteEntityIDs { get; set; } = [];
        /// <summary>
        /// 
        /// </summary>
        public int[] ChildEntityIDs { get; set; } = [];
    }
}
