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
using TGC.Core.Textures;

namespace LosTiburones.Model
{
    class Metal : InterfazRecolectable
    {
        public Metal(TgcTexture textura, TGCVector3 tamanio, TGCVector3 posicion, string nombrePar)
        {
            Objeto = TGCBox.fromSize(tamanio, textura);
            Objeto.Position = posicion;
            Objeto.Transform = TGCMatrix.Translation(Objeto.Position);
            Nombre = nombrePar;
            EsferaColision = new TgcBoundingSphere(posicion, 4f);
            EsferaColision.setRenderColor(Color.LimeGreen);
            renderiza = true;
        }

        private TGCBox objeto;
        public String nombre;
        private TgcBoundingSphere esferaColision;
        private bool renderiza;

        public bool colisionaCon(TgcBoundingCylinder cilindro)
        {
            return TgcCollisionUtils.testSphereCylinder(EsferaColision, cilindro);
        }

        public String dameNombre()
        {
            return Nombre;
        }

        public void Render()
        {
            if (renderiza)
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

        public void recolectado()
        {
            renderiza = false;
        }

        public TGCBox Objeto { get => objeto; set => objeto = value; }
        public TgcBoundingSphere EsferaColision { get => esferaColision; set => esferaColision = value; }
        public String Nombre { get => nombre; set => nombre = value; }
        public bool Renderiza { get => renderiza; set => renderiza = value; }
    }
}
