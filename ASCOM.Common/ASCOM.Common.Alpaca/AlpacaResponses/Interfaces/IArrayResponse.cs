namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// ImageArrayResponse interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IArrayResponse<T>
    {
        /// <summary>
        /// The type of data contained in the Value field."/>
        /// </summary>
        ArrayType Type { get; }

        /// <summary>
        /// The array's rank, will be 2 (single plane image (monochrome)) or 3 (multi-plane image).
        /// </summary>
        int Rank { get; }

        /// <summary>
        /// Image data array of rank <see cref="Rank"/> with elements of type <see cref="Type"/>.
        /// </summary>
        T Value { get; set; }

    }
}