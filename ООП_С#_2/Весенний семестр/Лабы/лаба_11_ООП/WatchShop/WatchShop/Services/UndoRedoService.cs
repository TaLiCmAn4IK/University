using System;
using System.Collections.Generic;

namespace WatchShop.Services
{
    /// <summary>
    /// Сервис Undo/Redo. Хранит стек выполненных операций и стек отменённых.
    /// Каждая операция представлена парой делегатов Do/Undo.
    /// </summary>
    public class UndoRedoService
    {
        private readonly Stack<UndoableAction> undoStack = new();
        private readonly Stack<UndoableAction> redoStack = new();

        public event EventHandler? StateChanged;

        public bool CanUndo => undoStack.Count > 0;
        public bool CanRedo => redoStack.Count > 0;

        /// <summary>
        /// Регистрирует уже выполненное действие — складывает в стек Undo.
        /// Стек Redo при этом очищается (как принято в редакторах).
        /// </summary>
        public void Push(UndoableAction action)
        {
            undoStack.Push(action);
            redoStack.Clear();
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Undo()
        {
            if (!CanUndo) return;
            var action = undoStack.Pop();
            action.Undo();
            redoStack.Push(action);
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Redo()
        {
            if (!CanRedo) return;
            var action = redoStack.Pop();
            action.Do();
            undoStack.Push(action);
            StateChanged?.Invoke(this, EventArgs.Empty);
        }

        public void Clear()
        {
            undoStack.Clear();
            redoStack.Clear();
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>Описание одного отменяемого действия.</summary>
    public class UndoableAction
    {
        public string Description { get; }
        public Action Do { get; }
        public Action Undo { get; }

        public UndoableAction(string description, Action doAction, Action undoAction)
        {
            Description = description;
            Do = doAction;
            Undo = undoAction;
        }
    }
}
