using ASCOM.Alpaca.Responses;
using Xunit;

namespace ASCOM.Alpaca.Test.Responses
{
    public class ImageArrayShort3DResponseIsInitialisedWith
    {
        [Fact]
        public void ImageArrayShort3DResponse_IsInitialisedWith_Rank2_And_TypeShort()
        {
            var response = new ImageArrayShort3DResponse();
            
            Assert.Equal(3, response.Rank);
            Assert.Equal(ImageArrayType.Short, response.ArrayType);
        }
    }
}