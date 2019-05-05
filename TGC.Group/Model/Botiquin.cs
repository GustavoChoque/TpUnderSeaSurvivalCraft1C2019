using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LosTiburones.Model
{
    class Botiquin : ObjetoDeInventario
    {
        public Botiquin()
        {
            this.nombre = "Botiquin";
            this.cantidad = 0;
        }
    }
}
