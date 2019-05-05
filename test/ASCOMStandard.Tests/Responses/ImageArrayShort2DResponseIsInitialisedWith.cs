using ASCOM.Alpaca.Responses;
using Xunit;

namespace ASCOM.Alpaca.Test.Responses
{
    public class ImageArrayShort2DResponseIsInitialisedWith
    {
        [Fact]
        public void Rank2_And_TypeShort()
        {
            var response = new ImageArrayShort2DResponse();
            
            Assert.Equal(2, response.Rank);
            Assert.Equal(ImageArrayType.Short, response.ArrayType);
        }
    }
}