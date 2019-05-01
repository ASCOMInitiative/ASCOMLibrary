namespace ASCOM.Alpaca.Responses
{
    public abstract class ImageArrayResponse<T> : Response, IImageResponse<T>
    {
        public virtual T Value { get; set; }
        public ImageArrayType ArrayType { get; set; }
        public int Rank { get; set; }
    }
}