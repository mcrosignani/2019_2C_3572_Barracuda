using BulletSharp;
using TGC.Core.SceneLoader;
using TGC.Core.Mathematica;

namespace TGC.Group.Model
{
    public class ItemModel
    {
        public TgcMesh Mesh { get; set; }
        public RigidBody RigidBody { get; set; }
        public bool IsCollectable { get; set; } = false;
        public TGCVector3 CollectablePosition { get; set; }

        public ItemModel()
        { }

        public ItemModel(TgcMesh mesh)
        {
            Mesh = mesh;
        }

        public void Dispose()
        {
            Mesh.Dispose();

            if (RigidBody != null)
            {
                RigidBody.Dispose();
            }
        }
    }
}
