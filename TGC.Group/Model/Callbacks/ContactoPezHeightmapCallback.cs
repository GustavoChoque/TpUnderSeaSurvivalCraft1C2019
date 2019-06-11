using System;
using BulletSharp;
using TGC.Group.Model;
using TGC.Core.Mathematica;

namespace LosTiburones.Model
{
    internal class ContactoPezHeightmapCallback : ContactResultCallback
    {
        private Pez pez;

        public ContactoPezHeightmapCallback(Pez pez)
        {
            this.pez = pez;
        }

        public override float AddSingleResult(ManifoldPoint cp, CollisionObjectWrapper colObj0Wrap, int partId0, int index0, CollisionObjectWrapper colObj1Wrap, int partId1, int index1)
        {
            //reboto
            pez.CurrentMoveDir *= -1;
            pez.Rotation += new TGCVector3(FastMath.PI, 0, 0);
            return 0;
        }

        public override bool NeedsCollision(BroadphaseProxy proxy)
        {
            // superclass will check CollisionFilterGroup and CollisionFilterMask
            if (base.NeedsCollision(proxy))
            {
                // if passed filters, may also want to avoid contacts between constraints
                return pez.RigidBody.CheckCollideWithOverride(proxy.ClientObject as CollisionObject);
            }

            return false;
        }
    }
}