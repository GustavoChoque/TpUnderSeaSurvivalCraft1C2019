using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;

namespace LosTiburones.Model
{
    //Inferfaz que deberan usar los peces para moverse
    interface MovimientoPez
    {
        void mover(TgcMesh pez, float ElapsedTime);
    }
}
