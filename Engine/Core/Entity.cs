using Engine.Characters.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Core
{
    /// <summary>
    /// <c>Entity</c> is the root class of every object in the engine
    /// that can be saved to disk. The main purpose of <c>Entity</c>
    /// is to maintain a tree of all engine objects in a doubly linked
    /// list and provide functionality to traverse each node in the engine
    /// object tree and appropriately serialize that object to a string or 
    /// convert to binary format for eventual saving to disk or for providing
    /// that information to external tools.
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// <c>Entity</c> constructor. Initializes <c>_parent</c> to <c>null</c>
        /// and <c>_children</c> to an empty <c>List</c>.
        /// </summary>
        public Entity()
        {
            // Set _parent to null.
            _parent = null;
            // Initialize _children to an empty List.
            _children = new List<Entity>();
        }
        /// <summary>
        /// <c>AddChild</c> is a method that adds a user interface
        /// component to the <c>List</c> of children user interface
        /// components.
        /// </summary>
        /// <param name="child"></param>
        public virtual void AddChild(Entity child)
        {
            Children.Add(child);
        }
        public List<Entity> Children { get { return _children; } }
        /// <summary>
        /// <c>Parent</c> is a property that stores the parent user interface node
        /// if any.
        /// </summary>
        public Entity? Parent { get { return _parent; } }
        /// <summary>
        /// <c>_parent</c> is a reference to the instance of <c>Entity</c> that
        /// is one level up in the tree from the current entity.
        /// </summary>
        Entity? _parent;
        /// <summary>
        /// <c>_children</c> is a <c>List</c> of <c>Entity</c> objects that are
        /// child nodes of this game entity.
        /// </summary>
        List<Entity> _children;
    }
}
