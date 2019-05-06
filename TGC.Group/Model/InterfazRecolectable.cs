using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;

namespace LosTiburones.Model
{
    public interface InterfazRecolectable
    {

        bool colisionaCon(TgcBoundingCylinder cilindro); //Todos los objetos deben saber si colisionan con la interaccino con el personaje
        void recolectado(); //Todos los objetos Recolectables deben poder borrarse cuando se recolectan
        String dameNombre(); //Los objetos deben saber mostrar su nombre para poder mostrar un cartel por pantalla cuando se pasa cerca del objeto
        
    }
}
