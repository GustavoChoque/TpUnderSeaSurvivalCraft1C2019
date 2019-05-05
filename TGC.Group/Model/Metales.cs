using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace LosTiburones.Model
{
    class Metales : ObjetoDeInventario 
    {
        public Metales(TgcTexture textura, TGCVector3 tamanio, TGCVector3 posicion, string nombre)
        {
            Objeto = TGCBox.fromSize(tamanio, textura);
            Objeto.Position = posicion;
            Objeto.Transform = TGCMatrix.Translation(Objeto.Position);
            Nombre = nombre;
            cantidad = 1;
            EsferaColision = new TgcBoundingSphere(posicion, 4f);
            EsferaColision.setRenderColor(Color.LimeGreen);
            Rending = true;
        }

        private TGCBox objeto;

        public void Render()
        {
            if (Rending)
            {
                Objeto.Render();
                EsferaColision.Render();
            }            
        }

        public void Dispose()
        {
            Objeto.Dispose();
        }

        public TGCBox Objeto { get => objeto; set => objeto = value; }

    }
}
