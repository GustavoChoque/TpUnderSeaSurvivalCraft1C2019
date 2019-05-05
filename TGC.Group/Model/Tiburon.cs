using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Sound;
using TGC.Group.Model;

namespace LosTiburones.Model
{
    class Tiburon
    {
        private GameModel gmodel;
        private TgcMesh mesh;
        private float velocidad = 100f;
        private float radioDeteccion = 1000;
        private TGCVector3 movDir = new TGCVector3(1, 0, 1);
        private float tiempoCambioRumbo = 10;
        private float contadorTiempo = 0;
        private TgcMp3Player sonido = new TgcMp3Player();
        private Boolean escapo = false;

        public Tiburon(TgcMesh mesh, GameModel gmodel)
        {
            this.mesh = mesh;
            this.gmodel = gmodel;
            movDir.Normalize();
        }

        public Boolean tocoBorde()
        {
            return Position.X >= 20000 || Position.X <= -20000 || Position.Z >= 20000 || Position.Z <= -20000;
        }

        public Boolean estoyCercaDelPersonaje(Personaje personaje)
        {
            return FastMath.Sqrt(FastMath.Pow2(personaje.Position.X - this.Position.X) + FastMath.Pow2(personaje.Position.Z - this.Position.Z)) < radioDeteccion;
        }

        public void moverse(Escenario escenario)
        {
            if (!gmodel.GetPersonaje.cercaDeNave(escenario.getBarco()))
            {
                escapo = true;

                if (estoyCercaDelPersonaje(gmodel.GetPersonaje))
                {
                    Velocidad = 200f;
                    perseguir(gmodel.GetPersonaje);
                    sonido = gmodel.hacerSonar("Music\\SharkNear.mp3");
                }
                else
                {
                    Velocidad = 100f;
                    pasear();
                    gmodel.detener(sonido);
                }
            }
            else
            {
                if (escapo)
                {
                    Position = new TGCVector3((float)gmodel.GetRandom.NextDouble() * 1000 + 1000, Position.Y, (float)gmodel.GetRandom.NextDouble() * 1000 + 1000);
                }

                escapo = false;

                Velocidad = 100f;
                pasear();
                gmodel.detener(sonido);
            }            
        }

        private void perseguir(Personaje personaje)
        {
            var vectorPosPers = new TGCVector3(personaje.Position);
            var vectorPosTibu = new TGCVector3(Position);
            vectorPosPers.Subtract(vectorPosTibu);

            var nuevaDir = new TGCVector3(vectorPosPers);
            nuevaDir.Normalize();

            var thetaViejaDir = FastMath.Atan2(movDir.Z , movDir.X);
            var thetaNuevaDir = FastMath.Atan2(nuevaDir.Z , nuevaDir.X);

            float anguloEntreDosVectores = thetaViejaDir - thetaNuevaDir;

            RotateY(anguloEntreDosVectores);

            movDir = new TGCVector3(nuevaDir);
            var dirPosta = new TGCVector3(movDir);
            dirPosta.Multiply(gmodel.ElapsedTime);
            dirPosta.Multiply(Velocidad);
            Move(dirPosta);
        }

        private void pasear()
        {
            contadorTiempo = contadorTiempo + gmodel.ElapsedTime;

            if (contadorTiempo < tiempoCambioRumbo)
            {
                var dirPosta = new TGCVector3(movDir);
                dirPosta.Multiply(gmodel.ElapsedTime);
                dirPosta.Multiply(Velocidad);
                Move(dirPosta);
            }
            else
            {
                cambioRumbo();
            }
        }

        private void cambioRumbo()
        {
            contadorTiempo = 0;

            float x = (float)gmodel.GetRandom.NextDouble() * 2 - 1;
            float y = 0;
            float z = (float)gmodel.GetRandom.NextDouble() * 2 - 1;

            var nuevaDir = new TGCVector3(x, y, z);
            nuevaDir.Normalize();

            var thetaViejaDir = FastMath.Atan2(movDir.Z, movDir.X);
            var thetaNuevaDir = FastMath.Atan2(nuevaDir.Z, nuevaDir.X);

            float anguloEntreDosVectores = thetaViejaDir - thetaNuevaDir;

            RotateY(anguloEntreDosVectores);

            movDir = new TGCVector3(nuevaDir);
            var dirPosta = new TGCVector3(movDir);
            dirPosta.Multiply(gmodel.ElapsedTime);
            dirPosta.Multiply(Velocidad);
            Move(dirPosta);
        }
        
        public void Render()
        {
            this.mesh.Render();
        }

        public void Dispose()
        {
            this.mesh.Dispose();
        }

        public TGCMatrix Transform { get => mesh.Transform; set => mesh.Transform = value; }
        public TGCVector3 Position { get => mesh.Position; set => mesh.Position = value; }
        public float Velocidad { get => velocidad; set => velocidad = value; }
        public void RotateX(float angle)
        {
            mesh.RotateX(angle);
        }

        public void RotateY(float angle)
        {
            mesh.RotateY(angle);
        }

        public void RotateZ(float angle)
        {
            mesh.RotateZ(angle);
        }

        public void Move(TGCVector3 vector)
        {
            mesh.Move(vector);
        }
    }
}
