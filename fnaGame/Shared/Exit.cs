using Microsoft.Xna.Framework;
using Nez;
using System.Collections.Generic;

using fnaGame.Scenes;

namespace fnaGame.Shared
{
    /// <summary>
    /// exits the level and transitions to a specified level
    /// </summary>
    public class Exit : Component, IUpdatable
    {
        public Exit()
        {
            
        }
        public override void OnAddedToEntity(){
            var collider = Entity.GetComponent<BoxCollider>();
        }
        void IUpdatable.Update()
        {

        }
    }
}