using BulletSharp;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BoundingVolumes;
using TGC.Core.BulletPhysics;
using TGC.Core.Collision;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Sound;
using TGC.Core.Terrain;
using TGC.Core.Textures;
using TGC.Group.Model;
using TGC.Group.Model.Camara;
using TGC.Group.Model.Sprites;
using TGC.Core.Shaders;
using Microsoft.DirectX.Direct3D;
using Effect = Microsoft.DirectX.Direct3D.Effect;
using TGC.Group.Model.Quadtree;
using TGC.Group.Model.Optimizacion;
using TGC.Core.Text;
using TGC.Core.Input;
using LosTiburones.Model.CraftingInventario;
using LosTiburones.Model.Animales;

namespace LosTiburones.Model
{
    public class Escenario
    {   //DECLARO VARIABLES 'GLOBALES'
        GameModel GModel;
        
        private TgcPlane piso, agua;
        private TgcMesh coralBrain, coral, meshTiburon, fish, pillarCoral, seaShell, spiralWireCoral, treeCoral, yellowFish;
        //private TgcMesh arbusto, arbusto2, pasto, planta, planta2, planta3, roca;

        private TgcMesh workbench;

        private CustomSprite spriteArpon = new CustomSprite();
        private CustomSprite spriteRedPesca = new CustomSprite();
        private CustomBitmap bitmapArpon;
        private CustomBitmap bitmapRedPesca;

        private TgcScene barco;
        private TgcSkyBox skybox;
        private ObjetoRecolectable objetoAMostrar;
        private List<ObjetoRecolectable> objetosRecolectables = new List<ObjetoRecolectable>();

        private TgcSimpleTerrain terreno;
        private float currentScaleXZ;
        private float currentScaleY;

        private TgcMp3Player musica = new TgcMp3Player();
        private TgcStaticSound sonidoTiburonCerca = new TgcStaticSound();

        private List<TgcMesh> objetosEstaticosEnArray = new List<TgcMesh>();
        private List<Pez> pecesAmarillos = new List<Pez>();
        private List<Pez> pecesAzules = new List<Pez>();
        private List<Pez> pecesAmarillosCercaPersonaje = new List<Pez>();
        private List<Pez> pecesAzulesCercaPersonaje = new List<Pez>();

        private Tiburon tiburon;

        //Constantes para velocidades de movimiento
        private const float ROTATION_SPEED = 50f;
        private const float MOVEMENT_SPEED = 50f;

        private CustomBitmap bitmapCursor;
        private CustomBitmap bitmapCorazon, bitmapTanqueOxigeno, bitmapBarra;
        private CustomBitmap bitmapRellenoVida, bitmapRellenoOxigeno;
        private Drawer2D spriteDrawer = new Drawer2D();

        private List<CustomSprite> sprites = new List<CustomSprite>();

        private CustomSprite spriteCursor = new CustomSprite();
        private CustomSprite spriteCorazon = new CustomSprite();
        private CustomSprite spriteBarraVida = new CustomSprite();
        private CustomSprite spriteBarraOxigeno = new CustomSprite();
        private CustomSprite spriteRellenoVida = new CustomSprite();
        private CustomSprite spriteRellenoOxigeno = new CustomSprite();
        private CustomSprite spriteTanqueOxigeno = new CustomSprite();
        private TgcText2D textoVida;
        private TgcText2D textoOxigeno;

        private int ScreenWidth = D3DDevice.Instance.Device.Viewport.Width;
        private int ScreenHeight = D3DDevice.Instance.Device.Viewport.Height;

        //-----------Fisica-------------------
        private RigidBody rigidCamera;

        DiscreteDynamicsWorld dynamicsWorld;
        CollisionDispatcher dispatcher;
        DefaultCollisionConfiguration collisionConfiguration;
        SequentialImpulseConstraintSolver constraintSolver;
        BroadphaseInterface broadphaseInterface;

        TGCVector3 posicionInicial = new TGCVector3(600, 10, -250);
        TgcFpsCamera camaraInterna;
        
        //-------------------

        //-------Shaders----------
        private Effect efectoSuperficieAgua, efectoMetalico, efectoNiebla;
        private float time;
        //---------------------

        //--------Para las burbujas------
        private TGCSphere burbuja;
        private float movimientoBurbuja = 0;
        private float timerBurbujas = 60f;
        //----------------------
        private TgcScene objetosDelTerreno;
        //private Quadtree quadtree;
        private Octree octree;
        private const int DESPLAZAMIENTO_EN_Y = 5260;

        //////////////////////////////
        private TgcText2D textoPesque;
        private bool renderizoTextoPesque = false;
        private float acumuloTiempo = 0;

        //////////////////////////////
        private List<Arpon> arpones = new List<Arpon>();
        private TgcMesh arponMesh;
        private Int64 arponSeq = 0;

        //////////////////////////////
        private Boolean recienMeSumergi = false;

        //////////////////////////////
        //escalado para pantallas de otros sizes
        //pantalla que use originalmente, ancho 1366, alto 768
        private float anchoOriginal = 1366f;
        private float altoOriginal = 768f;
        private float factorCorreccionAncho;
        private float factorCorreccionAlto;

        /////////////////////////////
        private int sizeMapa = 80000;
        private int fondoMapa = -5230;
        private int alturaTecho = 100;
        private TgcText2D mensajeErrorArponRed;
        private Boolean renderizoErrorArponRed = false;

        public void Init(GameModel gmodel)
        {
            this.GModel = gmodel;

            factorCorreccionAncho = D3DDevice.Instance.Device.Viewport.Width / anchoOriginal;
            factorCorreccionAlto = D3DDevice.Instance.Device.Viewport.Height / altoOriginal;

            //----------------Fisica------------

            //----configuracion del Mundo fisico--------------
            collisionConfiguration = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfiguration);
            GImpactCollisionAlgorithm.RegisterAlgorithm(dispatcher);
            constraintSolver = new SequentialImpulseConstraintSolver();
            broadphaseInterface = new DbvtBroadphase();
            dynamicsWorld = new DiscreteDynamicsWorld(dispatcher, broadphaseInterface, constraintSolver, collisionConfiguration);
            //dynamicsWorld.Gravity=new TGCVector3(0,-20f,0).ToBulletVector3();
            //dynamicsWorld.Gravity=new TGCVector3(0,0,0).ToBulletVector3();
            
            //-------------creacion rigidbodies-------------------
            var ballShape = new SphereShape(50);
            var ballTransform = TGCMatrix.Identity;
            ballTransform.Origin = posicionInicial;//GModel.Camara.Position;//new TGCVector3(100, 50, 0);
            var ballMotionState = new DefaultMotionState(ballTransform.ToBsMatrix);
            var ballInertia = ballShape.CalculateLocalInertia(1f);
            var ballInfo = new RigidBodyConstructionInfo(1, ballMotionState, ballShape, ballInertia);

            rigidCamera = new RigidBody(ballInfo);
            rigidCamera.SetDamping(0.9f, 0.9f);
            //esto es para que no le afecte la gravedad al inicio de la partida
            rigidCamera.ActivationState = ActivationState.IslandSleeping;
            dynamicsWorld.AddRigidBody(rigidCamera);

            camaraInterna = new TgcFpsCamera(posicionInicial, GModel.Input);

            //Device de DirectX para crear primitivas.
            var d3dDevice = D3DDevice.Instance.Device;
            //-----------init burbuja----------
            var textureBurbuja = TgcTexture.createTexture(GModel.MediaDir + "Texturas\\burbuja7.png");
            burbuja = new TGCSphere(50, textureBurbuja, new TGCVector3(0, 0, 0));
            burbuja.AlphaBlendEnable = true;
            burbuja.updateValues();
            burbuja.Transform = TGCMatrix.Scaling(20, 20, 20) * TGCMatrix.Translation(100, -50, 100);
            //------------------------------
            this.cargoPisos();

            this.cargoSkybox();

            this.cargoHeightmap();

            this.cargoMusica();

            this.cargoMeshes();

            this.generoObjetosEstaticosSueltos();

            this.generoObjetosEstaticosEnArray();

            this.generoPecesAmarillos();

            this.generoPecesAzules();

            this.generoMetales();

            this.generoHUD();

            this.configuroRedYArpon();


            //-----------Fisica---------
            //para el heighmap

            var heighmapRigid = BulletRigidBodyFactory.Instance.CreateSurfaceFromHeighMap(terreno.getData());
            dynamicsWorld.AddRigidBody(heighmapRigid);

            //----------------------

            //-----------cargar shaders----
            efectoSuperficieAgua = TGCShaders.Instance.LoadEffect(GModel.ShadersDir + "SuperficieDeAgua.fx");
            agua.Effect = efectoSuperficieAgua;
            agua.Technique = "OleajeNormal";
            time = 0;

            efectoNiebla = TGCShaders.Instance.LoadEffect(GModel.ShadersDir + "TgcFogShader.fx");

            efectoMetalico = TGCShaders.Instance.TgcMeshPhongShader;

            //-----------

            workbench.Scale = new TGCVector3(0.2f, 0.2f, 0.2f);
            workbench.Position = new TGCVector3(150f, -60, -50f);
            workbench.RotateY(-FastMath.PI/2);
            
            objetosDelTerreno.Meshes.ForEach(o => {
                //o.AutoTransformEnable = false;
                o.Move(new TGCVector3(0, -DESPLAZAMIENTO_EN_Y, 0));
                //o.Transform = TGCMatrix.Translation(new TGCVector3(0, -5000, 0));
            });

            objetosDelTerreno.Meshes.ForEach(mesh => {
                var body = BulletRigidBodyFactory.Instance.CreateBall(50, 0, mesh.Position);
                
                dynamicsWorld.AddRigidBody(body);

            });
          /*  //--------creo Quadtree para la Optimizacion---------------
            quadtree = new Quadtree();
            
            objetosDelTerreno.BoundingBox.move(new TGCVector3(0, -5260, 0));
            quadtree.create(objetosDelTerreno.Meshes, objetosDelTerreno.BoundingBox);
            quadtree.createDebugQuadtreeMeshes();
            //----------------------------------
            */
            //--------creo Octree para la Optimizacion---------------
            octree = new Octree();

            objetosDelTerreno.BoundingBox.move(new TGCVector3(0, -DESPLAZAMIENTO_EN_Y, 0));
            octree.create(objetosDelTerreno.Meshes, objetosDelTerreno.BoundingBox);
            octree.createDebugOctreeMeshes();
            //----------------------------------




            //BULLET DEBUG: Debug para Bullet (Commentar estas lineas si NO se desea Debugear)
            //-------------Start Bullet Debug Config----------------
            //debugDrawer = new ADebugDrawer(DebugDrawModes.DrawWireframe);
            //dynamicsWorld.DebugDrawer = debugDrawer;
            //-------------End Bullet Debug Config------------------

