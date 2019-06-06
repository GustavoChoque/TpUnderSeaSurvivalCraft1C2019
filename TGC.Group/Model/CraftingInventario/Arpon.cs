﻿using BulletSharp;
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
        //private float tiempoAcumulado = 0;
        private TGCVector3 direccion;

        public Arpon(TgcMesh meshArpon, RigidBody rigidBodyArpon, TGCVector3 posicion, GameModel gmodel, TGCVector3 direccion)
        {
            this.meshArpon = meshArpon;
            this.rigidBodyArpon = rigidBodyArpon;
            this.meshArpon.Position = posicion;
            this.gmodel = gmodel;
            this.direccion = direccion;
        }

        public void Dispose()
        {
            meshArpon.Dispose();
            rigidBodyArpon.Dispose();
        }

        public RigidBody RigidBody { get => rigidBodyArpon; }

        public TgcMesh Mesh { get => meshArpon; }

        public TGCVector3 Position { get => meshArpon.Position; set => meshArpon.Position = value; }

        public TGCVector3 Direccion { get => direccion; }

        public void Update()
        {
                //tiempoAcumulado = tiempoAcumulado + gmodel.ElapsedTime;
                Position = new TGCVector3(RigidBody.CenterOfMassPosition.X, RigidBody.CenterOfMassPosition.Y, RigidBody.CenterOfMassPosition.Z);
                Mesh.Scale = new TGCVector3(1f, 1f, 1f);

                var dirMeshInicial = new TGCVector3(0, 0, 1);

                var numerador = TGCVector3.Dot(Direccion, dirMeshInicial);
                var denominador = 1;

            float theta;
            if (Direccion.X > 0)
            {
                theta = FastMath.Acos(numerador / denominador);
            }
            else
            {
                theta = FastMath.Acos(-numerador / denominador) + FastMath.PI;
            }

            Mesh.Transform = TGCMatrix.Scaling(Mesh.Scale) * TGCMatrix.RotationYawPitchRoll(theta, FastMath.PI_HALF, 0) * TGCMatrix.Translation(Mesh.Position);
        }

        public void Render()
        {
            if (gmodel.Personaje.estaCercaArpon(this))
            {
                meshArpon.Render();
            }
        }
    }
}
