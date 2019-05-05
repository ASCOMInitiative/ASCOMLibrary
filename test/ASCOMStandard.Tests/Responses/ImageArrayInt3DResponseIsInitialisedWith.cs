using ASCOM.Alpaca.Responses;
using Xunit;

namespace ASCOM.Alpaca.Test.Responses
{
    public class ImageArrayInt3DResponseIsInitialisedWith
    {
        [Fact]
        public void ImageArrayInt3DResponse_IsInitialisedWith_Rank2_And_TypeInt()
        {
            var response = new ImageArrayInt3DResponse();
            
            Assert.Equal(3, response.Rank);
            Assert.Equal(ImageArrayType.Int, response.ArrayType);
        }
    }
}