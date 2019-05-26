using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TGC.Core.Direct3D;

namespace TGC.Group.Model.Menu
{
    public class MenuPrincipal
    {
        GameModel GModel;
        private DXGui gui = new DXGui();
        public bool msg_box_app_exit = false;
        public bool msg_box_nueva_mision = false;
        public const int IDOK = 0;

        public const int IDCANCEL = 1;


        public const int ID_NUEVA_PARTIDA = 102;
        public const int ID_CONTROLES = 103;
        public const int ID_APP_EXIT = 105;



        public void Init(GameModel gameModel) {
            GModel = gameModel;

            // levanto el GUI
            gui.Create(GModel.MediaDir);

            // menu principal
            gui.InitDialog(false,false);
            int W = D3DDevice.Instance.Width;
            int H = D3DDevice.Instance.Height;
            int x0 = W - 200;
            int y0 = H-300;
            int dy = 80;
            int dy2 = dy;
            int dx = 200;

            gui.InsertMenuItem(ID_NUEVA_PARTIDA, "  Jugar", "play.png",x0, y0, GModel.MediaDir, dx, dy);
            gui.InsertMenuItem(ID_CONTROLES, "  Controles", "navegar.png", x0, y0 += dy2, GModel.MediaDir, dx, dy);
            gui.InsertMenuItem(ID_APP_EXIT, "  Sair", "salir.png", x0, y0+= dy2, GModel.MediaDir, dx, dy);
            //gui.InsertButton(123,"Prueba", W - 500, y0 += dy2, dx, dy);
        }
        public void Render() {
            gui_render();
        }
        public void Dispose() {
            gui.Dispose();

        }

        public void gui_render() {
            GuiMessage msg = gui.Update(GModel.ElapsedTime,GModel.Input);
            // proceso el msg
            switch (msg.message)
            {
                case MessageType.WM_COMMAND:
                    switch (msg.id)
                    {
                        case IDOK:
                            if (msg_box_app_exit) {

                                Application.Exit();
                            }
                            if (msg_box_nueva_mision) {
                                GModel.partidaActiva = true;
                            }
                            break;
                        case IDCANCEL:
                            gui.EndDialog();
                            break;
                        case ID_NUEVA_PARTIDA:
                            gui.MessageBox("Nueva Partida", "TGC Gui Demo");
                            msg_box_nueva_mision = true;
                            msg_box_app_exit = false;
                            break;
                        case ID_APP_EXIT:
                            gui.MessageBox("Desea Salir?", "TGC Gui Demo");
                            msg_box_app_exit = true;
                            msg_box_nueva_mision = false;
                            break;
                    }
                    break;
                default:
                    break;
            }
            gui.Render();
        }

    }
}
