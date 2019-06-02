using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace LosTiburones.Model
{
    class RecolectableConMesh: ObjetoRecolectable
    {
        public RecolectableConMesh(TgcMesh mesh, TGCVector3 tamanio, TGCVector3 posicion, string nombrePar):
            base(tamanio, posicion, nombrePar)
        {
            Mesh = mesh.createMeshInstance(nombrePar);
            Mesh.Position = posicion;
            Mesh.Scale = tamanio;
            Mesh.Transform = TGCMatrix.Translation(Mesh.Position);
        }

        private TgcMesh mesh;

        public override void Render()
        {
            base.Render();
            Mesh.Render();
        }

        public override void Dispose()
        {
            base.Dispose();
            Mesh.Dispose();
        }

        public TgcMesh Mesh { get => mesh; set => mesh = value; }
    }
}
