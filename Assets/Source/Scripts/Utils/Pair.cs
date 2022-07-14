namespace Utility
{
    /// <summary>
    ///     A Generic pair collection
    /// </summary>
    public class Pair<T1, T2>
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Constructor
        /// </summary>
        public Pair(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The first value in the pair
        /// </summary>
        public T1 First { get; set; }

        /// <summary>
        ///     The second value in the pair
        /// </summary>
        public T2 Second { get; set; }

        #endregion
    }
}