using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Group.Model;

namespace LosTiburones.Model
{
    class ContactoTiburonCallback : ContactResultCallback
    {
        private Tiburon tiburon;
        private Personaje personaje;

        public ContactoTiburonCallback(Tiburon tiburon, Personaje personaje)
        {
            Personaje = personaje;
            Tiburon = tiburon;
        }

        public override bool NeedsCollision(BroadphaseProxy proxy)
        {
            // superclass will check CollisionFilterGroup and CollisionFilterMask
            if (base.NeedsCollision(proxy))
            {
                // if passed filters, may also want to avoid contacts between constraints
                return Tiburon.CuerpoRigido.CheckCollideWithOverride(proxy.ClientObject as CollisionObject);
            }

            return false;
        }

        public override float AddSingleResult(ManifoldPoint cp, CollisionObjectWrapper colObj0Wrap, int partId0, int index0, CollisionObjectWrapper colObj1Wrap, int partId1, int index1)
        {
            tiburon.aplicaDaño(personaje);
            return 0;
        }

        private Tiburon Tiburon { get => tiburon; set => tiburon = value; }
        private Personaje Personaje { get => personaje; set => personaje = value; }
    }
}
