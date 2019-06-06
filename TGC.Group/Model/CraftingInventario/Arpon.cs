using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model;

namespace LosTiburones.Model.CraftingInventario
{
    public class Arpon
    {
        private TgcMesh meshArpon;
        private RigidBody rigidBodyArpon;
        private GameModel gmodel;
        private float tiempoAcumulado = 0;
        private TGCVector3 direction;

        public Arpon(TgcMesh meshArpon, RigidBody rigidBodyArpon, TGCVector3 posicion, GameModel gmodel, TGCVector3 direction)
        {
            this.meshArpon = meshArpon;
            this.rigidBodyArpon = rigidBodyArpon;
            this.meshArpon.Position = posicion;
            this.gmodel = gmodel;
            this.direction = direction;
        }

        public void Dispose()
        {
            meshArpon.Dispose();
            rigidBodyArpon.Dispose();
        }

        public RigidBody RigidBody { get => rigidBodyArpon; }

        public TgcMesh Mesh { get => meshArpon; }

        public TGCVector3 Position { get => meshArpon.Position; set => meshArpon.Position = value; }

        public TGCVector3 Direction { get => direction; }

        public void Update()
        {
            tiempoAcumulado = tiempoAcumulado + gmodel.ElapsedTime;
            Position = new TGCVector3(RigidBody.CenterOfMassPosition.X, RigidBody.CenterOfMassPosition.Y, RigidBody.CenterOfMassPosition.Z);
            Mesh.Scale = new TGCVector3(1f, 1f, 1f);

            var dirMeshInicial = new TGCVector3(0, 0, 1);

            var numerador = TGCVector3.Dot(Direction, dirMeshInicial);
            float denominador = TGCVector3.Length(Direction) * TGCVector3.Length(dirMeshInicial);

            var theta = FastMath.Acos(numerador / denominador);

            Mesh.Transform = TGCMatrix.Scaling(Mesh.Scale) * TGCMatrix.RotationYawPitchRoll(theta, FastMath.PI_HALF, 0) * TGCMatrix.Translation(Mesh.Position);
        }
    }
}
