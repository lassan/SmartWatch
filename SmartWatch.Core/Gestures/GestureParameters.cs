namespace SmartWatch.Core.Gestures
{
    /// <summary>
    ///     TODO Perhaps it makes more sense to have a discrete set of parameters
    /// </summary>
    public class GestureParameters
    {
        public GestureParameters(int from, int to)
        {
            //TODO - Do some validation 
            From = from;
            To = to;
        }

        public int From { get; set; }

        public int To { get; set; }
    }
}