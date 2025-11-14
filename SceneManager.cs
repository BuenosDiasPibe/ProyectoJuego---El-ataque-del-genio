using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoJuego
{
    public class SceneManager
    {
        private Stack<IScene> sceneManager;
        public SceneManager()
        {
            sceneManager = new();
        }
        public void AddScene(IScene scene)
        {
            scene.LoadContent();
            sceneManager.Push(scene);
        }
        public void RemoveScene()
        {
            //Maybe unnesesary, but it works so fuck it we ball
            GetScene().UnloadContent();
            sceneManager.Pop();
            sceneManager.Peek().LoadContent();
        }
        public IScene GetScene()
        {
            return sceneManager.Peek();
        }
        public void RemoveAllScenes(){
            sceneManager.Clear();
        }
        public bool hasScenes(){
            return sceneManager.Count > 0;
        }
    }
}