using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace LosTiburones.Model
{
    //Esta clase describe un movimiento circular implementando la clase Movimiento Pez
    class MovimientoCircular : MovimientoPez
    {
        //Es necesario definirle, al instanciar, la posicion del centro, el radio y la velocidad angular que tendra el movimiento circular
        public MovimientoCircular(TGCVector3 centroPar ,float radio, float velocidadAngularPar)
        {
            this.Centro = centroPar;
            this.Radio = radio;
            this.VelocidadAngular = velocidadAngularPar;
        }
        private TGCVector3 centro;
        private float anguloRotacion = 0f;
        private float velocidadAngular = 0.35f;
        private float radio;
        public void mover(TgcMesh pez, float ElapsedTime)
        {
            anguloRotacion += velocidadAngular * ElapsedTime;
            if (anguloRotacion > 360f) anguloRotacion -= 360f;
            pez.Position = new TGCVector3((float)(radio * Math.Cos(anguloRotacion)) + centro.X, centro.Y, (float)(radio * Math.Sin(anguloRotacion)) + centro.Z);
            pez.Rotation -= new TGCVector3(0, velocidadAngular * ElapsedTime, 0);
            pez.Transform = TGCMatrix.RotationYawPitchRoll(pez.Rotation.Y, pez.Rotation.X, pez.Rotation.Z) * TGCMatrix.Translation(pez.Position);
        }

        public TGCVector3 Centro { get => centro; set => centro = value; }

        public float AnguloRotacion { get => anguloRotacion; set => anguloRotacion = value; }

        public float VelocidadAngular { get => velocidadAngular; set => velocidadAngular = value; }

        public float Radio { get => radio; set => radio = value; }
    }
}
