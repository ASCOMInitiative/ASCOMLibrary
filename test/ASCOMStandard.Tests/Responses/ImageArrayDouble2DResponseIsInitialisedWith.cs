using ASCOM.Alpaca.Responses;
using Xunit;

namespace ASCOM.Alpaca.Test.Responses
{
    public class ImageArrayDouble2DResponseIsInitialisedWith
    {
        [Fact]
        public void Rank2_And_TypeDouble()
        {
            var response = new ImageArrayDouble2DResponse();
            
            Assert.Equal(2, response.Rank);
            Assert.Equal(ImageArrayType.Double, response.ArrayType);
        }
    }
}