            textoVida = new TgcText2D();
            textoVida.Align = TgcText2D.TextAlign.CENTER;
            var width = D3DDevice.Instance.Device.Viewport.Width;
            var height = D3DDevice.Instance.Device.Viewport.Height;
            textoVida.Position = new Point((width / 2) + (width / 8) + (width / 16), (height / 2) + (height / 4) + (height / 20) + (height / 8));
            textoVida.Size = new Size(100, 100);
            textoVida.Color = Color.White;

            textoOxigeno = new TgcText2D();
            textoOxigeno.Align = TgcText2D.TextAlign.CENTER;
            textoOxigeno.Position = new Point((width / 2) + (width / 8) + (width / 16), (height / 2) + (height / 4) + (height / 24));
            textoOxigeno.Size = new Size(100, 100);
            textoOxigeno.Color = Color.White;

            textoPesque = new TgcText2D();
            textoPesque.Text = "Pez capturado con exito!";
            textoPesque.Align = TgcText2D.TextAlign.CENTER;
            textoPesque.Position = new Point(width / 3, height / 2 + 20);
            textoPesque.Size = new Size(500, 500);
            textoPesque.Color = Color.LawnGreen;

            mensajeErrorArponRed = new TgcText2D();
            mensajeErrorArponRed.Text = "El arpon y la red se pueden usar solo bajo el agua!";
            mensajeErrorArponRed.Align = TgcText2D.TextAlign.CENTER;
            mensajeErrorArponRed.Position = new Point(width / 3, height / 2 + 20);
            mensajeErrorArponRed.Size = new Size(500, 500);
            mensajeErrorArponRed.Color = Color.Red;
        }

        private void configuroRedYArpon()
        {
            bitmapArpon = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "ArponPersonaje.png", D3DDevice.Instance.Device);
            spriteArpon.Bitmap = bitmapArpon;
            spriteArpon.Scaling = new TGCVector2(.5f * factorCorreccionAncho, .5f * factorCorreccionAlto);
            spriteArpon.Position = new TGCVector2(ScreenWidth / 3.5f, ScreenHeight / 2f + 25f);

