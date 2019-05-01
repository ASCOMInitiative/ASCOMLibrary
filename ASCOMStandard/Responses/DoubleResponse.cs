namespace ASCOM.Alpaca.Responses
{
    public class DoubleResponse : Response, IValueResponse<double>
    {
        public double Value { get; internal set; }
    }
}