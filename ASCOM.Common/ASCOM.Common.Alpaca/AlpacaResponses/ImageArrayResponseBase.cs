namespace ASCOM.Common.Alpaca
{
    public class ImageArrayResponseBase : Response
    {
        private int rank = 0;
        private ImageArrayElementTypes type = ImageArrayElementTypes.Unknown;

        //[JsonProperty(Order = -3)]
        public int Type
        {
            get { return (int)type; }
            set { type = (ImageArrayElementTypes)value; }
        }

        //[JsonProperty(Order = -2)]
        public int Rank
        {
            get { return rank; }
            set { rank = value; }
        }

    }
}
