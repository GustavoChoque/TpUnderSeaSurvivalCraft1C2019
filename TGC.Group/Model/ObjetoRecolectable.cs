using BulletSharp;
using System;
using System.Drawing;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace LosTiburones.Model
{
    abstract class ObjetoRecolectable
    {
        public ObjetoRecolectable(TGCVector3 tamanio, TGCVector3 posicion, string nombrePar)
        {
            Nombre = nombrePar;
            EsferaColision = new TgcBoundingSphere(posicion, tamanio.X);
            EsferaColision.setRenderColor(Color.LimeGreen); //Borrar si no se va a renderizar
            CuerpoRigido = null;
        }

        private String nombre;
        private TgcBoundingSphere esferaColision;
        private RigidBody cuerpoRigido;

        public bool colisionaCon(TgcBoundingCylinder cilindro)
        {
            return TgcCollisionUtils.testSphereCylinder(EsferaColision, cilindro);
        }

        public virtual void Render()
        {
            if (CuerpoRigido != null)
            {
                var centroDeMasa = CuerpoRigido.CenterOfMassPosition;
                var vector = new TGCVector3(centroDeMasa.X, centroDeMasa.Y, centroDeMasa.Z);
                EsferaColision.setCenter(vector);
            }
            //EsferaColision.Render();
        }

        public virtual void Dispose()
        {
            EsferaColision.Dispose();
        }

        public TgcBoundingSphere EsferaColision { get => esferaColision; set => esferaColision = value; }
        public String Nombre { get => nombre; set => nombre = value; }
        public RigidBody CuerpoRigido { get => cuerpoRigido; set => cuerpoRigido = value; }

    }
}
