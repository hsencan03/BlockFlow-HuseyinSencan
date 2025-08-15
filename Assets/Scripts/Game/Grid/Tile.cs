using UnityEngine;

namespace Game.Grid
{
    public class Tile<T> where T : class
    {
        public bool IsEmpty => data == null;
        
        public readonly GridPosition Position;
        public GameObject VisualRepresentation;

        private T data;
        
        public Tile(int x, int y, GameObject visualRepresentation)
        {
            Position = new GridPosition(x, y);
            VisualRepresentation = visualRepresentation;
        }

        public Tile(GridPosition position, GameObject visualRepresentation)
        {
            Position = position;
            VisualRepresentation = visualRepresentation;
        }
        
        public void SetData(T data)
        {
            this.data = data;
        }
        
        public T GetData()
        {
            return data;
        }
        
        public void Destroy()
        {
            if (VisualRepresentation != null)
            {
                Object.Destroy(VisualRepresentation);
                VisualRepresentation = null;
            }
            data = null;
        }
        
        public override string ToString()
        {
            return $"Tile at ({Position.X}, {Position.Y}) with data: {data}";
        }
    }
}