using BulletSharp;
using BulletSharp.Math;
using Microsoft.DirectX.Direct3D;
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
        private RigidBody cuerpoRigido;
        private float fuerzaMordida = 5f;

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
                if (recienEmpiezoAAlejarme)
                {
                    recienEmpiezoAAlejarme = false;
                    var nuevaDir = new TGCVector3(movDir);
                    nuevaDir.Multiply(-1f);
                    cambioRumbo(nuevaDir);
                    gmodel.Escenario.detenerSonidoTiburonCerca();
                    posicionPersonajeOriginal = new TGCVector3(gmodel.Personaje.Position);
                }

                movetePosta(movDir, 500f);

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
                    perseguir(gmodel.Personaje);
                    gmodel.Escenario.hacerSonarTiburonCerca();

                    if (gmodel.Personaje.estaCercaDeNave())
                    {
                        estoyAlejandomeDeLaNave = true;
                        loEstoyPersiguiendo = false;
                    }
                }
                else
                {
                    pasear();
                    gmodel.Escenario.detenerSonidoTiburonCerca();

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
                dirPosta.Y = -0.05f;
            }

            movetePosta(dirPosta, 300f);
        }

        private void pasear()
        {
            contadorTiempo = contadorTiempo + gmodel.ElapsedTime;

            if (contadorTiempo < tiempoCambioRumbo)
            {
                movetePosta(movDir, 100f);
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

            var nuevaDir = new TGCVector3(vector);
            nuevaDir.Normalize();

            var thetaViejaDir = FastMath.Atan2(movDir.Z, movDir.X);
            var thetaNuevaDir = FastMath.Atan2(nuevaDir.Z, nuevaDir.X);

            float anguloEntreDosVectores = thetaViejaDir - thetaNuevaDir;

            RotateY(anguloEntreDosVectores);
            
            movetePosta(nuevaDir, Velocidad);
        }

        public void Render()
        {
            //Logica de mover y rotar 90 grados la capsula de cuerpo Rigido
            this.mesh.RotateZ(Geometry.DegreeToRadian(90f));
            this.mesh.UpdateMeshTransform();
            TGCMatrix transform = new TGCMatrix(this.mesh.Transform);
            var bulletTransform = transform.ToBsMatrix;
            CuerpoRigido.MotionState.SetWorldTransform(ref bulletTransform);
            this.mesh.RotateZ(-(Geometry.DegreeToRadian(90f)));
            this.mesh.UpdateMeshTransform();
            
            this.mesh.Render();
        }

        public void Dispose()
        {
            this.mesh.Dispose();
        }

        public TGCMatrix Transform { get => mesh.Transform; set => mesh.Transform = value; }
        public TGCVector3 Position { get => mesh.Position; set => mesh.Position = value; }
        public TGCVector3 Scale { get => mesh.Scale; set => mesh.Scale = value; }
        public float Velocidad { get => velocidad; set => velocidad = value; }
        public RigidBody CuerpoRigido { get => cuerpoRigido; set => cuerpoRigido = value; }
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
            /*Matrix newTransf;
            CuerpoRigido.MotionState.GetWorldTransform(out newTransf);
            newTransf.Origin = vector.ToBulletVector3();
            CuerpoRigido.MotionState.SetWorldTransform(ref newTransf);*/
            
            mesh.Move(vector);
        }

        private void movetePosta(TGCVector3 vector, float velocidad)
        {
            var tempDir = new TGCVector3(vector);
            tempDir.Normalize();
            movDir = tempDir;

            Velocidad = velocidad;
            var dirPosta = new TGCVector3(vector);
            dirPosta.Multiply(gmodel.ElapsedTime);
            dirPosta.Multiply(Velocidad);
            Move(dirPosta);
        }

        public void aplicaDaño(Personaje personaje)
        {
            personaje.sufriDanio(this.fuerzaMordida);
        }

        public bool RecienEmpiezoAAlejarme()
        {
            return recienEmpiezoAAlejarme;
        }
    }
}
