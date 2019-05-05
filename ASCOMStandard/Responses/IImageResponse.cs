namespace ASCOM.Alpaca.Responses
{
    public interface IImageResponse<T> : IValueResponse<T>
    {
        /// <summary>
        /// The type of data contained in <see cref="IValueResponse{T}.Value"/>
        /// </summary>
        ImageArrayType ArrayType { get; }

        /// <summary>
        /// The array's rank, will be 2 (single plane image (monochrome)) or 3 (multi-plane image).
        /// </summary>
        int Rank { get; }
    }
}