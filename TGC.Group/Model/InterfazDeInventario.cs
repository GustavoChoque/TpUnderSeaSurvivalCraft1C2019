using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Group.Model.Sprites;
using System.Windows.Forms;
using TGC.Core.Input;
using System.Drawing;
using TGC.Core.Text;
using TGC.Group.Model;
using TGC.Group.Model.Menu;
using TGC.Group.Model.Camara;

namespace LosTiburones.Model
{
    public class InterfazDeInventario
    {
        private Personaje personaje;
        private bool activo;
        private GameModel GModel;
        private bool recienActivo;
        private DXGui gui = new DXGui();
        private Boolean agregueItem = false;

        public void Init(GameModel gmodel, Personaje personaje)
        {
            this.GModel = gmodel;
            this.personaje = personaje;

            gui.Create(GModel.MediaDir);
            gui.InitDialog(false, false);

            activo = false;
        }

        public void Update()
        {
            if (activo && recienActivo)
            {

                var camaraInterna = (TgcFpsCamera)GModel.Camara;
                camaraInterna.LockCam = false;
                recienActivo = false;

            }
            else if (!activo && recienActivo)
            {
                var camaraInterna = (TgcFpsCamera)GModel.Camara;
                camaraInterna.LockCam = true;
                recienActivo = false;
            }
        }

        public void Render()
        {

            if (activo)
            {

                gui_render();

            }

        }

        public void Dispose()
        {

        }


        public void gui_render()
        {
            GuiMessage msg = gui.Update(GModel.ElapsedTime, GModel.Input);
            switch (msg.message)
            {
                case MessageType.WM_COMMAND:

                    switch (msg.id)
                    {
                        case 1:
                            incrementarOxigeno();
                            break;
                        case 2:
                            incrementarSalud();
                            break;
                    }


                    break;

            }

            gui.Render();


        }

        private void incrementarSalud()
        {
            throw new NotImplementedException();
        }

        private void incrementarOxigeno()
        {
            throw new NotImplementedException();
        }

        public void activar()
        {
            activo = !activo;
            recienActivo = true;

            if (activo)
            {
                int W = D3DDevice.Instance.Width;
                int H = D3DDevice.Instance.Height;

                int dy = H - 50;
                int dy2 = dy;
                int dx = W / 2;

                int posEnX = (W / 2) - (dx / 2);
                int posEnY = (H / 2) - (dy / 2);
                int x0 = posEnX + 150;
                int y0 = posEnY + 50;
                int x1 = x0;
                int y1 = y0;

                GUIItem frame = gui.InsertIFrame("", posEnX, posEnY, dx, dy, Color.FromArgb(140, 240, 140));
                frame.c_font = Color.FromArgb(0, 0, 0);
                frame.scrolleable = true;

                personaje.Inventario.Objetos.ForEach(elemento =>
                {
                    GUIItem item = new GUIItem();

                    switch (elemento.Nombre)
                    {
                        case "TanqueOxigeno":
                            item = gui.InsertImage("OxygenReducida.png", x1, y1 + 30, GModel.MediaDir);
                            gui.InsertItem("+100 Oxigeno" + "(x" + elemento.Cantidad + ")", x1 += 50, y1 + 20);
                            gui.InsertButton(1, "Usar", x1 += 300, y1, 120, 60);
                            agregueItem = true;
                            break;
                        case "Botiquin":
                            item = gui.InsertImage("botiquinReducido.png", x1, y1 + 30, GModel.MediaDir);
                            gui.InsertItem("+100 Salud" + "(x" + elemento.Cantidad + ")", x1 += 50, y1 + 20);
                            gui.InsertButton(1, "Usar", x1 += 300, y1, 120, 60);
                            agregueItem = true;
                            break;
                    }

                    if (agregueItem)
                    {
                        x1 = x0;
                        y1 = y1 + item.image_height + 20;
                        agregueItem = false;
                    }

                });
            }
            else
            {
                gui.Reset();
            }
        }
    }
}
