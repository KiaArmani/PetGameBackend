namespace PetGameBackend.Models.Data
{
    public class Tickable
    {
        /// <summary>
        ///     Time in Milliseconds with which the Attribute ticks
        ///     <para>Example: A value of 10000 would mean the value changes every 10 seconds.</para>
        /// </summary>
        public int BaseTickRate { get; set; }
    }
}