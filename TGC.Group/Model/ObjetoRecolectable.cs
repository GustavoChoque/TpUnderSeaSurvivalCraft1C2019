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
        //Objetos particulares con textura        
        public ObjetoRecolectable(TgcTexture textura, TGCVector3 tamanio, TGCVector3 posicion, string nombrePar)
        {
            Objeto = TGCBox.fromSize(tamanio, textura);
            Objeto.Position = posicion;
            Objeto.Transform = TGCMatrix.Translation(Objeto.Position);
            Nombre = nombrePar;
            Mesh = null;
            EsferaColision = new TgcBoundingSphere(posicion, tamanio.X);
            EsferaColision.setRenderColor(Color.LimeGreen);
        }

        //Corales u objetos de escenario
        public ObjetoRecolectable(TgcMesh mesh, TGCVector3 tamanio, TGCVector3 posicion, string nombrePar)
        {
            Mesh = mesh.createMeshInstance(nombrePar);
            Mesh.Position = posicion;
            Mesh.Transform = TGCMatrix.Translation(Mesh.Position);
            Nombre = nombrePar;
            Objeto = null;
            EsferaColision = new TgcBoundingSphere(posicion, tamanio.X);
            EsferaColision.setRenderColor(Color.LimeGreen);
        }

        private TGCBox objeto;
        private TgcMesh mesh;
        private String nombre;
        private TgcBoundingSphere esferaColision;

        public bool colisionaCon(TgcBoundingCylinder cilindro)
        {
            return TgcCollisionUtils.testSphereCylinder(EsferaColision, cilindro);
        }

        public void Render()
        {
            EsferaColision.Render();
            if (Objeto == null)
            {
                Mesh.Render();
            }
            else
            {
                Objeto.Render();
            }
        }

        public void Dispose()
        {
            EsferaColision.Dispose();
            if (Objeto == null)
            {
                Mesh.Dispose();
            }
            else
            {
                Objeto.Dispose();
            }
        }

        private TGCBox Objeto { get => objeto; set => objeto = value; }
        private TgcMesh Mesh { get => mesh; set => mesh = value; }
        public TgcBoundingSphere EsferaColision { get => esferaColision; set => esferaColision = value; }
        public String Nombre { get => nombre; set => nombre = value; }

    }
}
