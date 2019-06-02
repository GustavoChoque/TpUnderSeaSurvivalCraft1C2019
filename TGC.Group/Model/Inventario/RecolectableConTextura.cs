using BulletSharp;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace LosTiburones.Model
{
    class RecolectableConTextura : ObjetoRecolectable
    {
        public RecolectableConTextura(TgcTexture textura, TGCVector3 tamanio, TGCVector3 posicion, string nombrePar):
            base(tamanio, posicion, nombrePar)
        {
            Objeto = TGCBox.fromSize(tamanio, textura);
            Objeto.Position = posicion;
            Objeto.Transform = TGCMatrix.Translation(Objeto.Position);
        }

        private TGCBox objeto;

        public override void Render()
        {
            base.Render();
            if (CuerpoRigido != null)
            {
                Objeto.Transform = new TGCMatrix(CuerpoRigido.InterpolationWorldTransform);
            }
            Objeto.Render();
        }

        public override void Dispose()
        {
            base.Dispose();
            Objeto.Dispose();
        }

        private TGCBox Objeto { get => objeto; set => objeto = value; }
    }
}
