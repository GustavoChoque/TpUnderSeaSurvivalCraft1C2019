using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Quadtree
{
    /// <summary>
    ///     Nodo del �rbol Quadtree
    /// </summary>
    internal class QuadtreeNode
    {
        public QuadtreeNode[] children;
        public TgcMesh[] models;

        public bool isLeaf()
        {
            return children == null;
        }
    }
}