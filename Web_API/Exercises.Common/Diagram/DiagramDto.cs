namespace Exercises.Common.Diagram
{
    public class DiagramDto : EntityDtoBase
    {     
        /// <summary>
        /// A url string of an image for this diagram.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// A json definition of this diagram.
        /// </summary>
        public string Definition { get; set; }
    }
}
