using ASCOM.Common.Alpaca;
using Xunit;

namespace AlpacaResponses
{
    public class ImageArrayInt2DResponseIsInitialisedWith
    {
        [Fact]
        public void Rank2_And_TypeInt()
        {
            var response = new IntArray2DResponse();

            Assert.Equal(2, response.Rank);
            Assert.Equal(typeof(int[,]), response.Value.GetType());
        }
    }
}