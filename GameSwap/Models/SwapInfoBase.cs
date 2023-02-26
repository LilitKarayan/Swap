namespace GameSwap.Models
{
    public class SwapInfoBase : Swap
    {
        public string Role { get; set; }
        public string ProposerItemName { get; set; }
        public string Nickname { get; set; }
        public int? Rating { get; set; }
    }
}
