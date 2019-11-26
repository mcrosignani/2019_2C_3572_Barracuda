using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Camara;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Text;

namespace TGC.Group.Model.Levels
{
    public abstract class CustomModel
    {
        public UnderseaModel GameModel;
        public TgcCamera Camera;
        public TgcD3dInput Input;
        public string MediaDir;
        public string ShadersDir;
        public TgcFrustum Frustum;
        public TgcText2D DrawText;

        protected CustomModel(UnderseaModel gameModel,
            TgcCamera camera, 
            TgcD3dInput input, 
            string mediaDir, 
            string shadersDir, 
            TgcFrustum frustum, 
            TgcText2D drawText)
        {
            GameModel = gameModel;
            Camera = camera;
            Input = input;
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
            Frustum = frustum;
            DrawText = drawText;
        }

        public virtual void Init() { }
        public virtual void Update(float elapsedTime) { }
        public virtual void Render(float elapsedTime) { }
        public virtual void Dispose() { }
    }
}
