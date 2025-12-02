namespace S5_01_Blazor_CS_GOAT.Models;

public class ThreeDModel {
    public string Uuid { get; set; }
    public int UvType { get; set; }
    public string ItemModel { get; set; }
    public List<Byte[]> Texture { get; set; }
}