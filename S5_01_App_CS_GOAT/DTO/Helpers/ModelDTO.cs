namespace S5_01_App_CS_GOAT.DTO.Helpers
{
    public class ModelDTO
    {
        public string Uuid { get; set; } = null!;

        public int UvType { get; set; } // what model we hase to use for the item

        public string ItemModel { get; set; } = null!;

        public byte[]?[] Texture { get; set; } = Array.Empty<byte[]>();
    }
}
