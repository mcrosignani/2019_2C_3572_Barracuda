using BulletSharp;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    public class ItemModel
    {
        public TgcMesh Mesh { get; set; }
        public RigidBody RigidBody { get; set; }
        public bool IsCollectable { get; set; } = false;

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
