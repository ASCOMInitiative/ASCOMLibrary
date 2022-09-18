namespace ASCOM.Common.Alpaca
{
    /// <summary>
    /// Base class for ImageArray responses.
    /// </summary>
    public class ImageArrayResponseBase : Response
    {
        private int rank = 0;
        private ImageArrayElementTypes type = ImageArrayElementTypes.Unknown;

        /// <summary>
        /// Array type
        /// </summary>
        //[JsonProperty(Order = -3)]
        public int Type
        {
            get { return (int)type; }
            set { type = (ImageArrayElementTypes)value; }
        }

        /// <summary>
        /// Array rank
        /// </summary>
        //[JsonProperty(Order = -2)]
        public int Rank
        {
            get { return rank; }
            set { rank = value; }
        }

    }
}
