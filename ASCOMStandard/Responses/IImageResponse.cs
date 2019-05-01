namespace ASCOM.Alpaca.Responses
{
    public interface IImageResponse<out T> : IValueResponse<T>
    {
        /// <summary>
        /// minimum: 0
        /// maximum: 3
        /// 0 = Unknown, 1 = Short(int16), 2 = Integer(int32), 3 = Double(Double precision real number).
        /// </summary>
        ImageArrayType ArrayType { get; }

        /// <summary>
        /// The array's rank, will be 2 (single plane image (monochrome)) or 3 (multi-plane image).
        /// </summary>
        int Rank { get; }
    }
}