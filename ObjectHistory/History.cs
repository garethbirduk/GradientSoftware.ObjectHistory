using System.Collections.Immutable;

namespace ObjectHistory
{
    /// <summary>
    /// The object that holds a deep-copy history of the instance
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class History<T>
    {
        /// <summary>
        /// The constructor remains private
        /// </summary>
        /// <param name="nullCondition"></param>
        /// <param name="onNullError"></param>
        private History(NullCondition nullCondition, OnNullError onNullError)
        {
            NullCondition = nullCondition;
            OnNullError = onNullError;
        }

        /// <summary>
        /// Create method
        /// </summary>
        /// <param name="initialData">The initial value of the data</param>
        /// <param name="nullCondition">What is the null condition.</param>
        /// <param name="onNullError">What to do if we violate the nullCondition</param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static History<T> Create(T initialData, NullCondition nullCondition = NullCondition.Disallow, OnNullError onNullError = OnNullError.Throw)
        {
            var history = new History<T>(nullCondition, onNullError);
            if (initialData == null && nullCondition == NullCondition.Disallow)
                throw new NotSupportedException("Intial data cannot be null");
            history.InitialiseValue(initialData);
            return history;
        }

        /// <summary>
        /// The data history dictionary
        /// </summary>
        protected Dictionary<int, T> DataHistory { get; private set; } = new();

        /// <summary>
        /// internal interface as immutable
        /// </summary>
        internal ImmutableDictionary<int, T> Data
        {
            get
            {
                return DataHistory.ToImmutableDictionary();
            }
        }

        /// <summary>
        /// The current value of the object
        /// </summary>
        public T Value
        {
            get
            {
                return DataHistory[Key];
            }
        }

        /// <summary>
        /// The key in the dictionary that corresponds to the current state of the data
        /// </summary>
        private int Key { get; set; } = 0;

        /// <summary>
        /// The null condition
        /// </summary>
        private NullCondition NullCondition { get; }

        /// <summary>
        /// The on null error
        /// </summary>
        private OnNullError OnNullError { get; }

        /// <summary>
        /// Undo the latest action
        /// </summary>
        /// <returns></returns>
        public T Undo()
        {
            if (Key > 0)
                Key--;
            LastUndoOperation = UndoOperation.Undo;
            return DataHistory[Key];
        }

        /// <summary>
        /// Redo the previous action
        /// </summary>
        /// <returns></returns>
        public T Redo()
        {
            if (Key < DataHistory.Count - 1)
                Key++;
            LastUndoOperation = UndoOperation.Redo;
            return DataHistory[Key];
        }

        /// <summary>
        /// add a value to the history
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        internal bool AddValue(T value)
        {
            // null check
            if (value == null)
            {
                if (NullCondition == NullCondition.AllowAll)
                {
                    if (DataHistory[Key] == null)
                    {
                        if (OnNullError == OnNullError.Throw)
                            throw new NotSupportedException("Cannot add null to history as previous value was null");
                        return false;
                    }
                }
                else
                {
                    if (OnNullError == OnNullError.Throw)
                        throw new NotSupportedException("Cannot add null value to history");
                    return false;
                }
            }

            if (DataHistory[Key] != null && DataHistory[Key].Equals(value))
            {
                if (OnNullError == OnNullError.Throw)
                    throw new NotSupportedException("Cannot add value to history as previous value was same");
                return false;
            }

            foreach (var key in DataHistory.Keys.Where(x => x > Key).ToList())
                DataHistory.Remove(key);
            Key++;
            DataHistory.Add(Key, value);
            return true;
        }

        /// <summary>
        /// Set the initial value of the data
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>

        private bool InitialiseValue(T value)
        {
            DataHistory.Add(0, value);
            return true;
        }

        /// <summary>
        /// Insert a value into the history at a position immediately prior to the current history; this prevents redo
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        internal bool InsertBefore(T item)
        {
            foreach (var key in DataHistory.Keys.Where(x => x > Key).ToList())
                DataHistory.Remove(key);
            DataHistory.Add(Key + 1, DataHistory[Key]);
            DataHistory[Key] = item;
            Key++;

            return true;
        }

        /// <summary>
        /// The most recent undo/redo operation performed
        /// </summary>
        public UndoOperation LastUndoOperation = UndoOperation.None;
    }
}