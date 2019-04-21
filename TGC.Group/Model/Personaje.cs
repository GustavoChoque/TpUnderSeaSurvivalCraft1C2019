using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model
{
    //JUGADOR COMIENZA VIVO, CON ENERGIA Y OXIGENO AL 100
    //MUERE CUANDO VIDA U OXIGENO SON MENORES A 1
    //PUEDE MORIR O REVIVIR, SUFRIR DANIO, PERDER OXIGENO, ASI COMO RECUPERAR ENERGIA Y OXIGENO
    class Personaje
    {
        //private List<Items> inventario;
        private int health;
        private int oxygen;
        private Boolean vivo = true;
        
        public Personaje(int health, int oxygen)
        {
            this.health = health;
            this.oxygen = oxygen;
        }

        public void sufriDanio(int danio)
        {
            this.health = this.health - danio;
            if (this.Health < 1) this.morite();
        }

        public void morite()
        {
            this.vivo = false;
        }

        public void revivi()
        {
            this.vivo = true;
        }

        public void recuperaVida(int vidaRecuperada)
        {
            if ((this.health + vidaRecuperada) > 100)
            {
                this.health = 100;
            }
            else
            {
                this.health = this.health + vidaRecuperada;
            }
        }

        public void perdeOxigeno(int oxigenoPerdido)
        {
            this.oxygen = this.oxygen - oxigenoPerdido;
            if (this.Oxygen < 1) this.morite();
        }

        public void recuperaOxigeno(int oxigenoRecuperado)
        {
            if ((this.oxygen + oxigenoRecuperado) > 100)
            {
                this.oxygen = 100;
            }
            else
            {
                this.oxygen = this.health + oxigenoRecuperado;
            }
        }

        public float Health { get => health; }
        public float Oxygen { get => oxygen; }
    }
}
