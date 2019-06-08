using BulletSharp;
using LosTiburones.Model.CraftingInventario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LosTiburones.Model.Callbacks
{
    public class TechoArponCallback : ContactResultCallback
    {
        private Arpon arpon;
        private Escenario escenario;

        public TechoArponCallback(Arpon arpon, Escenario escenario)
        {
            this.arpon = arpon;
            this.escenario = escenario;
        }

        public override float AddSingleResult(ManifoldPoint cp, CollisionObjectWrapper colObj0Wrap, int partId0, int index0, CollisionObjectWrapper colObj1Wrap, int partId1, int index1)
        {
            arpon.Deshabilitar();
            return 0;
        }

        public override bool NeedsCollision(BroadphaseProxy proxy)
        {
            // superclass will check CollisionFilterGroup and CollisionFilterMask
            if (base.NeedsCollision(proxy))
            {
                // if passed filters, may also want to avoid contacts between constraints
                return arpon.RigidBody.CheckCollideWithOverride(proxy.ClientObject as CollisionObject);
            }

            return false;
        }
    }
}
