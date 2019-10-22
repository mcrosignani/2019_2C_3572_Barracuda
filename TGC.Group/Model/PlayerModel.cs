using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    public class PlayerModel
    {
        private float surfaceYPosition;

        public TGCVector3 Position { get; set; }

        public PlayerModel(float surfacePosition, TGCVector3 initialPosition)
        {
            surfaceYPosition = surfacePosition;
            Position = initialPosition;
        }

        public void Init()
        { }

        public void Update(float elapsedTime)
        { }

        public void Render()
        { }

        public void Dispose()
        { }

        public bool UnderSurface()
        {
            return Position.Y < surfaceYPosition;
        }
    }
}
