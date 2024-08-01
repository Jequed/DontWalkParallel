using System.Collections.Generic;

public class UndoRedoManager
{
    private Stack<UndoCollection> undoCollections = new Stack<UndoCollection>();
    private Stack<UndoCollection> redoCollections = new Stack<UndoCollection>();

    private List<IUndoable> currentUndoList = new List<IUndoable>();

    private const int maxStackSize = 10;

    private class UndoCollection
    {
        public List<IUndoable> undos;

        public void Undo()
        {
            var undosReverse = new List<IUndoable>(undos);
            undosReverse.Reverse();

            foreach (var undo in undosReverse)
                undo.Undo();
        }
        public void Redo()
        {
            foreach (var undo in undos)
                undo.Redo();
        }

        public UndoCollection(List<IUndoable> undos)
        {
            this.undos = undos;
        }
    }

    public void AddUndoable(IUndoable undo)
    {
        currentUndoList.Add(undo);
    }

    public void Undo()
    {
        if (undoCollections.Count > 0)
        {
            var collection = undoCollections.Pop();

            redoCollections.Push(collection);

            collection.Undo();
        }
    }

    public void Redo()
    {
        if (redoCollections.Count > 0)
        {
            var collection = redoCollections.Pop();

            undoCollections.Push(collection);

            collection.Redo();
        }
    }

    public void Commit()
    {
        if (currentUndoList.Count > 0)
        {
            if (undoCollections.Count >= maxStackSize)
            {
                var array = undoCollections.ToArray();
                undoCollections.Clear();
                for (int i = maxStackSize - 2; i >= 0; i--)
                    undoCollections.Push(array[i]);
            }
            undoCollections.Push(new UndoCollection(currentUndoList));

            redoCollections.Clear();

            currentUndoList = new List<IUndoable>();
        }
    }
}