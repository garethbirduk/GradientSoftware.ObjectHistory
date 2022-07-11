namespace ObjectHistory
{
    /// <summary>
    /// Options for what to do when the NullCondition is violated. When violated, the code will exit its method, and may either throw an exception or not.
    /// </summary>
    public enum OnNullError
    {
        /// <summary>
        /// When the NullCondition is violated, exit the method, but do not throw any exception.
        /// </summary>
        Ignore,

        /// <summary>
        /// When the NullCondition is violated, throw an exception.
        /// </summary>
        Throw
    }
}