using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;

namespace LosTiburones.Model
{
    public abstract class ObjetoDeInventario
    {
        public String nombre;
        public int cantidad;
        private TgcBoundingSphere esferaColision;
        private bool rending;

        public bool colisionaCon(TgcBoundingCylinder cilindro)
        {
            return TgcCollisionUtils.testSphereCylinder(EsferaColision, cilindro);
        }
        public void stopRending()
        {
            Rending = false;
            EsferaColision.Dispose();
            EsferaColision = null;
        }

        public TgcBoundingSphere EsferaColision { get => esferaColision; set => esferaColision = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public bool Rending { get => rending; set => rending = value; }
    }
}
