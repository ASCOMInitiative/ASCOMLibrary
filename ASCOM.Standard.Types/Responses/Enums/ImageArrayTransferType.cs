using System;
using System.Collections.Generic;
using System.Text;

namespace ASCOM.Standard.Responses
{
    // Enum used by the dynamic client to indicate what type of image array transfer should be used
    public enum ImageArrayTransferType
    {
        JSON = 0,
        Base64HandOff = 1,
    }

}
