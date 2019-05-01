using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model;

namespace LosTiburones.Model
{
    class Tiburon
    {
        private TGCVector3 posInicial;
        private TgcMesh mesh;
        private float anguloRebote;
        private float anguloGiro;

        public Tiburon(TGCVector3 posInicial, TgcMesh mesh)
        {
            this.posInicial = posInicial;
            this.mesh = mesh;
            this.mesh.Position = posInicial;
        }

        public Boolean tocoBorde() {
            return Position.X >= 5000 || Position.X <= -5000 || Position.Z >= 5000 || Position.Z <= -5000;
     
        }

        public void reboto(GameModel model)
        {
            //Se mueve en diagonal
            float x = model.MovementSpeed * model.ElapsedTime * 0.707f;
            float y = 0;
            float z = model.MovementSpeed * model.ElapsedTime * -0.707f;
        }

        private int sentidoRandom(GameModel model)
        {
            if (model.GetRandom.NextDouble() < 0.5)
            {
                return 1;
            }
            else return -1;
        }

        public void moverse(GameModel model)
        {
            //Cada X segundos cambio el sentido de movimiento
            if ((model.ElapsedTime * 1000) % 10 < 0.2)
            {
                //Encuentro el vector rotacion
                float x = 0; // model.MovementSpeed * model.ElapsedTime * FastMath.Cos(anguloGiro);
                float y = (float) model.GetRandom.NextDouble() * (FastMath.PI / 2) * sentidoRandom(model);
                float z = 0; // model.MovementSpeed * model.ElapsedTime * FastMath.Sin(anguloGiro);

                Rotation += new TGCVector3(x, y, z);

                Transform = TGCMatrix.RotationYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z);
            }
            
            //Reboto en un angulo que varia entre 0 y pi/2
            //anguloRebote = (float) model.GetRandom.NextDouble() * (FastMath.PI / 2);
            

            
            /*
            TGCVector3 deltaPosition = new TGCVector3(x, y, z);
            Position += deltaPosition;

            if (this.tocoBorde()){
                this.reboto(model);
            }
            */
            /*

            if (!((posInicial.X + 500 > Position.X) && (posInicial.X - 500 < Position.X)))
            {
                currentMoveDir *= -1;
                Rotation += new TGCVector3(0, FastMath.PI, 0);
            }
            */

            //Transform = TGCMatrix.RotationYawPitchRoll(Rotation.X, Rotation.Y, Rotation.Z) * TGCMatrix.Translation(Position);
        }

        public void Render()
        {
            this.mesh.Render();
        }

        public void Dispose()
        {
            this.mesh.Dispose();
        }

		public TGCVector3 Rotation { get => mesh.Rotation; set => mesh.Rotation = value; }
        public TGCMatrix Transform { get => mesh.Transform; set => mesh.Transform = value; }
        public TGCVector3 Position { get => mesh.Position; set => mesh.Position = value; }
        //public float CurrentMoveDir { get => currentMoveDir; set => currentMoveDir = value; }
    }
}
