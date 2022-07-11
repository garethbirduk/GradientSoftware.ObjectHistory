namespace ObjectHistory
{
    /// <summary>
    /// The undo/redo state
    /// </summary>
    public enum UndoOperation
    {
        /// <summary>
        /// No previous action
        /// </summary>
        None,

        /// <summary>
        /// The previous action was an undo action
        /// </summary>
        Undo,

        /// <summary>
        /// The previous action was a redo action
        /// </summary>
        Redo
    }
}