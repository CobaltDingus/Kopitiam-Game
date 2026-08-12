using System;
using UnityEngine.Events;

public enum DragEnum
{
    None,
    Ingredient,
    FinishedDrink,
    UnfinishedDrink,
    Tray
}

public static class DragManager
{
    public static bool IsDragging { get; private set; }

    // public static DraggableObject Current { get; private set; }
    public static DragEnum CurrentlyDragging;
    public static event Action<DragEnum> onDragChange;

    public static void BeginDrag(DragEnum dragType)
    {
        IsDragging = true;
        CurrentlyDragging = dragType;
        onDragChange?.Invoke(CurrentlyDragging);
    }

    public static void EndDrag()
    {
        IsDragging = false;
        CurrentlyDragging = DragEnum.None;
        onDragChange?.Invoke(CurrentlyDragging);
    }
}