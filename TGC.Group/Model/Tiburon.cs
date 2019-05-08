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
        private float radioCercania = 750f;
        private float radioLejania = 1250f;
        private TGCVector3 movDir = new TGCVector3(1, 0, 1);
        private TGCVector3 posicionPersonajeOriginal = new TGCVector3(1, 0, 1);
        private float tiempoCambioRumbo = 10;
        private float contadorTiempo = 0;
        private TgcMp3Player sonido = new TgcMp3Player();
        private Boolean estoyAlejandomeDeLaNave = false;
        private Boolean recienEmpiezoAAlejarme = true;
        private Boolean loEstoyPersiguiendo = false;

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

        private Boolean estoyLejosDelPersonaje(TGCVector3 posicionOriginal)
        {
            return FastMath.Sqrt(TGCVector3.LengthSq(posicionOriginal - this.Position)) > radioLejania;
        }

        public Boolean estoyCercaDelPersonaje(Personaje personaje)
        {
            return FastMath.Sqrt(TGCVector3.LengthSq(personaje.Position - this.Position)) < radioCercania;
        }

        public void moverse(Escenario escenario)
        {
            if (estoyAlejandomeDeLaNave)
            {
                //me muevo alejandome
                Velocidad = 1000f;

                if (recienEmpiezoAAlejarme)
                {
                    recienEmpiezoAAlejarme = false;
                    var nuevaDir = new TGCVector3(movDir);
                    nuevaDir.Normalize();
                    nuevaDir.Multiply(-1);
                    cambioRumbo(nuevaDir);
                    gmodel.detener(sonido);
                    posicionPersonajeOriginal = new TGCVector3(gmodel.Personaje.Position);
                }
                
                var dirPosta = new TGCVector3(movDir);
                dirPosta.Multiply(gmodel.ElapsedTime);
                dirPosta.Multiply(Velocidad);
                Move(dirPosta);
                
                if (estoyLejosDelPersonaje(posicionPersonajeOriginal))
                {
                    estoyAlejandomeDeLaNave = false;
                }
            }
            else
            {
                recienEmpiezoAAlejarme = true;

                if (loEstoyPersiguiendo)
                {
                    Velocidad = 200f;
                    perseguir(gmodel.Personaje);
                    sonido = gmodel.hacerSonar("Music\\SharkNear.mp3");

                    if (gmodel.Personaje.estaCercaDeNave())
                    {
                        estoyAlejandomeDeLaNave = true;
                        loEstoyPersiguiendo = false;
                    }
                }
                else
                {
                    Velocidad = 100f;
                    pasear();
                    gmodel.detener(sonido);

                    if (estoyCercaDelPersonaje(gmodel.Personaje))
                    {
                        loEstoyPersiguiendo = true;
                    }
                }
            }
        }

        private void perseguir(Personaje personaje)
        {
            var vectorPosPers = new TGCVector3(personaje.Position);
            var vectorPosTibu = new TGCVector3(Position);
            vectorPosPers.Subtract(vectorPosTibu);

            var nuevaDir = new TGCVector3(vectorPosPers);
            nuevaDir.Normalize();

            var thetaViejaDir = FastMath.Atan2(movDir.Z, movDir.X);
            var thetaNuevaDir = FastMath.Atan2(nuevaDir.Z, nuevaDir.X);

            float anguloEntreDosVectores = thetaViejaDir - thetaNuevaDir;

            RotateY(anguloEntreDosVectores);

            movDir = new TGCVector3(nuevaDir);
            var dirPosta = new TGCVector3(movDir);

            if (Position.Y >= 0)
            {
                dirPosta.Y = 0;
            }

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
                var nuevaDir = new TGCVector3((float)gmodel.GetRandom.NextDouble() * 2 - 1, 0, (float)gmodel.GetRandom.NextDouble() * 2 - 1);
                cambioRumbo(nuevaDir);
            }
        }

        private void cambioRumbo(TGCVector3 vector)
        {
            contadorTiempo = 0;

            var nuevaDir = new TGCVector3(vector.X, vector.Y, vector.Z);
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
