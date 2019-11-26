using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Shaders;

namespace TGC.Group.Entities
{
    class Plane
    {
        public Vector3 center;
        public float scaleX;
        public float scaleY;
        public float scaleZ;
        public Texture planeTexture;
        public int totalVertices;
        private VertexBuffer vbPlane;

        public Plane()
        {
        }

        public void Create(float pscaleX, float pscaleY, float pscaleZ, Vector3 center)
        {
            scaleX = pscaleX;
            scaleY = pscaleY;
            scaleZ = pscaleZ;

            this.center = center;

            //Dispose de VertexBuffer anterior, si habia
            if (vbPlane != null && !vbPlane.Disposed)
            {
                vbPlane.Dispose();
            }

            //Crear vertexBuffer
            totalVertices = 2 * 3 * ((int)scaleX + 1) * ((int)scaleZ + 1);
            //totalVertices *= (int)ki * (int)kj;
            vbPlane = new VertexBuffer(typeof(CustomVertex.PositionTextured), totalVertices,
                D3DDevice.Instance.Device,
                Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);

            //Cargar vertices
            var dataIdx = 0;
            var data = new CustomVertex.PositionTextured[totalVertices];
            float width = (int)scaleX;
            float length = (int)scaleZ;
            for (var i = 0; i < width - 1; i++)
            {
                for (var j = 0; j < length - 1; j++)
                {
                    //Vertices
                    var v1 = new Vector3(center.X + i, center.Y * scaleY, center.Z + j);
                    var v2 = new Vector3(center.X + i, center.Y * scaleY, center.Z + (j + 1));
                    var v3 = new Vector3(center.X + (i + 1), center.Y * scaleY, center.Z + j);
                    var v4 = new Vector3(center.X + (i + 1), center.Y * scaleY, center.Z + (j + 1));

                    //Coordendas de textura
                    var t1 = new Vector2(i / width, j / length);
                    var t2 = new Vector2(i / width, (j + 1) / length);
                    var t3 = new Vector2((i + 1) / width, j / length);
                    var t4 = new Vector2((i + 1) / width, (j + 1) / length);

                    //Cargar triangulo 1
                    data[dataIdx] = new CustomVertex.PositionTextured(v1, t1.X, t1.Y);
                    data[dataIdx + 1] = new CustomVertex.PositionTextured(v2, t2.X, t2.Y);
                    data[dataIdx + 2] = new CustomVertex.PositionTextured(v4, t4.X, t4.Y);

                    //Cargar triangulo 2
                    data[dataIdx + 3] = new CustomVertex.PositionTextured(v1, t1.X, t1.Y);
                    data[dataIdx + 4] = new CustomVertex.PositionTextured(v4, t4.X, t4.Y);
                    data[dataIdx + 5] = new CustomVertex.PositionTextured(v3, t3.X, t3.Y);

                    dataIdx += 6;
                }
            }

            vbPlane.SetData(data, 0, LockFlags.None);
        }

        /// <summary>
        ///     Carga la textura del terreno
        /// </summary>
        public void LoadTexture(string path)
        {
            //Dispose textura anterior, si habia
            if (planeTexture != null && !planeTexture.Disposed)
            {
                planeTexture.Dispose();
            }

            //Rotar e invertir textura
            var b = (Bitmap)Image.FromFile(path);
            b.RotateFlip(RotateFlipType.Rotate90FlipX);
            planeTexture = Texture.FromBitmap(D3DDevice.Instance.Device, b, Usage.None, Pool.Managed);
        }

        // utilizo estos metodos para el render:
        public void Render()
        {
            D3DDevice.Instance.Device.Transform.World = Matrix.Identity;

            //Render terrain
            D3DDevice.Instance.Device.SetTexture(0, planeTexture);
            D3DDevice.Instance.Device.SetTexture(1, null);
            D3DDevice.Instance.Device.Material = D3DDevice.DEFAULT_MATERIAL;

            D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionTextured.Format;
            D3DDevice.Instance.Device.SetStreamSource(0, vbPlane, 0);
            D3DDevice.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, totalVertices / 3);
        }

        public void ExecuteRender(Effect effect)
        {
            TGCShaders.Instance.SetShaderMatrixIdentity(effect);

            //Render terrain
            effect.SetValue("texDiffuseMap", planeTexture);

            D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionTextured.Format;
            D3DDevice.Instance.Device.SetStreamSource(0, vbPlane, 0);

            var numPasses = effect.Begin(0);
            for (var n = 0; n < numPasses; n++)
            {
                effect.BeginPass(n);
                D3DDevice.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, totalVertices / 3);
                effect.EndPass();
            }
            effect.End();
        }

        public void Dispose()
        {
            if (vbPlane != null)
            {
                vbPlane.Dispose();
            }
            if (planeTexture != null)
            {
                planeTexture.Dispose();
            }
        }
    }
}