            bitmapRedPesca = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "RedPersonaje.png", D3DDevice.Instance.Device);
            spriteRedPesca.Bitmap = bitmapRedPesca;
            spriteRedPesca.Scaling = new TGCVector2(.5f * factorCorreccionAncho, .5f * factorCorreccionAlto);
            spriteRedPesca.Position = new TGCVector2(ScreenWidth / 2.5f, ScreenHeight / 2f + 50f);
        }

        public void Update()
        {
            var Input = this.GModel.Input;

            factorCorreccionAncho = D3DDevice.Instance.Device.Viewport.Width / anchoOriginal;
            factorCorreccionAlto = D3DDevice.Instance.Device.Viewport.Height / altoOriginal;

            //-------Fisica----------
            dynamicsWorld.StepSimulation(1 / 60f, 100);

            ContactoTiburonCallback tiburonCallback = new ContactoTiburonCallback(this.tiburon, this.GModel.Personaje);
            dynamicsWorld.ContactPairTest(tiburon.CuerpoRigido, RigidCamera, tiburonCallback);

            arpones.ForEach(arpon => {

                dynamicsWorld.ContactPairTest(tiburon.CuerpoRigido, arpon.RigidBody, new ContactoTiburonArponCallback(arpon, this.Tiburon));

            });

            //dynamicsWorld.ContactPairTest(tiburon.CuerpoRigido, )

            //----------------
            var director = GModel.Camara.LookAt - GModel.Camara.Position; //new TGCVector3(1,0,0);
            director.Normalize();
            var strength = 50f;
            //if (GModel.Input.keyDown(Key.UpArrow))

            //SI ME MUERO, o tengo los menues de inventario y crafting NO ME MUEVO
            if (GModel.Personaje.Vivo && !GModel.InterfazCrafting.Activo && !GModel.InterfazInventario.Activo)
            {
                if (GModel.Input.keyDown(Key.W))
                {

                    //muevo el rigidsegun a donde miro

                    rigidCamera.ActivationState = ActivationState.ActiveTag;

                    rigidCamera.ApplyCentralImpulse(strength * director.ToBulletVector3());

                }
                if (GModel.Input.keyUp(Key.W))
                {
                    //para detener el rigid
                    //op1
                    rigidCamera.ActivationState = ActivationState.IslandSleeping;
                    //op2
                    //rigidCamera.ActivationState = ActivationState.DisableSimulation;
                }

                if (GModel.Input.keyDown(Key.A))
                {

                    //muevo el rigidsegun a donde miro

                    rigidCamera.ActivationState = ActivationState.ActiveTag;

                    var miIzquierda = new TGCVector3();
                    miIzquierda.X = director.X * FastMath.Cos(FastMath.PI_HALF) - director.Z * FastMath.Sin(FastMath.PI_HALF);
                    miIzquierda.Z = director.X * FastMath.Sin(FastMath.PI_HALF) + director.Z * FastMath.Cos(FastMath.PI_HALF);

                    rigidCamera.ApplyCentralImpulse(strength * miIzquierda.ToBulletVector3());

                }
                if (GModel.Input.keyUp(Key.A))
                {
                    //para detener el rigid
                    //op1
                    rigidCamera.ActivationState = ActivationState.IslandSleeping;
                    //op2
                    //rigidCamera.ActivationState = ActivationState.DisableSimulation;
                }

                if (GModel.Input.keyDown(Key.D))
                {

                    //muevo el rigidsegun a donde miro

                    rigidCamera.ActivationState = ActivationState.ActiveTag;

                    var miDerecha = new TGCVector3();
                    miDerecha.X = director.X * FastMath.Cos(FastMath.PI + FastMath.PI_HALF) - director.Z * FastMath.Sin(FastMath.PI + FastMath.PI_HALF);
                    miDerecha.Z = director.X * FastMath.Sin(FastMath.PI + FastMath.PI_HALF) + director.Z * FastMath.Cos(FastMath.PI + FastMath.PI_HALF);

                    rigidCamera.ApplyCentralImpulse(strength * miDerecha.ToBulletVector3());

                }
                if (GModel.Input.keyUp(Key.D))
                {
                    //para detener el rigid
                    //op1
                    rigidCamera.ActivationState = ActivationState.IslandSleeping;
                    //op2
                    //rigidCamera.ActivationState = ActivationState.DisableSimulation;
                }

                //if (GModel.Input.keyDown(Key.DownArrow))
                if (GModel.Input.keyDown(Key.S))
                {

                    //muevo el rigidsegun a donde miro
                    rigidCamera.ActivationState = ActivationState.ActiveTag;
                    rigidCamera.ApplyCentralImpulse(-strength * director.ToBulletVector3());

                }
                if (GModel.Input.keyUp(Key.S))
                {
                    //para detener el rigid
                    //op1
                    rigidCamera.ActivationState = ActivationState.IslandSleeping;
                    //op2
                    //rigidCamera.ActivationState = ActivationState.DisableSimulation;
                }
                //-----------------------

                //God Mode toggle on/off
                if (Input.keyPressed(Key.G))
                {
                    GModel.Personaje.ModoDios = !GModel.Personaje.ModoDios;
                }

                /////////PESCO SOLO BAJO EL AGUA
                if (GModel.Personaje.UsoRedPesca && bajoElAgua(GModel.Personaje))
                {
                    pecesAmarillosCercaPersonaje = pecesAmarillos.FindAll(pez => pez.estoyCercaDePersonaje(GModel.Personaje));
                    pecesAzulesCercaPersonaje = pecesAzules.FindAll(pez => pez.estoyCercaDePersonaje(GModel.Personaje));

                    if (pecesAmarillosCercaPersonaje.Count > 0 && (GModel.Input.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_LEFT)))
                    {
                        renderizoTextoPesque = true;
                        acumuloTiempo = 0;
                        pecesAmarillosCercaPersonaje.ForEach(pez => {
                            pez.disable();
                            pecesAmarillos.Remove(pez);
                            pez.Dispose();
                            GModel.Personaje.Inventario.agregaObjeto(new ObjetoInventarioPezAmarillo());
                        });
                    }

                    if (pecesAzulesCercaPersonaje.Count > 0 && (GModel.Input.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_LEFT)))
                    {

                        renderizoTextoPesque = true;
                        acumuloTiempo = 0;
                        pecesAzulesCercaPersonaje.ForEach(pez => {
                            pez.disable();
                            pecesAzules.Remove(pez);
                            pez.Dispose();
                            GModel.Personaje.Inventario.agregaObjeto(new ObjetoInventarioPezAzul());
                        });
                    }
                }

                /////////ATACO SOLO BAJO EL AGUA
                if (GModel.Personaje.UsoArma && bajoElAgua(GModel.Personaje))
                {
                    if (GModel.Input.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_LEFT))
                    {
                        var dir = new TGCVector3(GModel.Camara.LookAt.X - GModel.Camara.Position.X, GModel.Camara.LookAt.Y - GModel.Camara.Position.Y, GModel.Camara.LookAt.Z - GModel.Camara.Position.Z).ToBulletVector3();
                        dir.Normalize();

                        arponSeq = arponSeq + 1;
                        var posicionArpon = new TGCVector3(dir.X * 10 + GModel.Camara.Position.X, dir.Y * 10 + GModel.Camara.Position.Y, dir.Z * 10 + GModel.Camara.Position.Z);
                        var meshArpon = arponMesh.createMeshInstance(arponMesh.Name + "_" + arponSeq);
                        var arponRigidBody = BulletRigidBodyFactory.Instance.CreateCapsule(10f, 500f, posicionArpon, 1000f, false);
                        
                        arponRigidBody.LinearVelocity = dir * 5000f;
                        arponRigidBody.LinearFactor = TGCVector3.One.ToBulletVector3();

                        var arponPosta = new Arpon(meshArpon, arponRigidBody, posicionArpon, GModel, new TGCVector3(dir.X, dir.Y, dir.Z));
                        arpones.Add(arponPosta);
                        dynamicsWorld.AddRigidBody(arponPosta.RigidBody);
                    }
                }

                //ERROR ARPON Y RED
                if ((GModel.Personaje.UsoRedPesca || GModel.Personaje.UsoArma) && !bajoElAgua(GModel.Personaje) && GModel.Input.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_LEFT))
                {
                    renderizoErrorArponRed = true;
                    acumuloTiempo = 0;
                }

                //Muestro los mensajes por dos segundos
                acumuloTiempo = acumuloTiempo + GModel.ElapsedTime;
                if (acumuloTiempo > 2 && renderizoTextoPesque)
                {
                    renderizoTextoPesque = false;
                }

                if (acumuloTiempo > 2 && renderizoErrorArponRed)
                {
                    renderizoErrorArponRed = false;
                }
            }

            //Music toggle on/off
            if (Input.keyPressed(Key.P))
            {
                switch (musica.getStatus()) {
                    case TgcMp3Player.States.Playing:
                        musica.pause();
                        break;
                    case TgcMp3Player.States.Paused:
                        musica.resume();
                        break;
                    case TgcMp3Player.States.Open:
                        musica.play(true);
                        break;
                    case TgcMp3Player.States.Stopped:
                        musica.play(true);
                        break;
                }
            }

            //-----------movimientos-------------
            tiburon.moverse(this);

            //-----------
            //Muevo los peces amarillos

            //pezCircular.Update(GModel.ElapsedTime);

            pecesAmarillos.ForEach(pez =>
            {
                pez.Position += new TGCVector3(5f * pez.MoveSpeed * GModel.ElapsedTime * pez.CurrentMoveDir, 0, 0);

                if (GModel.GetRandom.Next(0, 2000) > GModel.GetRandom.Next(1998, 1999) //cada tanto
                    || this.fueraDelMapa(pez)) //si toco los bordes
                {
                    //reboto
                    pez.CurrentMoveDir *= -1;
                    pez.Rotation += new TGCVector3(FastMath.PI, 0, 0);
                }

                pez.Transform = TGCMatrix.Scaling(pez.Scale) * TGCMatrix.RotationYawPitchRoll(pez.Rotation.X, pez.Rotation.Y, pez.Rotation.Z) * TGCMatrix.Translation(pez.Position);
            }
            );

            //-----------
            //Muevo los peces azules

            pecesAzules.ForEach(pez =>
            {
                pez.Position += new TGCVector3(0, 0, 2f * pez.MoveSpeed * GModel.ElapsedTime * pez.CurrentMoveDir);

                if (GModel.GetRandom.Next(0, 2000) > GModel.GetRandom.Next(1998, 1999) //cada tanto
                    || this.fueraDelMapa(pez)) //si toco los bordes
                {
                    //reboto
                    pez.CurrentMoveDir *= -1;
                    pez.Rotation += new TGCVector3(FastMath.PI, 0, 0);
                }

                pez.Transform = TGCMatrix.Scaling(pez.Scale) * TGCMatrix.RotationYawPitchRoll(pez.Rotation.X, pez.Rotation.Y, pez.Rotation.Z) * TGCMatrix.Translation(pez.Position);
            }
            );

            //-----Skybox
            skybox.Center = GModel.Personaje.Position; //GModel.Camara.Position;
            //----------

            //REFRESCO EL TAMAÑO DE LA PANTALLA
            ScreenWidth = D3DDevice.Instance.Device.Viewport.Width;
            ScreenHeight = D3DDevice.Instance.Device.Viewport.Height;

            this.actualizoValoresSaludOxigeno(GModel.Personaje);

            textoOxigeno.Text = ((int)Math.Round(GModel.Personaje.Oxygen)).ToString() + "/" + ((int)Math.Round(GModel.Personaje.MaxOxygen)).ToString();
            textoVida.Text = ((int)Math.Round(GModel.Personaje.Health)).ToString() + "/" + ((int)Math.Round(GModel.Personaje.MaxHealth)).ToString();

            arpones.ForEach(arpon => {

                arpon.Update();

            });

            if (bajoElAgua(GModel.Personaje))
            {
                if (recienMeSumergi)
                {
                    recienMeSumergi = false;
                    musica.stop();
                    musica.closeFile();
                    musica.FileName = GModel.MediaDir + "Music\\UnderWater.mp3";
                    musica.play(true);
                }
            }
            else
            {
                if (!recienMeSumergi)
                {
                    recienMeSumergi = true;
                    musica.stop();
                    musica.closeFile();
                    musica.FileName = GModel.MediaDir + "Music\\SeaShore.mp3";
                    musica.play(true);
                }
            }
            movimientoBurbuja += GModel.ElapsedTime * 100;
        }

        public void Render()
        {
            ///DEBUGGING DE LANZAMIENTO DE ARPON
            var dir = new TGCVector3(GModel.Camara.LookAt.X - GModel.Camara.Position.X, GModel.Camara.LookAt.Y - GModel.Camara.Position.Y, GModel.Camara.LookAt.Z - GModel.Camara.Position.Z);
            dir.Normalize();

            //VECTOR EN Z
            var dirMeshInicial = new TGCVector3(0, 0, 1);

            //CALCULO ANGULO
            var numerador = TGCVector3.Dot(dir, dirMeshInicial);
            var denominador = TGCVector3.Length(dir) * TGCVector3.Length(dirMeshInicial);

            float theta;
            if (dir.X > 0)
            {
                theta = FastMath.Acos(numerador / denominador);
            }
            else
            {
                theta = FastMath.Acos(-numerador / denominador) + FastMath.PI;
            }

            float gamma = FastMath.Acos(dir.Y);

            GModel.DrawText.drawText("Posicion Camara: " + TGCVector3.PrintVector3(GModel.Camara.Position), 0, 20, Color.OrangeRed);
            GModel.DrawText.drawText("LookAt: " + TGCVector3.PrintVector3(GModel.Camara.LookAt), 0, 30, Color.OrangeRed);
            GModel.DrawText.drawText("Direction (LookAt - Posicion): " + TGCVector3.PrintVector3(dir), 0, 40, Color.OrangeRed);
            GModel.DrawText.drawText("Theta entre Direction y Z: " + theta, 0, 50, Color.OrangeRed);
            GModel.DrawText.drawText("Gamma entre Direction y Z: " + gamma, 0, 60, Color.OrangeRed);

            //BULLET DEBUG: Le indico a bullet que Dibuje las lineas de debug.
            //ATENCION: COMENTAR ESTA LINEA SI NO SE DESEA DEBUGEAR BULLET
            //dynamicsWorld.DebugDrawObject(tiburon.CuerpoRigido.WorldTransform, tiburon.CuerpoRigido.CollisionShape, new TGCVector3(Color.Red.R, Color.Red.G, Color.Red.B).ToBulletVector3());

            //--------------------------------

            //----Fisica----------
            if (GModel.partidaActiva) { 
            camaraInterna.setPosicion(new TGCVector3(rigidCamera.CenterOfMassPosition));
            GModel.Camara = camaraInterna;
            }
            //----------------

            //----------Shaders----------
            time += GModel.ElapsedTime;
            efectoSuperficieAgua.SetValue("time", time);

            //-----------
            // Cargamos las variables de shader, color del fog.
            efectoNiebla.SetValue("ColorFog", Color.SeaGreen.ToArgb());
            efectoNiebla.SetValue("CameraPos", TGCVector3.Vector3ToFloat4Array(GModel.Camara.Position));
            efectoNiebla.SetValue("StartFogDistance", 2000);
            efectoNiebla.SetValue("EndFogDistance", 7000);
            efectoNiebla.SetValue("Density", 0.0025f);

            foreach (var mesh in skybox.Faces)
            {
                mesh.Effect = efectoNiebla;
                mesh.Technique = "RenderScene";
                //mesh.Render();
            }

            foreach (var mesh in objetosEstaticosEnArray)
            {
                mesh.Effect = efectoNiebla;
                mesh.Technique = "RenderScene";

            }

            foreach (var mesh in objetosDelTerreno.Meshes)
            {
                mesh.Effect = efectoNiebla;
                mesh.Technique = "RenderScene";

            }

            terreno.Effect = efectoNiebla;
            terreno.Technique = "RenderScene";

            piso.Effect = efectoNiebla;
            piso.Technique = "RenderScene";
            //----------
            var posLuz = new TGCVector3(-100, 500, 0);

            foreach (var mesh in barco.Meshes)
            {
                mesh.Effect = efectoMetalico;
                mesh.Technique = TGCShaders.Instance.GetTGCMeshTechnique(mesh.RenderType);
                mesh.Effect.SetValue("lightPosition", TGCVector3.Vector3ToFloat4Array(posLuz));
                mesh.Effect.SetValue("eyePosition", TGCVector3.Vector3ToFloat4Array(GModel.Camara.Position));
                mesh.Effect.SetValue("ambientColor", ColorValue.FromColor(Color.Orange));
                mesh.Effect.SetValue("diffuseColor", ColorValue.FromColor(Color.LightGoldenrodYellow));
                mesh.Effect.SetValue("specularColor", ColorValue.FromColor(Color.Gray));
                mesh.Effect.SetValue("specularExp", 30f);
            }

            //--------------
            if (GModel.Personaje.ModoDios)
            {
                GModel.DrawText.drawText("Modo Dios Activado", ScreenWidth - (ScreenWidth * 2) / 10, ScreenHeight - (ScreenHeight * 95) / 100 + 20, Color.Red);
            }
            //--------------


            if (GModel.Personaje.Vivo)
            {
                if (bajoElAgua(GModel.Personaje))
                {
                    GModel.DrawText.drawText("Sufriendo daño por falta de oxigeno", ScreenWidth - (ScreenWidth * 2)/10, ScreenHeight - (ScreenHeight * 95)/100, Color.Red);
                }

                if ((this.fueraDelMapa(GModel.Personaje)))
                {
                    GModel.DrawText.drawText("Sufriendo daño por estar fuera del mapa", ScreenWidth - (ScreenWidth * 2) / 10, ScreenHeight - (ScreenHeight * 95) / 100 + 10, Color.Red);
                }
            }
            else
            {
                GModel.DrawText.drawText("Te moriste", ScreenWidth / 2 - 25, ScreenHeight / 2 + 20, Color.Red);

                camaraInterna.LockCam = false;
            }

            if (objetoAMostrar != null)
            {
                GModel.DrawText.drawText("Recolectar: " + objetoAMostrar.Nombre, Convert.ToInt32(Math.Round((double)ScreenWidth / 2.2)), Convert.ToInt32(Math.Round((double)ScreenHeight / 2.2)), Color.Red);
            }

            //--------skybox---------
            skybox.Render();
            //---------------
            //----heighmap
            terreno.Render();

            //------------------------------------
            agua.Render();
            piso.Render();
            coral.Render();
            tiburon.Render();
            coralBrain.Render();
            barco.RenderAll();
            fish.Render();
            pillarCoral.Render();
            seaShell.Render();
            spiralWireCoral.Render();
            treeCoral.Render();
            yellowFish.Render();
            /*arbusto.Render();
            arbusto2.Render();
            pasto.Render();
            planta.Render();
            planta2.Render();
            planta3.Render();
            roca.Render();*/
            
            ///------para rederear las burbujas-----
            objetosDelTerreno.Meshes.ForEach(p => {
                if (p.Position.Y + movimientoBurbuja + burbuja.Radius < 0)
                {
                    burbuja.Transform = TGCMatrix.Scaling(20, 20, 20) * TGCMatrix.Translation(p.Position.X, p.Position.Y + movimientoBurbuja, p.Position.Z);

                    burbuja.Render();
                }

            });
            //control de creacion burbujas, aprox cada 60 seg
            timerBurbujas -= GModel.ElapsedTime;
            if (timerBurbujas < 0)
            {
                movimientoBurbuja = 0;
                timerBurbujas = 60;
            }
            //--------------------
            
            pecesAmarillos.ForEach(obj => obj.Render());
            //pezCircular.Render();
            pecesAzules.ForEach(obj => obj.Render());

            objetosRecolectables.ForEach(obj => obj.Render());

            if (GModel.partidaActiva) { 
            //SPRITES
            spriteDrawer.BeginDrawSprite();
            sprites.ForEach(sprite => spriteDrawer.DrawSprite(sprite));
            spriteDrawer.EndDrawSprite();
            }

            workbench.Render();

            objetosDelTerreno.RenderAll();

            //quadtree.render(GModel.Frustum, true);

            octree.render(GModel.Frustum, false);

            //objetosEstaticosEnArray.ForEach(obj => obj.Render());

            //----------Frustum Culling-------------
            //por alguna razon no funciona bien al renderizar los objetos,
            objetosEstaticosEnArray.ForEach(mesh => {
                if (mesh.Enabled)
                {
                    var r = TgcCollisionUtils.classifyFrustumAABB(GModel.Frustum, mesh.BoundingBox);
                    if (r != TgcCollisionUtils.FrustumResult.OUTSIDE)
                    {
                        mesh.Render();

                    }
                }

            });
            //-------------------------

            textoOxigeno.render();
            textoVida.render();

            if (GModel.Personaje.UsoArma)
            {
                spriteDrawer.BeginDrawSprite();
                spriteDrawer.DrawSprite(spriteArpon);
                spriteDrawer.EndDrawSprite();
            }

            if (GModel.Personaje.UsoRedPesca)
            {
                spriteDrawer.BeginDrawSprite();
                spriteDrawer.DrawSprite(spriteRedPesca);
                spriteDrawer.EndDrawSprite();
            }

            if (renderizoTextoPesque)
            {
                textoPesque.render();
            }

            ///////////////////////////ARPONES
            arpones.ForEach(arpon => {

                arpon.Render();

            });

            if (renderizoErrorArponRed)
            {
                mensajeErrorArponRed.render();
            }
        }

        public void Dispose()
        {

            //---------fisica------
            dynamicsWorld.Dispose();
            dispatcher.Dispose();
            collisionConfiguration.Dispose();
            constraintSolver.Dispose();
            broadphaseInterface.Dispose();
            rigidCamera.Dispose();

            //------------
            //----------Shaders-------
            efectoSuperficieAgua.Dispose();

            //-------------

            //------------------------
            skybox.Dispose();
            terreno.Dispose();
            agua.Dispose();
            piso.Dispose();
            coral.Dispose();
            tiburon.Dispose();
            coralBrain.Dispose();
            barco.DisposeAll();
            fish.Dispose();
            pillarCoral.Dispose();
            seaShell.Dispose();
            spiralWireCoral.Dispose();
            treeCoral.Dispose();
            yellowFish.Dispose();
            /*
             * arbusto.Dispose();
            arbusto2.Dispose();
            pasto.Dispose();
            planta.Dispose();
            planta2.Dispose();
            planta3.Dispose();
            roca.Dispose();
            */
            burbuja.Dispose();
            objetosEstaticosEnArray.ForEach(obj => obj.Dispose());

            pecesAmarillos.ForEach(obj => obj.Dispose());
            //pezCircular.Dispose();

            pecesAzules.ForEach(obj => obj.Dispose());

            objetosRecolectables.ForEach(obj => obj.Dispose());

            //BULLET DEBUG: Libero el DebugDrawer
            //debugDrawer.Dispose();


            workbench.Dispose();
            objetosDelTerreno.DisposeAll();

            textoVida.Dispose();
            textoOxigeno.Dispose();

            spriteArpon.Dispose();
            spriteRedPesca.Dispose();

            arpones.ForEach(obj => obj.Dispose());
        }


        private Boolean fueraDelMapa(Personaje personaje)
        {
            return personaje.Position.X > sizeMapa / 2 || personaje.Position.X < -sizeMapa / 2 || personaje.Position.Z > sizeMapa / 2 || personaje.Position.Z < -sizeMapa / 2;
        }

        private Boolean fueraDelMapa(Pez pez)
        {
            return pez.Position.X > sizeMapa / 2 || pez.Position.X < -sizeMapa / 2 || pez.Position.Z > sizeMapa / 2 || pez.Position.Z < -sizeMapa / 2;
        }

        private Boolean dentroDelMapa(Personaje personaje)
        {
            return personaje.Position.X <= sizeMapa / 2 && personaje.Position.X >= -sizeMapa / 2 && personaje.Position.Z <= sizeMapa / 2 && personaje.Position.Z >= -sizeMapa / 2;
        }

        private Boolean bajoElAgua(Personaje personaje)
        {
            return personaje.Position.Y < 0;
        }

        private Boolean sobreElAgua(Personaje personaje)
        {
            return personaje.Position.Y >= 0;
        }

        private void actualizoValoresSaludOxigeno(Personaje personaje)
        {
            //ACTUALIZO LOS VALORES DE SALUD Y OXIGENO
            if (this.fueraDelMapa(personaje))
            {
                personaje.sufriDanio(7.5f * GModel.ElapsedTime) ;
            }

            if (this.dentroDelMapa(personaje))
            {
                personaje.recuperaVida(3f * GModel.ElapsedTime);
            }

            if (this.bajoElAgua(personaje))
            {
                personaje.perdeOxigeno(7.5f * GModel.ElapsedTime);
            }

            if (this.sobreElAgua(personaje))
            {
                personaje.recuperaOxigeno(3f * GModel.ElapsedTime);
            }

            //ACTUALIZO LOS SPRITES DE ENERGIA Y OXIGENO
            if (personaje.Vivo)
            {
                switch (personaje.Health / personaje.MaxHealth)
                {
                    case float n when (n <= 0):
                        spriteRellenoVida.Scaling = new TGCVector2(.75f * 0.0f * factorCorreccionAncho, 1f * factorCorreccionAlto);
                        break;
                    case float n when (n <= .1):
                        spriteRellenoVida.Scaling = new TGCVector2(.75f * 0.1f * factorCorreccionAncho, 1f * factorCorreccionAlto);
                        break;
                    case float n when (n <= .2):
                        spriteRellenoVida.Scaling = new TGCVector2(.75f * 0.2f * factorCorreccionAncho, 1f * factorCorreccionAlto);
                        break;
                    case float n when (n <= .3):
                        spriteRellenoVida.Scaling = new TGCVector2(.75f * 0.3f * factorCorreccionAncho, 1f * factorCorreccionAlto);
                        break;
                    case float n when (n <= .4):
                        spriteRellenoVida.Scaling = new TGCVector2(.75f * 0.4f * factorCorreccionAncho, 1f * factorCorreccionAlto);
                        break;
                    case float n when (n <= .5):
                        spriteRellenoVida.Scaling = new TGCVector2(.75f * 0.5f * factorCorreccionAncho, 1f * factorCorreccionAlto);
                        break;
                    case float n when (n <= .6):
                        spriteRellenoVida.Scaling = new TGCVector2(.75f * 0.6f * factorCorreccionAncho, 1f * factorCorreccionAlto);
                        break;
                    case float n when (n <= .7):
                        spriteRellenoVida.Scaling = new TGCVector2(.75f * 0.7f * factorCorreccionAncho, 1f * factorCorreccionAlto);
                        break;
                    case float n when (n <= .8):
                        spriteRellenoVida.Scaling = new TGCVector2(.75f * 0.8f * factorCorreccionAncho, 1f * factorCorreccionAlto);
                        break;
                    case float n when (n <= .9):
                        spriteRellenoVida.Scaling = new TGCVector2(.75f * 0.9f * factorCorreccionAncho, 1f * factorCorreccionAlto);
                        break;
                    case float n when (n <= 1):
                        spriteRellenoVida.Scaling = new TGCVector2(.75f * 1.0f * factorCorreccionAncho, 1f * factorCorreccionAlto);
                        break;
                }

                switch (personaje.Oxygen / personaje.MaxOxygen)
                {
                    case float n when (n <= 0):
                        spriteRellenoOxigeno.Scaling = new TGCVector2(.75f * 0.0f * factorCorreccionAncho, 1f * factorCorreccionAlto);
                        break;
                    case float n when (n <= .1):
                        spriteRellenoOxigeno.Scaling = new TGCVector2(.75f * 0.1f * factorCorreccionAncho, 1f * factorCorreccionAlto);
                        break;
                    case float n when (n <= .2):
                        spriteRellenoOxigeno.Scaling = new TGCVector2(.75f * 0.2f * factorCorreccionAncho, 1f * factorCorreccionAlto);
                        break;
                    case float n when (n <= .3):
                        spriteRellenoOxigeno.Scaling = new TGCVector2(.75f * 0.3f * factorCorreccionAncho, 1f * factorCorreccionAlto);
                        break;
                    case float n when (n <= .4):
                        spriteRellenoOxigeno.Scaling = new TGCVector2(.75f * 0.4f * factorCorreccionAncho, 1f * factorCorreccionAlto);
                        break;
                    case float n when (n <= .5):
                        spriteRellenoOxigeno.Scaling = new TGCVector2(.75f * 0.5f * factorCorreccionAncho, 1f * factorCorreccionAlto);
                        break;
                    case float n when (n <= .6):
                        spriteRellenoOxigeno.Scaling = new TGCVector2(.75f * 0.6f * factorCorreccionAncho, 1f * factorCorreccionAlto);
                        break;
                    case float n when (n <= .7):
                        spriteRellenoOxigeno.Scaling = new TGCVector2(.75f * 0.7f * factorCorreccionAncho, 1f * factorCorreccionAlto);
                        break;
                    case float n when (n <= .8):
                        spriteRellenoOxigeno.Scaling = new TGCVector2(.75f * 0.8f * factorCorreccionAncho, 1f * factorCorreccionAlto);
                        break;
                    case float n when (n <= .9):
                        spriteRellenoOxigeno.Scaling = new TGCVector2(.75f * 0.9f * factorCorreccionAncho, 1f * factorCorreccionAlto);
                        break;
                    case float n when (n <= 1):
                        spriteRellenoOxigeno.Scaling = new TGCVector2(.75f * 1.0f * factorCorreccionAncho, 1f * factorCorreccionAlto);
                        break;
                }
            }
            else
            {
                if (personaje.Health <= 0)
                {
                    spriteRellenoVida.Scaling = new TGCVector2(.75f * factorCorreccionAncho, .0f * factorCorreccionAlto);
                }

                if (personaje.Oxygen <= 0)
                {
                    spriteRellenoOxigeno.Scaling = new TGCVector2(.75f * factorCorreccionAncho, .0f * factorCorreccionAlto);
                }
            }
        }

        private void generoPecesAmarillos()
        {
            //pezCircular = new Pez(yellowFish.createMeshInstance("pezPrueba"), new TGCVector3(0, -100, 0), new TGCVector3(0, 1.57f, 0), new MovimientoCircular(new TGCVector3(0, -100, 0), 150, 0.25f));
            //----------------
            //PECES AMARILLOS
            //Se mueven en X
            //Se autoescalan entre 1 y 5
            //Son 64
            //Tienen una velocidad de entre 25 y 75
            //----------------
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int currentMoveDirection = GModel.GetRandom.Next(0, 2) * 2 - 1; //Devuelve aleatoriamente una direccion de movimiento inicial (-1 o 1)
                    float moveSpeed = (float)(GModel.GetRandom.NextDouble() * 75) + 25;

                    Pez pez = new Pez(yellowFish.createMeshInstance(yellowFish.Name + i + "_" + j), currentMoveDirection, moveSpeed);
                    pez.Position = new TGCVector3(GModel.GetRandom.Next(-4000, 4000), GModel.GetRandom.Next(-900, -50), GModel.GetRandom.Next(-4000, 4000));

                    int scale = GModel.GetRandom.Next(1, 5);
                    pez.Scale = new TGCVector3(scale, scale, scale);

                    //Corrijo que los peces vayan para atras
                    if (pez.CurrentMoveDir.Equals(1))
                    {
                        pez.Rotation += new TGCVector3(FastMath.PI, 0, 0);
                        pez.Transform = TGCMatrix.Scaling(pez.Scale) * TGCMatrix.RotationYawPitchRoll(pez.Rotation.X, pez.Rotation.Y, pez.Rotation.Z) * TGCMatrix.Translation(pez.Position);
                    }
                    else
                    {
                        pez.Transform = TGCMatrix.Scaling(pez.Scale) * TGCMatrix.Translation(pez.Position);
                    }

                    pecesAmarillos.Add(pez);
                }

            }
        }

        private void generoPecesAzules()
        {
            //----------------
            //PECES AZULES
            //Se mueven en Z
            //Se autoescalan entre 10 y 20
            //Son 64
            //Tienen una velocidad de entre 40 y 90
            //----------------
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    int currentMoveDirection = GModel.GetRandom.Next(0, 2) * 2 - 1; //Devuelve aleatoriamente una direccion de movimiento inicial (-1 o 1)
                    float moveSpeed = (float)(GModel.GetRandom.NextDouble() * 90) + 40;

                    Pez pez = new Pez(fish.createMeshInstance(fish.Name + i + "_" + j), currentMoveDirection, moveSpeed);
                    pez.Position = new TGCVector3(GModel.GetRandom.Next(-4000, 4000), GModel.GetRandom.Next(-900, -50), GModel.GetRandom.Next(-4000, 4000));

                    int scale = GModel.GetRandom.Next(10, 20);
                    pez.Scale = new TGCVector3(scale, scale, scale);

                    //Corrijo que los peces vayan para atras
                    if (pez.CurrentMoveDir.Equals(1))
                    {
                        pez.Rotation += new TGCVector3(FastMath.PI, 0, 0);
                        pez.Transform = TGCMatrix.Scaling(pez.Scale) * TGCMatrix.RotationYawPitchRoll(pez.Rotation.X, pez.Rotation.Y, pez.Rotation.Z) * TGCMatrix.Translation(pez.Position);
                    }
                    else
                    {
                        pez.Transform = TGCMatrix.Scaling(pez.Scale) * TGCMatrix.Translation(pez.Position);
                    }

                    pecesAzules.Add(pez);
                }

            }
        }

        private void generoHUD()
        {
            //Cargo cursor
            Microsoft.DirectX.Direct3D.Device d3dDevice = D3DDevice.Instance.Device;
            bitmapCursor = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "cursor_default.png", d3dDevice);
            spriteCursor.Bitmap = bitmapCursor;
            spriteCursor.Scaling = new TGCVector2(1f * factorCorreccionAncho, 1f * factorCorreccionAlto);
            spriteCursor.Position = new TGCVector2(ScreenWidth / 2.093f, ScreenHeight / 2.17f);
            sprites.Add(spriteCursor);
            //HUD
            bitmapCorazon = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "Vida.png", D3DDevice.Instance.Device);
            spriteCorazon.Bitmap = bitmapCorazon;
            spriteCorazon.Scaling = new TGCVector2(0.15f * factorCorreccionAncho, 0.15f * factorCorreccionAlto);
            spriteCorazon.Position = new TGCVector2(ScreenWidth / 1.1f, ScreenHeight / 1.15f);
            sprites.Add(spriteCorazon);

            bitmapTanqueOxigeno = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "Oxygen.png", D3DDevice.Instance.Device);
            spriteTanqueOxigeno.Bitmap = bitmapTanqueOxigeno;
            spriteTanqueOxigeno.Scaling = new TGCVector2(0.25f * factorCorreccionAncho, 0.2f * factorCorreccionAlto);
            spriteTanqueOxigeno.Position = new TGCVector2(ScreenWidth / 1.125f, ScreenHeight / 1.4f);
            sprites.Add(spriteTanqueOxigeno);

            bitmapBarra = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "Barra0.0.png", D3DDevice.Instance.Device);
            spriteBarraOxigeno.Bitmap = bitmapBarra;
            spriteBarraOxigeno.Scaling = new TGCVector2(.75f * factorCorreccionAncho, 1f * factorCorreccionAlto);
            spriteBarraOxigeno.Position = new TGCVector2(ScreenWidth / 1.7f, ScreenHeight / 1.325f);
            sprites.Add(spriteBarraOxigeno);

            spriteBarraVida.Bitmap = bitmapBarra;
            spriteBarraVida.Scaling = new TGCVector2(.75f * factorCorreccionAncho, 1f * factorCorreccionAlto);
            spriteBarraVida.Position = new TGCVector2(ScreenWidth / 1.7f, ScreenHeight / 1.13f);
            sprites.Add(spriteBarraVida);
            
            bitmapRellenoOxigeno = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "RellenoOxigeno.png", D3DDevice.Instance.Device);
            spriteRellenoOxigeno.Bitmap = bitmapRellenoOxigeno;
            spriteRellenoOxigeno.Scaling = new TGCVector2(.75f * factorCorreccionAncho, 1f * factorCorreccionAlto);
            spriteRellenoOxigeno.Position = new TGCVector2(ScreenWidth / 1.7f, ScreenHeight / 1.325f);
            sprites.Add(spriteRellenoOxigeno);

            bitmapRellenoVida = new CustomBitmap(GModel.MediaDir + "Bitmaps\\" + "RellenoVida.png", D3DDevice.Instance.Device);
            spriteRellenoVida.Bitmap = bitmapRellenoVida;
            spriteRellenoVida.Scaling = new TGCVector2(.75f * factorCorreccionAncho, 1f * factorCorreccionAlto);
            spriteRellenoVida.Position = new TGCVector2(ScreenWidth / 1.7f, ScreenHeight / 1.13f);
            sprites.Add(spriteRellenoVida);
            
        }

        private void cargoMeshes()
        {
            workbench = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "ModelosTgc\\Workbench\\Workbench-TgcScene.xml").Meshes[0];

            objetosDelTerreno = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "MeshCreator\\Scenes\\vegetacionScena\\objetosScene-TgcScene.xml");

            coral = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\coral-TgcScene.xml").Meshes[0];

            meshTiburon = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\shark-TgcScene.xml").Meshes[0];

            coralBrain = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\brain_coral-TgcScene.xml").Meshes[0];

            barco = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\ship-TgcScene.xml");

            fish = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\fish-TgcScene.xml").Meshes[0];

            pillarCoral = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\pillar_coral-TgcScene.xml").Meshes[0];

            seaShell = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\sea_shell-TgcScene.xml").Meshes[0];

            spiralWireCoral = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\spiral_wire_coral-TgcScene.xml").Meshes[0];

            treeCoral = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\tree_coral-TgcScene.xml").Meshes[0];

            yellowFish = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\Aquatic\\Meshes\\yellow_fish-TgcScene.xml").Meshes[0];

            /*arbusto = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\MeshCreator\\Meshes\\Vegetacion\\Arbusto\\Arbusto-TgcScene.xml").Meshes[0];

            arbusto2 = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\MeshCreator\\Meshes\\Vegetacion\\Arbusto2\\Arbusto2-TgcScene.xml").Meshes[0];

            pasto = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\MeshCreator\\Meshes\\Vegetacion\\Pasto\\Pasto-TgcScene.xml").Meshes[0];

            planta = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\MeshCreator\\Meshes\\Vegetacion\\Planta\\Planta-TgcScene.xml").Meshes[0];

            planta2 = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\MeshCreator\\Meshes\\Vegetacion\\Planta2\\Planta2-TgcScene.xml").Meshes[0];

            planta3 = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\MeshCreator\\Meshes\\Vegetacion\\Planta3\\Planta3-TgcScene.xml").Meshes[0];

            roca = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\MeshCreator\\Meshes\\Vegetacion\\Roca\\Roca-TgcScene.xml").Meshes[0];
            */

            //redPesca = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\ModelosTgc\\RedPesca\\RedPesca-TgcScene.xml").Meshes[0];

            arponMesh = new TgcSceneLoader().loadSceneFromFile(GModel.MediaDir + "\\ModelosTgc\\Arpon\\Arpon-TgcScene.xml").Meshes[0];
        }

        private void generoObjetosEstaticosSueltos()
        {
            barco.Meshes.ForEach(meshBarco => meshBarco.Scale = new TGCVector3(10,10,10));
            barco.Meshes.ForEach(meshBarco => meshBarco.Position = new TGCVector3(-100,0,-100));

            //--------------objetos---------
            /*coral.Position = new TGCVector3(100, -1000, -435);
            coral.Scale = new TGCVector3(15, 15, 15);*/

            TGCVector3 posCoral = new TGCVector3(10, -300, 0);
            coral.AutoTransformEnable = false;
            coral.Transform = TGCMatrix.Translation(posCoral);


            tiburon = new Tiburon(meshTiburon, GModel);
            tiburon.RotateY(FastMath.PI / 2 + FastMath.PI / 4);
            tiburon.Move(new TGCVector3(-650, -100, 1000));
            tiburon.Scale = new TGCVector3(1f, 1f, 1f);
            var cuerpoRigido = BulletRigidBodyFactory.Instance.CreateCapsule(28f, 220f, tiburon.Position, 0, false);
            cuerpoRigido.CollisionFlags = CollisionFlags.KinematicObject;
            cuerpoRigido.ActivationState = ActivationState.DisableDeactivation;
            tiburon.CuerpoRigido = cuerpoRigido;
            dynamicsWorld.AddRigidBody(cuerpoRigido);


            TGCVector3 posCoralBrain = new TGCVector3(10, -300, 0);
            //coralBrain.Position = new TGCVector3(-200, -1000, 340);
            coralBrain.AutoTransformEnable = false;
            coralBrain.Transform = TGCMatrix.Translation(posCoralBrain);

            //fish.Position = new TGCVector3(0, -1000, 0);
            //fish.Transform = TGCMatrix.Translation(fish.Position);

            TGCVector3 posPillarCoral = new TGCVector3(10, -300, 0);
            //pillarCoral.Position = new TGCVector3(0, -1000, 40);
            pillarCoral.AutoTransformEnable = false;
            pillarCoral.Transform = TGCMatrix.Translation(posPillarCoral);

            TGCVector3 posSeaShell = new TGCVector3(10, -300, 0);
            //seaShell.Position = new TGCVector3(500, -1000, 40);
            seaShell.AutoTransformEnable = false;
            seaShell.Transform = TGCMatrix.Translation(posSeaShell);

            TGCVector3 posSpiralWireCoral = new TGCVector3(10, -300, 0);
            //spiralWireCoral.Position = new TGCVector3(-50, -1000, 40);
            spiralWireCoral.AutoTransformEnable = false;
            spiralWireCoral.Transform = TGCMatrix.Translation(posSpiralWireCoral);

            TGCVector3 posTreeCoral = new TGCVector3(10, -300, 0);
            TGCVector3 escalaTreeCoral = new TGCVector3(10f, 10f, 10f);
           // treeCoral.Position = new TGCVector3(-70, -1000, 200);
            treeCoral.AutoTransformEnable = false;
            treeCoral.Transform = TGCMatrix.Scaling(escalaTreeCoral) * TGCMatrix.Translation(posTreeCoral);

            //yellowFish.Position = new TGCVector3(50, -500, -20);
            //yellowFish.Transform = TGCMatrix.Translation(yellowFish.Position);

           /* arbusto.Position = new TGCVector3(70, -1000, -30);
            arbusto.AlphaBlendEnable = true;
            arbusto.Transform = TGCMatrix.Translation(arbusto.Position);

            arbusto2.Position = new TGCVector3(60, -1000, -20);
            arbusto2.Transform = TGCMatrix.Translation(arbusto2.Position);

            pasto.Position = new TGCVector3(50, -1000, -20);
            pasto.Transform = TGCMatrix.Translation(pasto.Position);

            planta.Position = new TGCVector3(40, -1000, -20);
            planta.Transform = TGCMatrix.Translation(planta.Position);

            planta2.Position = new TGCVector3(30, -1000, -20);
            planta2.Transform = TGCMatrix.Translation(planta2.Position);

            planta3.Position = new TGCVector3(20, -1000, -20);
            planta3.Transform = TGCMatrix.Translation(planta3.Position);

            roca.Position = new TGCVector3(10, -1000, -20);
            roca.Transform = TGCMatrix.Translation(roca.Position);*/
        }

        private void generoObjetosEstaticosEnArray()
        {
            TgcMesh instance;
            BvhTriangleMeshShape childTriangleMesh;
            RigidBody rigidBody;
            RecolectableConMesh instanceMesh;
            TGCVector3 posicion;
            var rows = 10;
            var cols = 10;

            //-------CoralBrain---------
            var x = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
            var z = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
            posicion = new TGCVector3(x, CalcularAltura(x,z,terreno),z );

            while (posicion.Y >= 0) //NO OBJECTS OVER SEA LEVEL
            {
                x = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                z = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                posicion = new TGCVector3(x, CalcularAltura(x, z, terreno), z);
            }

            instanceMesh = new RecolectableConMesh(coralBrain, new TGCVector3(67, 0, 0), posicion, "BrainCoral");
            instanceMesh.Mesh.Scale = new TGCVector3(10, 10, 10);
            instanceMesh.Mesh.Transform = TGCMatrix.Scaling(instanceMesh.Mesh.Scale) * TGCMatrix.Translation(instanceMesh.Mesh.Position);
            objetosRecolectables.Add(instanceMesh);
            //Contruyo el BulletShape de base para clonar al resto de los objetos
            childTriangleMesh = construirTriangleMeshShape(instanceMesh.Mesh);
            rigidBody = construirRigidBodyDeTriangleMeshShape(childTriangleMesh);
            dynamicsWorld.AddRigidBody(rigidBody);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    x = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                    z = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                    posicion = new TGCVector3(x, CalcularAltura(x, z, terreno), z);

                    while (posicion.Y >= 0) //NO OBJECTS OVER SEA LEVEL
                    {
                        x = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                        z = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                        posicion = new TGCVector3(x, CalcularAltura(x, z, terreno), z);
                    }

                    instanceMesh = new RecolectableConMesh(coralBrain, new TGCVector3(67, 0, 0), posicion, "BrainCoral");
                    instanceMesh.Mesh.Scale = new TGCVector3(2, 2, 2);
                    instanceMesh.Mesh.Transform = TGCMatrix.Scaling(instanceMesh.Mesh.Scale) * TGCMatrix.Translation(instanceMesh.Mesh.Position);
                    objetosRecolectables.Add(instanceMesh);
                    //Creo una instancia de RigidBody con el BulletShape de base, la clono, escalo y posiciono
                    rigidBody = construirRigidBodyDeChildTriangleMeshShape(childTriangleMesh, instanceMesh.Mesh.Position, instanceMesh.Mesh.Scale);
                    dynamicsWorld.AddRigidBody(rigidBody);

                }

            }
            //--------Coral--------
            instance = crearInstanciasObjetosEstaticos(coral, 10000, 2, coral.Name + 0 + "_" + 0);
            objetosEstaticosEnArray.Add(instance);
            //Contruyo el BulletShape de base para clonar al resto de los objetos
            childTriangleMesh = construirTriangleMeshShape(instance);
            rigidBody = construirRigidBodyDeTriangleMeshShape(childTriangleMesh);
            dynamicsWorld.AddRigidBody(rigidBody);

            for (int i = 1; i < rows; i++)
            {
                for (int j = 1; j < cols; j++)
                {
                    instance = crearInstanciasObjetosEstaticos(coral, 10000, 2, coral.Name + i + "_" + j);
                    objetosEstaticosEnArray.Add(instance);
                    //Creo una instancia de RigidBody con el BulletShape de base, la clono, escalo y posiciono
                    rigidBody = construirRigidBodyDeChildTriangleMeshShape(childTriangleMesh, instance.Position, instance.Scale);
                    dynamicsWorld.AddRigidBody(rigidBody);                    
                }

            }
            //---------PillarCoral--------
            instance = crearInstanciasObjetosEstaticos(pillarCoral, 10000, 10, pillarCoral.Name + 0 + "_" + 0);
            objetosEstaticosEnArray.Add(instance);
            //Contruyo el BulletShape de base para clonar al resto de los objetos
            childTriangleMesh = construirTriangleMeshShape(instance);
            rigidBody = construirRigidBodyDeTriangleMeshShape(childTriangleMesh);
            dynamicsWorld.AddRigidBody(rigidBody);

            for (int i = 1; i < rows; i++)
            {
                for (int j = 1; j < cols; j++)
                {
                    instance = crearInstanciasObjetosEstaticos(pillarCoral, 10000, 10, pillarCoral.Name + i + "_" + j);
                    objetosEstaticosEnArray.Add(instance);
                    //Creo una instancia de RigidBody con el BulletShape de base, la clono, escalo y posiciono
                    rigidBody = construirRigidBodyDeChildTriangleMeshShape(childTriangleMesh, instance.Position, instance.Scale);
                    dynamicsWorld.AddRigidBody(rigidBody);
                }

            }

            //-------SeaShell----------
            x = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
            z = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
            posicion = new TGCVector3(x, CalcularAltura(x, z, terreno), z);

            while (posicion.Y >= 0) //NO OBJECTS OVER SEA LEVEL
            {
                x = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                z = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                posicion = new TGCVector3(x, CalcularAltura(x, z, terreno), z);
            }

            instanceMesh = new RecolectableConMesh(seaShell, new TGCVector3(67, 0, 0), posicion, "SeaShell");
            instanceMesh.Mesh.Scale = new TGCVector3(2, 2, 2);
            instanceMesh.Mesh.Transform = TGCMatrix.Scaling(instanceMesh.Mesh.Scale) * TGCMatrix.Translation(instanceMesh.Mesh.Position);
            objetosRecolectables.Add(instanceMesh);
            //Contruyo el BulletShape de base para clonar al resto de los objetos
            childTriangleMesh = construirTriangleMeshShape(instanceMesh.Mesh);
            rigidBody = construirRigidBodyDeTriangleMeshShape(childTriangleMesh);
            dynamicsWorld.AddRigidBody(rigidBody);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    x = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                    z = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                    posicion = new TGCVector3(x, CalcularAltura(x, z, terreno), z);

                    while (posicion.Y >= 0) //NO OBJECTS OVER SEA LEVEL
                    {
                        x = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                        z = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                        posicion = new TGCVector3(x, CalcularAltura(x, z, terreno), z);
                    }

                    posicion = new TGCVector3(x, CalcularAltura(x, z, terreno), z);
                    instanceMesh = new RecolectableConMesh(seaShell, new TGCVector3(67, 0, 0), posicion, "SeaShell");
                    instanceMesh.Mesh.Scale = new TGCVector3(2, 2, 2);
                    instanceMesh.Mesh.Transform = TGCMatrix.Scaling(instanceMesh.Mesh.Scale) * TGCMatrix.Translation(instanceMesh.Mesh.Position);
                    objetosRecolectables.Add(instanceMesh);
                    //Creo una instancia de RigidBody con el BulletShape de base, la clono, escalo y posiciono
                    rigidBody = construirRigidBodyDeChildTriangleMeshShape(childTriangleMesh, instanceMesh.Mesh.Position, instanceMesh.Mesh.Scale);
                    dynamicsWorld.AddRigidBody(rigidBody);

                }

            }
            //--------treeCoral---------
            instance = crearInstanciasObjetosEstaticos(treeCoral, 10000, 8, treeCoral.Name + 0 + "_" + 0);
            objetosEstaticosEnArray.Add(instance);
            //Contruyo el BulletShape de base para clonar al resto de los objetos
            childTriangleMesh = construirTriangleMeshShape(instance);
            rigidBody = construirRigidBodyDeTriangleMeshShape(childTriangleMesh);
            dynamicsWorld.AddRigidBody(rigidBody);

            for (int i = 1; i < rows; i++)
            {
                for (int j = 1; j < cols; j++)
                {
                    instance = crearInstanciasObjetosEstaticos(treeCoral, 10000, 8, treeCoral.Name + i + "_" + j);
                    objetosEstaticosEnArray.Add(instance);
                    //Creo una instancia de RigidBody con el BulletShape de base, la clono, escalo y posiciono
                    rigidBody = construirRigidBodyDeChildTriangleMeshShape(childTriangleMesh, instance.Position, instance.Scale);
                    dynamicsWorld.AddRigidBody(rigidBody);
                }

            }
            //--------spiralWireCoral---------
            instance = crearInstanciasObjetosEstaticos(spiralWireCoral, 10000, 5, spiralWireCoral.Name + 0 + "_" + 0);
            objetosEstaticosEnArray.Add(instance);
            //Contruyo el BulletShape de base para clonar al resto de los objetos
            childTriangleMesh = construirTriangleMeshShape(instance);
            rigidBody = construirRigidBodyDeTriangleMeshShape(childTriangleMesh);
            dynamicsWorld.AddRigidBody(rigidBody);

            for (int i = 1; i < rows; i++)
            {
                for (int j = 1; j < cols; j++)
                {
                    instance = crearInstanciasObjetosEstaticos(spiralWireCoral, 10000, 5, spiralWireCoral.Name + i + "_" + j);
                    objetosEstaticosEnArray.Add(instance);
                    //Creo una instancia de RigidBody con el BulletShape de base, la clono, escalo y posiciono
                    rigidBody = construirRigidBodyDeChildTriangleMeshShape(childTriangleMesh, instance.Position, instance.Scale);
                    dynamicsWorld.AddRigidBody(rigidBody);
                }

            }
            /*
            //--------arbusto---------
            instance = crearInstanciasObjetosEstaticos(arbusto, 10000, 5, arbusto.Name + 0 + "_" + 0);
            instance.AlphaBlendEnable = true;
            objetosEstaticosEnArray.Add(instance);
            //Contruyo el BulletShape de base para clonar al resto de los objetos
            childTriangleMesh = construirTriangleMeshShape(instance);
            rigidBody = construirRigidBodyDeTriangleMeshShape(childTriangleMesh);
            dynamicsWorld.AddRigidBody(rigidBody);

            for (int i = 1; i < rows; i++)
            {
                for (int j = 1; j < cols; j++)
                {
                    instance = crearInstanciasObjetosEstaticos(arbusto, 10000, 5, arbusto.Name + i + "_" + j);
                    instance.AlphaBlendEnable = true;
                    objetosEstaticosEnArray.Add(instance);
                    //Creo una instancia de RigidBody con el BulletShape de base, la clono, escalo y posiciono
                    rigidBody = construirRigidBodyDeChildTriangleMeshShape(childTriangleMesh, instance.Position, instance.Scale);
                    dynamicsWorld.AddRigidBody(rigidBody);
                }

            }
            //--------arbusto 2---------
            instance = crearInstanciasObjetosEstaticos(arbusto2, 10000, 5, arbusto2.Name + 0 + "_" + 0);
            instance.AlphaBlendEnable = true;
            objetosEstaticosEnArray.Add(instance);
            //Contruyo el BulletShape de base para clonar al resto de los objetos
            childTriangleMesh = construirTriangleMeshShape(instance);
            rigidBody = construirRigidBodyDeTriangleMeshShape(childTriangleMesh);
            dynamicsWorld.AddRigidBody(rigidBody);

            for (int i = 1; i < rows; i++)
            {
                for (int j = 1; j < cols; j++)
                {
                    instance = crearInstanciasObjetosEstaticos(arbusto2, 10000, 5, arbusto2.Name + i + "_" + j);
                    instance.AlphaBlendEnable = true;
                    objetosEstaticosEnArray.Add(instance);
                    //Creo una instancia de RigidBody con el BulletShape de base, la clono, escalo y posiciono
                    rigidBody = construirRigidBodyDeChildTriangleMeshShape(childTriangleMesh, instance.Position, instance.Scale);
                    dynamicsWorld.AddRigidBody(rigidBody);
                }

            }
            //--------pasto---------
            instance = crearInstanciasObjetosEstaticos(pasto, 10000, 5, pasto.Name + 0 + "_" + 0);
            instance.AlphaBlendEnable = true;
            objetosEstaticosEnArray.Add(instance);
            //Contruyo el BulletShape de base para clonar al resto de los objetos
            childTriangleMesh = construirTriangleMeshShape(instance);
            rigidBody = construirRigidBodyDeTriangleMeshShape(childTriangleMesh);
            dynamicsWorld.AddRigidBody(rigidBody);

            for (int i = 1; i < rows; i++)
            {
                for (int j = 1; j < cols; j++)
                {
                    instance = crearInstanciasObjetosEstaticos(pasto, 10000, 5, pasto.Name + i + "_" + j);
                    instance.AlphaBlendEnable = true;
                    objetosEstaticosEnArray.Add(instance);
                    //Creo una instancia de RigidBody con el BulletShape de base, la clono, escalo y posiciono
                    rigidBody = construirRigidBodyDeChildTriangleMeshShape(childTriangleMesh, instance.Position, instance.Scale);
                    dynamicsWorld.AddRigidBody(rigidBody);
                }

            }
            //--------planta---------
            instance = crearInstanciasObjetosEstaticos(planta, 10000, 5, planta.Name + 0 + "_" + 0);
            objetosEstaticosEnArray.Add(instance);
            //Contruyo el BulletShape de base para clonar al resto de los objetos
            childTriangleMesh = construirTriangleMeshShape(instance);
            rigidBody = construirRigidBodyDeTriangleMeshShape(childTriangleMesh);
            dynamicsWorld.AddRigidBody(rigidBody);

            for (int i = 1; i < rows; i++)
            {
                for (int j = 1; j < cols; j++)
                {
                    instance = crearInstanciasObjetosEstaticos(planta, 10000, 5, planta.Name + i + "_" + j);
                    objetosEstaticosEnArray.Add(instance);
                    //Creo una instancia de RigidBody con el BulletShape de base, la clono, escalo y posiciono
                    rigidBody = construirRigidBodyDeChildTriangleMeshShape(childTriangleMesh, instance.Position, instance.Scale);
                    dynamicsWorld.AddRigidBody(rigidBody);
                }

            }
            //--------planta 2---------
            instance = crearInstanciasObjetosEstaticos(planta2, 10000, 5, planta2.Name + 0 + "_" + 0);
            objetosEstaticosEnArray.Add(instance);
            //Contruyo el BulletShape de base para clonar al resto de los objetos
            childTriangleMesh = construirTriangleMeshShape(instance);
            rigidBody = construirRigidBodyDeTriangleMeshShape(childTriangleMesh);
            dynamicsWorld.AddRigidBody(rigidBody);

            for (int i = 1; i < rows; i++)
            {
                for (int j = 1; j < cols; j++)
                {
                    instance = crearInstanciasObjetosEstaticos(planta2, 10000, 5, planta2.Name + i + "_" + j);
                    objetosEstaticosEnArray.Add(instance);
                    //Creo una instancia de RigidBody con el BulletShape de base, la clono, escalo y posiciono
                    rigidBody = construirRigidBodyDeChildTriangleMeshShape(childTriangleMesh, instance.Position, instance.Scale);
                    dynamicsWorld.AddRigidBody(rigidBody);
                }

            }
            //--------planta3---------
            instance = crearInstanciasObjetosEstaticos(planta3, 10000, 5, planta3.Name + 0 + "_" + 0);
            instance.AlphaBlendEnable = true;
            objetosEstaticosEnArray.Add(instance);
            //Contruyo el BulletShape de base para clonar al resto de los objetos
            childTriangleMesh = construirTriangleMeshShape(instance);
            rigidBody = construirRigidBodyDeTriangleMeshShape(childTriangleMesh);
            dynamicsWorld.AddRigidBody(rigidBody);

            for (int i = 1; i < rows; i++)
            {
                for (int j = 1; j < cols; j++)
                {
                    instance = crearInstanciasObjetosEstaticos(planta3, 10000, 5, planta3.Name + i + "_" + j);
                    instance.AlphaBlendEnable = true;
                    objetosEstaticosEnArray.Add(instance);
                    //Creo una instancia de RigidBody con el BulletShape de base, la clono, escalo y posiciono
                    rigidBody = construirRigidBodyDeChildTriangleMeshShape(childTriangleMesh, instance.Position, instance.Scale);
                    dynamicsWorld.AddRigidBody(rigidBody);
                }

            }
            //--------roca---------
            instance = crearInstanciasObjetosEstaticos(roca, 10000, 5, roca.Name + 0 + "_" + 0);
            objetosEstaticosEnArray.Add(instance);
            //Contruyo el BulletShape de base para clonar al resto de los objetos
            childTriangleMesh = construirTriangleMeshShape(instance);
            rigidBody = construirRigidBodyDeTriangleMeshShape(childTriangleMesh);
            dynamicsWorld.AddRigidBody(rigidBody);

            for (int i = 1; i < rows; i++)
            {
                for (int j = 1; j < cols; j++)
                {
                    instance = crearInstanciasObjetosEstaticos(roca, 10000, 5, roca.Name + i + "_" + j);
                    objetosEstaticosEnArray.Add(instance);
                    //Creo una instancia de RigidBody con el BulletShape de base, la clono, escalo y posiciono
                    rigidBody = construirRigidBodyDeChildTriangleMeshShape(childTriangleMesh, instance.Position, instance.Scale);
                    dynamicsWorld.AddRigidBody(rigidBody);
                }

            }*/
        }

        private void cargoSkybox()
        {
            var skyBoxSize = sizeMapa / 4;
            skybox = new TgcSkyBox();
            skybox.Center = TGCVector3.Empty;
            skybox.Size = new TGCVector3(skyBoxSize, skyBoxSize, skyBoxSize);

            var texturesPath = GModel.MediaDir + "Texturas\\SkyBox\\";

            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Up, texturesPath + "lostatseaday_up.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Down, texturesPath + "lostatseaday_dn.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Left, texturesPath + "lostatseaday_lf.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Right, texturesPath + "lostatseaday_rt.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Back, texturesPath + "lostatseaday_ft.jpg");
            skybox.setFaceTexture(TgcSkyBox.SkyFaces.Front, texturesPath + "lostatseaday_bk.jpg");
            skybox.SkyEpsilon = 25f;
            skybox.Init();
        }

        private void cargoHeightmap()
        {
            terreno = new TgcSimpleTerrain();
            var path = GModel.MediaDir + "Texturas\\Heighmaps\\heightmap1Final.jpg";
            var textu = GModel.MediaDir + "Texturas\\mountain.jpg";
            currentScaleXZ = 100f;
            currentScaleY = 25;
            terreno.loadHeightmap(path, currentScaleXZ, currentScaleY, new TGCVector3(0, -210, 0));
            terreno.loadTexture(textu);
            terreno.AlphaBlendEnable = true;
        }

        
        private void cargoPisos()
        {
            var aguaTextura = TgcTexture.createTexture(D3DDevice.Instance.Device, GModel.MediaDir + "Texturas\\agua20.jpg");
            agua = new TgcPlane(new TGCVector3(-sizeMapa / 2, 0, -sizeMapa / 2), new TGCVector3(sizeMapa, 0, sizeMapa), TgcPlane.Orientations.XZplane, aguaTextura);

            var pisoTextura = TgcTexture.createTexture(D3DDevice.Instance.Device, GModel.MediaDir + "Texturas\\seabed.jpg");
            piso = new TgcPlane(new TGCVector3(-sizeMapa / 2, fondoMapa, -sizeMapa / 2), new TGCVector3(sizeMapa, 0, sizeMapa), TgcPlane.Orientations.XZplane, pisoTextura);

            //FLOOR
            var floorShape = new StaticPlaneShape(TGCVector3.Up.ToBulletVector3(), fondoMapa);
            var floorMotionState = new DefaultMotionState();
            var floorInfo = new RigidBodyConstructionInfo(0, floorMotionState, floorShape);
            var bodyFloor = new RigidBody(floorInfo);

            //ROOF
            var techoShape = new StaticPlaneShape(TGCVector3.Down.ToBulletVector3(), -alturaTecho);
            var techoMotionState = new DefaultMotionState();
            var techoInfo = new RigidBodyConstructionInfo(0, techoMotionState, techoShape);
            var bodyTecho = new RigidBody(techoInfo);

            dynamicsWorld.AddRigidBody(bodyFloor);
            dynamicsWorld.AddRigidBody(bodyTecho); //SIRVE PARA LIMITAR LA ALTURA DEL PERSONAJE... HACER QUE LAS LANZAS DESAPAREZCAN CUANDO TOCAN EL TECHO
        }

        private void cargoMusica()
        {
            //musica.FileName = GModel.MediaDir + "\\Music\\AbandonShip.mp3";
            sonidoTiburonCerca.loadSound(GModel.MediaDir + "\\Music\\SharkNear.wav", GModel.DirectSound.DsDevice);
        }

        private void generoMetales()
        {
            var texturaOro = TgcTexture.createTexture(GModel.MediaDir + "\\Texturas\\oro.jpg");
            var texturaRubi = TgcTexture.createTexture(GModel.MediaDir + "\\Texturas\\ruby.jpg");
            var texturaPlatino = TgcTexture.createTexture(GModel.MediaDir + "\\Texturas\\platinum.jpg");

            for (int i = 0; i < 16; i++)
            {
                for (int j = 0; j < 22; j++)
                {
                    var side = GModel.GetRandom.Next(50, 75);
                    var tamanio = new TGCVector3(side, side / 4, side / 2);

                    var x = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                    var z = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                    var posicion = new TGCVector3(x, CalcularAltura(x, z, terreno) + side/8, z);

                    while (posicion.Y >= 0) //NO OBJECTS OVER SEA LEVEL
                    {
                        x = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                        z = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                        posicion = new TGCVector3(x, CalcularAltura(x, z, terreno) + side / 8, z);
                    }
                    
                    var metalRigidBody = BulletRigidBodyFactory.Instance.CreateBox(tamanio, 100, posicion, 0, 0, 0, 10, false); //ACLARACION: Se esta reutilizando vector tamanio
                    dynamicsWorld.AddRigidBody(metalRigidBody);
                    tamanio.Multiply(2f);
                    var instance = new RecolectableConTextura(texturaOro, tamanio, posicion, "Oro");
                    instance.CuerpoRigido = metalRigidBody;
                    objetosRecolectables.Add(instance);
                }

            }

            for (int i = 0; i < 12; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    var side = GModel.GetRandom.Next(50, 75);
                    var tamanio = new TGCVector3(side, side, side);

                    var x = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                    var z = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                    var posicion = new TGCVector3(x, CalcularAltura(x, z, terreno) + side / 8, z);

                    while (posicion.Y >= 0) //NO OBJECTS OVER SEA LEVEL
                    {
                        x = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                        z = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                        posicion = new TGCVector3(x, CalcularAltura(x, z, terreno) + side / 8, z);
                    }

                    var metalRigidBody = BulletRigidBodyFactory.Instance.CreateBox(tamanio, 100, posicion, 0, 0, 0, 10, false); //ACLARACION: Se esta reutilizando vector tamanio
                    dynamicsWorld.AddRigidBody(metalRigidBody);
                    tamanio.Multiply(2f);
                    var instance = new RecolectableConTextura(texturaRubi, tamanio, posicion, "Ruby");
                    instance.CuerpoRigido = metalRigidBody;
                    objetosRecolectables.Add(instance);
                }

            }

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var side = GModel.GetRandom.Next(50, 75);
                    var tamanio = new TGCVector3(side, side, side);

                    var x = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                    var z = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                    var posicion = new TGCVector3(x, CalcularAltura(x, z, terreno) + side / 8, z);

                    while (posicion.Y >= 0) //NO OBJECTS OVER SEA LEVEL
                    {
                        x = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                        z = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                        posicion = new TGCVector3(x, CalcularAltura(x, z, terreno) + side / 8, z);
                    }

                    var metalRigidBody = BulletRigidBodyFactory.Instance.CreateBox(tamanio, 100, posicion, 0, 0, 0, 10, false); //ACLARACION: Se esta reutilizando vector tamanio
                    dynamicsWorld.AddRigidBody(metalRigidBody);
                    tamanio.Multiply(2f);
                    var instance = new RecolectableConTextura(texturaPlatino, tamanio, posicion, "Platino");
                    instance.CuerpoRigido = metalRigidBody;
                    objetosRecolectables.Add(instance);
                }
            }

        }

        public void detectarColision(TgcBoundingCylinder cilindro)
        {
            List<ObjetoRecolectable> objetosEnColision = objetosRecolectables.Where(objeto => objeto.colisionaCon(cilindro)).ToList();
            if (objetosEnColision.Any())
            {
                cilindro.setRenderColor(Color.Red);
                objetoAMostrar = objetosEnColision.First();
                if (GModel.Input.keyPressed(Key.E))
                {
                    GModel.Personaje.Inventario.agregaObjeto(new ObjetoInventario(objetoAMostrar.Nombre, 1));
                    objetosRecolectables.Remove(objetoAMostrar);
                    if(objetoAMostrar.CuerpoRigido != null)
                    {
                        dynamicsWorld.RemoveCollisionObject(objetoAMostrar.CuerpoRigido);
                        objetoAMostrar.CuerpoRigido.Dispose();
                    }                    
                    objetoAMostrar = null;
                }
            }
            else
            {
                cilindro.setRenderColor(Color.LimeGreen);
                objetoAMostrar = null;
            }
        }

        private TgcMesh crearInstanciasObjetosEstaticos(TgcMesh mesh, int randomPosition, int escala, String name)
        {
            var instance = mesh.createMeshInstance(name);
            var x = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
            var z = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
            var posicion = new TGCVector3(x, CalcularAltura(x, z, terreno), z);

            while (posicion.Y >= 0) //NO OBJECTS OVER SEA LEVEL
            {
                x = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                z = GModel.GetRandom.Next(-sizeMapa / 2, sizeMapa / 2);
                posicion = new TGCVector3(x, CalcularAltura(x, z, terreno), z);
            }

            instance.Position = posicion;
            //escalado random
            var escalaObjeto = GModel.GetRandom.Next(escala, escala * 3);
            instance.Scale = new TGCVector3(escalaObjeto, escalaObjeto, escalaObjeto);
            instance.Transform = TGCMatrix.Scaling(instance.Scale) * TGCMatrix.Translation(instance.Position);

            return instance;
        }

        private BvhTriangleMeshShape construirTriangleMeshShape(TgcMesh mesh)
        {
            var vertexCoords = mesh.getVertexPositions();

            TriangleMesh triangleMesh = new TriangleMesh();
            for (int i = 0; i < vertexCoords.Length; i = i + 3)
            {
                triangleMesh.AddTriangle(vertexCoords[i].ToBulletVector3(), vertexCoords[i + 1].ToBulletVector3(), vertexCoords[i + 2].ToBulletVector3());
            }
            return new BvhTriangleMeshShape(triangleMesh, false);
        }

        private RigidBody construirRigidBodyDeTriangleMeshShape(BvhTriangleMeshShape triangleMeshShape)
        {
            var transformationMatrix = TGCMatrix.RotationYawPitchRoll(0, 0, 0).ToBsMatrix;
            DefaultMotionState motionState = new DefaultMotionState(transformationMatrix);

            var boxLocalInertia = triangleMeshShape.CalculateLocalInertia(0);
            var bodyInfo = new RigidBodyConstructionInfo(0, motionState, triangleMeshShape, boxLocalInertia);
            var rigidBody = new RigidBody(bodyInfo);
            rigidBody.Friction = 0.4f;
            rigidBody.RollingFriction = 1;
            rigidBody.Restitution = 1f;

            return rigidBody;
        }

        private RigidBody construirRigidBodyDeChildTriangleMeshShape(BvhTriangleMeshShape triangleMeshShape, TGCVector3 posicion, TGCVector3 escalado)
        {
            var transformationMatrix = TGCMatrix.RotationYawPitchRoll(0, 0, 0);
            transformationMatrix.Origin = posicion;
            DefaultMotionState motionState = new DefaultMotionState(transformationMatrix.ToBsMatrix);

            var bulletShape = new ScaledBvhTriangleMeshShape(triangleMeshShape, escalado.ToBulletVector3());
            var boxLocalInertia = bulletShape.CalculateLocalInertia(0);

            var bodyInfo = new RigidBodyConstructionInfo(0, motionState, bulletShape, boxLocalInertia);
            var rigidBody = new RigidBody(bodyInfo);
            rigidBody.Friction = 0.4f;
            rigidBody.RollingFriction = 1;
            rigidBody.Restitution = 1f;

            return rigidBody;
        }

        public void hacerSonarTiburonCerca()
        {
            if (musica.getStatus().Equals(TgcMp3Player.States.Playing))
            {
                musica.pause();
            }

            sonidoTiburonCerca.play(true);
        }

        public void detenerSonidoTiburonCerca()
        {
            sonidoTiburonCerca.stop();
        }

        public float MovementSpeed { get => MOVEMENT_SPEED; }

        public TgcScene Barco { get => barco; }

        public TgcMesh Workbench { get => workbench; }

        public RigidBody RigidCamera { get => rigidCamera; }


        public float CalcularAltura(float x, float z, TgcSimpleTerrain terrain)
        {
            var largo = currentScaleXZ * 128;
            var pos_i = 128f * (0.5f + x / largo);
            var pos_j = 128f * (0.5f + z / largo);

            var pi = (int)pos_i;
            var fracc_i = pos_i - pi;
            var pj = (int)pos_j;
            var fracc_j = pos_j - pj;

            if (pi < 0)
                pi = 0;
            else if (pi > 127)
                pi = 127;

            if (pj < 0)
                pj = 0;
            else if (pj > 127)
                pj = 127;

            var pi1 = pi + 1;
            var pj1 = pj + 1;
            if (pi1 > 127)
                pi1 = 127;
            if (pj1 > 127)
                pj1 = 127;

            // 2x2 percent closest filtering usual:
            var H0 = terrain.HeightmapData[pi, pj] * currentScaleY;
            var H1 = terrain.HeightmapData[pi1, pj] * currentScaleY;
            var H2 = terrain.HeightmapData[pi, pj1] * currentScaleY;
            var H3 = terrain.HeightmapData[pi1, pj1] * currentScaleY;
            var H = (H0 * (1 - fracc_i) + H1 * fracc_i) * (1 - fracc_j) + (H2 * (1 - fracc_i) + H3 * fracc_i) * fracc_j;

            return H - DESPLAZAMIENTO_EN_Y;
        }

        public Tiburon Tiburon { get => tiburon; }

    }
}
