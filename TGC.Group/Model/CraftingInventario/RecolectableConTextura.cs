using BulletSharp;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;

namespace LosTiburones.Model
{
    class RecolectableConTextura : ObjetoRecolectable
    {
        public RecolectableConTextura(TgcTexture textura, TGCVector3 tamanio, TGCVector3 posicion, string nombrePar):
            base(tamanio, posicion, nombrePar)
        {
            Mesh = TGCBox.fromSize(tamanio, textura).ToMesh(nombrePar).createMeshInstance(nombrePar);
            Mesh.Position = posicion;
            Mesh.Transform = TGCMatrix.Translation(Mesh.Position);
        }

        private TgcMesh objeto;

        public override void Render()
        {
            base.Render();
            if (CuerpoRigido != null)
            {
                Mesh.Transform = new TGCMatrix(CuerpoRigido.InterpolationWorldTransform);
            }
            Mesh.Render();
        }

        public override void Dispose()
        {
            base.Dispose();
            Mesh.Dispose();
        }

        //public TgcMesh Mesh { get => objeto; set => objeto = value; }
    }
}
