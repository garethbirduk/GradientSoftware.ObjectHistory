namespace Gradient.ObjectHistory
{
    /// <summary>
    /// Options for allowing nulls to be set to the history of the object
    /// </summary>
    public enum NullCondition
    {
        /// <summary>
        /// Do not allow null at any time
        /// </summary>
        Disallow,

        /// <summary>
        /// Allow the initial value to be null, but subsequent values may not null
        /// </summary>
        AllowInitial,

        /// <summary>
        /// Allow the value to be null at any time
        /// </summary>
        AllowAll,
    }
}