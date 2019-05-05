using ASCOM.Alpaca.Responses;
using Xunit;

namespace ASCOM.Alpaca.Test.Responses
{
    public class ImageArrayInt2DResponseIsInitialisedWith
    {
        [Fact]
        public void Rank2_And_TypeInt()
        {
            var response = new ImageArrayInt2DResponse();
            
            Assert.Equal(2, response.Rank);
            Assert.Equal(ImageArrayType.Int, response.ArrayType);
        }
    }
}