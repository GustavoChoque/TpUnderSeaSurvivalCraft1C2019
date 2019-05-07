using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;

namespace LosTiburones.Model
{
    public class ObjetoRecolectable
    {
        public ObjetoRecolectable(TgcTexture textura, TGCVector3 tamanio, TGCVector3 posicion, string nombrePar)
        {
            Objeto = TGCBox.fromSize(tamanio, textura);
            Objeto.Position = posicion;
            Objeto.Transform = TGCMatrix.Translation(Objeto.Position);
            Nombre = nombrePar;
            EsferaColision = new TgcBoundingSphere(posicion, tamanio.X);
            EsferaColision.setRenderColor(Color.LimeGreen);
            renderiza = true;
        }

        public ObjetoRecolectable(TgcMesh mesh, TGCVector3 tamanio, TGCVector3 posicion, string nombrePar)
        {
            Mesh = mesh.createMeshInstance(nombrePar);
            Mesh.Position = posicion;
            Mesh.Transform = TGCMatrix.Translation(Mesh.Position);
            Nombre = nombrePar;
            EsferaColision = new TgcBoundingSphere(posicion, tamanio.X);
            EsferaColision.setRenderColor(Color.LimeGreen);
            renderiza = true;
        }

        private TGCBox objeto;
        private TgcMesh mesh;
        private String nombre;
        private TgcBoundingSphere esferaColision;
        private bool renderiza;

        public bool colisionaCon(TgcBoundingCylinder cilindro)
        {
            return TgcCollisionUtils.testSphereCylinder(EsferaColision, cilindro);
        }
        public void recolectado()
        {
            renderiza = false;
        }

        public void Render()
        {
            if (Renderiza)
            {
                Objeto.Render();
                EsferaColision.Render();
            }
        }

        public void Dispose()
        {
            EsferaColision.Dispose();
            Objeto.Dispose();
        }

        private TGCBox Objeto { get => objeto; set => objeto = value; }
        private TgcMesh Mesh { get => mesh; set => mesh = value; }
        public TgcBoundingSphere EsferaColision { get => esferaColision; set => esferaColision = value; }
        public String Nombre { get => nombre; set => nombre = value; }
        public bool Renderiza { get => renderiza; set => renderiza = value; }

    }
}
